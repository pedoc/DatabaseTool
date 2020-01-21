using System;

namespace DatabaseTool.EventBus
{
    public class RemoteExecuteCommand : CommandBase
    {
        public readonly string Sql;
        public RemoteExecuteCommand(Guid sender, string sql) : base(sender)
        {
            Sql = sql;
        }
    }
}
