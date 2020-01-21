using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseTool.Entity;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace DatabaseTool.ViewModel
{
    public class DatabaseServer
    {
        public DatabaseType DatabaseType { get; set; }
        public string DisplayName { get; set; }
        public string ConnectionString { get; set; }

        public bool Checked { get; set; }

        public IDataLayer GetDataLayer()
        {
         return   XpoDefault.GetDataLayer(GetXpoConnectionString(), AutoCreateOption.None);
        }

        public string GetXpoConnectionString()
        {
            return $"{ConnectionString};XpoProvider={GetXpoProvider()}";
        }

        public string GetXpoProvider()
        {
            switch (DatabaseType)
            {
                case DatabaseType.Mysql:
                    return "MySql;";
                case DatabaseType.Oracle:
                    return "ODPManaged;";
                case DatabaseType.SqlServer:
                    return "MSSqlServer;";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
