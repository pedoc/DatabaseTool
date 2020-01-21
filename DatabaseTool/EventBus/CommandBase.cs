using System;
using Redbus.Events;

namespace DatabaseTool.EventBus
{
   public class CommandBase:EventBase
   {
       public readonly Guid Sender;

       public CommandBase(Guid sender)
       {
           Sender = sender;
       }
   }
}
