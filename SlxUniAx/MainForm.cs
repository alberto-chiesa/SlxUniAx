using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gianos.UniLib;

namespace SlxUniAx
{
    public partial class Form1 : Form
    {
        private DbHandler dbHandler;
        private bool isDbHandlerInitialized;

        private SLXModelHandler slxModelHandler;
        private bool isSlxModelInitialized;

        private FieldInformationCollection fields;

        public void Log(string message)
        {
            this.textBox1.AppendText(message);
            this.textBox1.AppendText("\r\n");
            this.Refresh();
        }

        private void CleanLog()
        {
            this.textBox1.Text = String.Empty;
            this.Refresh();
        }

        public Form1()
        {
            InitializeComponent();
            isDbHandlerInitialized = false;
            isSlxModelInitialized = false;
        }

        private void btnSelModel_Click(object sender, EventArgs e)
        {
            modelBrowser.SelectedPath = txtFolderModel.Text;
            modelBrowser.Description = "Select the Model folder inside your File System Project";

            if (modelBrowser.ShowDialog() == DialogResult.OK)
            {
                txtFolderModel.Text = modelBrowser.SelectedPath;
            }
        }

        private void btnTestDb_Click(object sender, EventArgs e)
        {
            this.CleanLog();

            try
            {
                this.Log("Performing Db Test...");
                dbHandler = new DbHandler(txtServer.Text, txtDatabase.Text, txtUser.Text, txtPassword.Text);
            }
            catch (Exception exc)
            {
                this.Log("Test Failed! Logged Exception:");
                this.Log(exc.Message);
                isDbHandlerInitialized = false;

                return;
            }

            isDbHandlerInitialized = true;
            this.RefreshLoadButton();

            this.Log("Database Connection Completed!\r\nTest Ok");
        }

        private void btnTestModel_Click(object sender, EventArgs e)
        {
            this.CleanLog();

            try
            {
                this.Log("Performing Model Test...");
                slxModelHandler = new SLXModelHandler(this.txtFolderModel.Text);
            }
            catch (Exception exc)
            {
                this.Log("Test Failed! Logged Exception:");
                this.Log(exc.Message);
                isSlxModelInitialized = false;

                return;
            }

            isSlxModelInitialized = true;
            this.RefreshLoadButton();

            this.Log("Model Test Ok!");
        }

        private void RefreshLoadButton()
        {
            this.btnLoad.Enabled = isSlxModelInitialized && isDbHandlerInitialized;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                this.CleanLog();
                this.Log("Started metadata collection...");
                this.Log("");
                this.Log("Accessing db data...");

                this.fields = this.dbHandler.ReadTableDataFromSLXDb();

                this.Log("done. Reading data from model...");

                this.fields = this.slxModelHandler.FindEntityModels(fields);
                this.Log("Done. Load completed succesfully!");

                this.FillTreeView();

                this.tabControl1.SelectedTab = this.tabFields;
            }
            catch (Exception exc)
            {
                this.Log("Whoops. Seems something went wrong.");
                this.Log("Caught exception:");
                this.Log(exc.Message);
            }
        }

        private void FillTreeView()
        {
            treeFields.Nodes.Clear();

            string[] tables = fields.GetTablesList();

            foreach(string tableName in tables)
            {
                var tableNode = treeFields.Nodes.Add(tableName);

                //tableNode.Nodes
                var tableFields = fields[tableName];
                foreach (var fieldName in tableFields.Keys)
                {
                    // Skipping fields with no slx Text type or no Sql type
                    // Could leverage some special error?
                    if (String.IsNullOrEmpty(tableFields[fieldName].sqlType)
                        ||
                        String.IsNullOrEmpty(tableFields[fieldName].slxType))
                        continue;

                    var fieldNode = tableNode.Nodes.Add(fieldName);

                    fieldNode.Tag = tableFields[fieldName];
                }

                tableNode.Collapse();
            }
        }

        private void treeFields_AfterSelect(object sender, TreeViewEventArgs e)
        {
            const string descriptionTemplate =
                @"{0}
{1}
{2}({3})
{4}({5})";
            FieldInformation selectedField = e.Node.Tag as FieldInformation;

            string fieldDescription = String.Empty;

            if (selectedField != null)
            {
                fieldDescription = String.Format(descriptionTemplate,
                    selectedField.tableName,
                    selectedField.fieldName,
                    selectedField.slxType,
                    selectedField.slxLength,
                    selectedField.sqlType,
                    selectedField.sqlLength);
            }

            lblFieldDesc.Text = fieldDescription;
        }

        private void btnSetUnicode_Click(object sender, EventArgs e)
        {
            SetUnicodeness(FieldState.Unicode);
        }

        private void btnSetAnsi_Click(object sender, EventArgs e)
        {
            SetUnicodeness(FieldState.Ansi);
        }

        private void SetUnicodeness(FieldState newState)
        {
            FieldInformation selectedField = treeFields.SelectedNode.Tag as FieldInformation;

            string fieldDescription = String.Empty;

            if (selectedField != null)
            {
                selectedField.NewState = newState;
            }
        }

        /// <summary>
        /// Let's wreck database AND model! Yai!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDoDamage_Click(object sender, EventArgs e)
        {
            this.CleanLog();
            this.Log("Reading actions to be performed...");

            var actions = fields.GetActions();

            this.Log(actions.Length + " found.");

            var res = ConfirmActions(actions);

            if (res == DialogResult.Yes)
            {
                this.dbHandler.ApplyActionsToDb(actions);
                this.slxModelHandler.ApplyActionsToModel(actions);
            }
        }

        /// <summary>
        /// Displays a message box asking for confirmation of the selected
        /// actions
        /// </summary>
        /// <param name="actions">The list of the actions to be performed
        /// onto fields</param>
        /// <returns></returns>
        private static DialogResult ConfirmActions(FieldAction[] actions)
        {

            var sb = new StringBuilder();
            sb.AppendLine("Fields to be updated:");
            for (int i = 0; i < Math.Min(10, actions.Length); i++)
            {
                sb.AppendLine(actions[i].ToString());
            }

            if (actions.Length > 10)
                sb.AppendLine((actions.Length - 10).ToString() + " more actions to be performed.");

            sb.AppendLine("Are you sure you want to continue?");

            var res = MessageBox.Show(sb.ToString(), "Watch out!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            return res;
        }
    }
}
