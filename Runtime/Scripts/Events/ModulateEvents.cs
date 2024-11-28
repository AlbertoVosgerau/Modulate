namespace DandyDino.Modulate
{
    public struct OnInitializeManager : IEvent
    {
        public IManager Manager;

        public OnInitializeManager(IManager manager)
        {
            Manager = manager;
        }
    }
    
    public struct OnInitializeServer : IEvent
    {
        public IService Service;

        public OnInitializeServer(IService service)
        {
            Service = service;
        }
    }
    
    public struct OnEnableController : IEvent
    {
        public IManager Manager;

        public OnEnableController(IManager manager)
        {
            Manager = manager;
        }
    }
    
    public struct OnDisableManager : IEvent
    {
        public IManager Manager;

        public OnDisableManager(IManager manager)
        {
            Manager = manager;
        }
    }
    
    public struct OnDisposeManager : IEvent
    {
        public IManager Manager;

        public OnDisposeManager(IManager manager)
        {
            Manager = manager;
        }
    }
    
    public struct OnDisposeService : IEvent
    {
        public IService Service;

        public OnDisposeService(IService service)
        {
            Service = service;
        }
    }
    
    public struct OnManagerAskForDisposal : IEvent
    {
        public IManager Manager;

        public OnManagerAskForDisposal(IManager manager)
        {
            Manager = manager;
        }
    }
}
