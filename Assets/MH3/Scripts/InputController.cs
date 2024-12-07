using System.Collections.Generic;

namespace MH3
{
    public class InputController
    {
        public enum InputActionType
        {
            Player,
            UI,
        }

        public MHInputActions Actions { get; }

        private readonly Stack<InputActionType> actionTypes = new();

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
