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
        public MainForm()
        {
            InitializeComponent();
        }

        private const string DefaultServerListConfigFile = "servers.json";

        private void MainForm_Load(object sender, EventArgs e)
        {
            List<DatabaseServer> dbs = null;
            if (File.Exists(DefaultServerListConfigFile))
            {
                dbs = File.ReadAllText(DefaultServerListConfigFile, Encoding.UTF8).FromJson<List<DatabaseServer>>();
                gridControl.DataSource = dbs;
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
            var view = new DatabaseView(databaseServer)
            {
                Dock = DockStyle.Fill
            };
            newPage.Controls.Add(view);
            xtraTabControl.TabPages.Add(newPage);
            xtraTabControl.SelectedTabPage = newPage;
        }
    }
}
