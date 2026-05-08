namespace DandyDino.Modulate
{
    public struct RegisterViewEvt : IEvent
    {
        public IView view;
    }
    
    public struct UnRegisterViewEvt : IEvent
    {
        public IView view;
    }
}