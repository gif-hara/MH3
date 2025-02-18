using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine.InputSystem;

namespace MH3
{
    public class InputController
    {
        public enum InputActionType
        {
            Player,
            UI,
            Disable,
        }

        public MHInputActions Actions { get; }

        private readonly Stack<InputActionType> actionTypes = new();
        
        private InputActionRebindingExtensions.RebindingOperation rebindOperation = null;
        
        public enum RebindingResult
        {
            Completed,
            Canceled,
        }
        
        public InputController()
        {
            Actions = new MHInputActions();
            PushActionType(InputActionType.Player);
            Actions.Enable();
        }

        public void PushActionType(InputActionType type)
        {
            actionTypes.Push(type);
            ChangeInputType(type);
        }

        public void PopActionType()
        {
            actionTypes.Pop();
            ChangeInputType(actionTypes.Peek());
        }

        public UniTask<RebindingResult> BeginRebindingAsync(InputAction inputAction, InputScheme.InputSchemeType schemeType)
        {
            rebindOperation?.Cancel();
            inputAction.Disable();
            PushActionType(InputActionType.Disable);
            var bindingIndex = inputAction.GetBindingIndex(InputBinding.MaskByGroup(InputScheme.GetSchemeName(schemeType)));
            var source = new UniTaskCompletionSource<RebindingResult>();
            rebindOperation = inputAction.PerformInteractiveRebinding(bindingIndex)
                .OnComplete(_ =>
                {
                    source.TrySetResult(RebindingResult.Completed);
                    OnFinished();
                })
                .OnCancel(_ =>
                {
                    source.TrySetResult(RebindingResult.Canceled);
                    OnFinished();
                })
                .WithControlsExcluding("<Gamepad>/leftStick/left")
                .WithControlsExcluding("<Gamepad>/leftStick/up")
                .WithControlsExcluding("<Gamepad>/leftStick/right")
                .WithControlsExcluding("<Gamepad>/leftStick/down")
                .WithControlsExcluding("<Gamepad>/rightStick/left")
                .WithControlsExcluding("<Gamepad>/rightStick/up")
                .WithControlsExcluding("<Gamepad>/rightStick/right")
                .WithControlsExcluding("<Gamepad>/rightStick/down")
                .Start();
            
            return source.Task;
            
            void OnFinished()
            {
                rebindOperation?.Dispose();
                rebindOperation = null;
                inputAction.Enable();
                PopActionType();
            }
        }
        
        public UniTask<RebindingResult> BeginRebindingAsync(InputAction inputAction)
        {
            return BeginRebindingAsync(inputAction, TinyServiceLocator.Resolve<InputScheme>().CurrentInputSchemeType);
        }

        public InputAction FindAction(InputActionReference reference)
        {
            return Actions.FindAction(reference.action.name);
        }

        public void LoadBindingOverrideFromJson(string json)
        {
            Actions.LoadBindingOverridesFromJson(json, false);
        }

        public void LoadBindingOverrideFromJson(List<string> jsons)
        {
            foreach (var json in jsons)
            {
                LoadBindingOverrideFromJson(json);
            }
        }

        private void ChangeInputType(InputActionType type)
        {
            Actions.Player.Disable();
            Actions.UI.Disable();
            switch (type)
            {
                case InputActionType.Player:
                    Actions.Player.Enable();
                    break;
                case InputActionType.UI:
                    Actions.UI.Enable();
                    break;
            }
        }
    }
}
