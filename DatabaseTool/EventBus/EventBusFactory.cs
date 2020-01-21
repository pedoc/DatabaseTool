using Redbus.Interfaces;

namespace DatabaseTool.EventBus
{
    public static class EventBusFactory
    {
        public static IEventBus Default=new Redbus.EventBus();
    }
}
