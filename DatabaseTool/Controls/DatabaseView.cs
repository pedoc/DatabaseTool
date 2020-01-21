using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DatabaseTool.EventBus;
using DatabaseTool.ViewModel;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.XtraTab;


namespace DatabaseTool.Controls
{
    public partial class DatabaseView : UserControl
    {
        public bool AllServer { get; private set; }
        public readonly Guid Id = Guid.NewGuid();

        private readonly DatabaseServer _databaseServer;
        private readonly XtraTabPage _tabPage;

        public DatabaseView()
        {
            InitializeComponent();
            EventBusFactory.Default.Subscribe<RemoteExecuteCommand>((cmd) =>
            {
                if (cmd.Sender == Id) return;
                else Execute(cmd.Sql, true);
            });
        }

        public DatabaseView(XtraTabPage tab, DatabaseServer databaseServer) : this()
        {
            _tabPage = tab;
            _databaseServer = databaseServer;
        }

        private void checkEdit_CheckStateChanged(object sender, EventArgs e)
        {
            AllServer = checkEdit.Checked;
        }

        private void btn_ExecuteAll_Click(object sender, EventArgs e)
        {
            Execute(txt_Sql.Text.Trim());
        }

        private void btn_ExecuteSelected_Click(object sender, EventArgs e)
        {
            Execute(txt_Sql.SelectedText.Trim());
        }

        private void Execute(string sql, bool remoteCommand = false)
        {
            try
            {
                if (string.IsNullOrEmpty(sql)) return;
                if (!remoteCommand) EventBusFactory.Default.Publish(new RemoteExecuteCommand(Id, sql));
                var session = new Session(_databaseServer.GetDataLayer());
                if (sql.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                {
                    gridView.Columns.Clear();
                    XPDataView dv = new XPDataView();
                    SelectedData data = session.ExecuteQueryWithMetadata(sql);
                    foreach (var row in data.ResultSet[0].Rows)
                    {
                        dv.AddProperty((string)row.Values[0],
                            DBColumn.GetType((DBColumnType)Enum.Parse(typeof(DBColumnType), (string)row.Values[2])));
                    }
                    dv.LoadData(new SelectedData(data.ResultSet[1])); //如果包含多个结果将丢弃
                    gridControl.DataSource = dv;
                    PrintLog($"数据行数={dv.Count}");
                }

                else
                {
                    var count = session.ExecuteNonQuery(sql);
                    PrintLog($"受影响行数={count}");
                }
                session.Disconnect();
                _tabPage.Appearance.HeaderActive.BackColor = Color.Empty;
            }
            catch (Exception ex)
            {
                PrintLog($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                _tabPage.Appearance.HeaderActive.BackColor=Color.Brown;
            }
        }

        private void PrintLog(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            LogHelper.Info(msg);
            // ReSharper disable once LocalizableElement
            txt_Log.Text += $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - {msg}{Environment.NewLine}";
            txt_Log.SelectionStart = txt_Log.Text.Length;
            txt_Log.ScrollToCaret();
        }
    }
}
