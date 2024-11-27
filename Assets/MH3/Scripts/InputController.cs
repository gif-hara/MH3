namespace MH3
{
    public class InputController
    {
        public MHInputActions Actions { get; }
        
        public InputController()
        {
            Actions = new MHInputActions();
            Actions.Enable();
        }
    }
}
