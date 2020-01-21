using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseTool.Controls;
using DatabaseTool.Entity;
using DatabaseTool.ViewModel;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using Oracle.ManagedDataAccess.Client;

namespace DatabaseTool
{
    public partial class MainForm : XtraForm
    {
        private  List<DatabaseServer> _databaseServers;
        public MainForm()
        {
            InitializeComponent();
        }

        private const string DefaultServerListConfigFile = "servers.json";

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(DefaultServerListConfigFile))
            {
                _databaseServers = File.ReadAllText(DefaultServerListConfigFile, Encoding.UTF8).FromJson<List<DatabaseServer>>();
                gridControl.DataSource = _databaseServers;
            }
        }

        private void gridView_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            var selectedRows = gridView.GetSelectedRows()
                .Select(item => gridView.GetRow(item) as DatabaseServer)
                .Where(item => item != null)
                .ToList();
            foreach (var row in selectedRows)
            {
                if (xtraTabControl.TabPages.All(item => item.Text != row.DisplayName)) AddTabPage(row);
            }
            var tabPages = xtraTabControl.TabPages.ToList();
            tabPages.Where(item => selectedRows.All(x => item.Text != x.DisplayName))
                .ForEach(RemovePage);
        }

        private void RemovePage(XtraTabPage tabPage)
        {
            xtraTabControl.TabPages.Remove(tabPage);
        }
        private void AddTabPage(DatabaseServer databaseServer)
        {
            XtraTabPage newPage = new XtraTabPage
            {
                Text = databaseServer.DisplayName,
                Tooltip = databaseServer.ConnectionString,
                Tag = databaseServer
            };
            var view = new DatabaseView(newPage,databaseServer)
            {
                Dock = DockStyle.Fill
            };
            newPage.Controls.Add(view);
            xtraTabControl.TabPages.Add(newPage);
            xtraTabControl.SelectedTabPage = newPage;
        }

        private void gridView_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            var databaseServer = gridView.GetFocusedRow() as DatabaseServer;
            if (databaseServer != null)
            {
                var tab = xtraTabControl.TabPages.FirstOrDefault(item => item.Text == databaseServer.DisplayName);
                if (tab != null)
                {
                    xtraTabControl.SelectedTabPage = tab;
                }
            }
        }

        private void gridView_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            if (e.Row is DatabaseServer databaseServer)
            {
                alertControl.Show(this,"通知","数据库信息已修改，将在关闭时自动保存");
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_databaseServers != null)
            {
                File.WriteAllText(DefaultServerListConfigFile,_databaseServers.ToJson());
            }
        }
    }
}
