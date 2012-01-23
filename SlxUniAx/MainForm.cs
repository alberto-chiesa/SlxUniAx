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
    public enum StatusIcons
    {
        Table = 0,
        Text = 1,
        Unicode = 2,
        ToText = 3,
        ToUnicode = 4,
        Error = 5
    }

    public partial class Form1 : Form
    {
        private bool isDbHandlerInitialized;

        private bool isSlxModelInitialized;

        private FieldInformationManager fields;

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

        private const string strKeepCurrent = "Keep Current";
        public Form1()
        {
            InitializeComponent();
            isDbHandlerInitialized = false;
            isSlxModelInitialized = false;
            this.fields = new FieldInformationManager(this.Log);

            this.cmbNewSize.Text = strKeepCurrent;
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
            DbHandler dbHandler;
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

            fields.LinkToDb(dbHandler);

            this.RefreshLoadButton();

            this.Log("Database Connection Completed!\r\nTest Ok");
        }

        private void btnTestModel_Click(object sender, EventArgs e)
        {
            SLXModelHandler slxModelHandler;

            this.CleanLog();

            try
            {
                this.Log("Performing Model Test...");

                slxModelHandler = (this.chkUseVFS.Checked) ?
                    new SLXModelHandler(txtServer.Text, txtDatabase.Text, txtUser.Text, txtPassword.Text) :
                    new SLXModelHandler(this.txtFolderModel.Text);
            }
            catch (Exception exc)
            {
                this.Log("Test Failed! Logged Exception:");
                this.Log(exc.Message);
                isSlxModelInitialized = false;

                return;
            }

            isSlxModelInitialized = true;

            fields.LinkToSlxModel(slxModelHandler);

            this.RefreshLoadButton();

            this.Log("Model Test Ok!");
        }

        private void RefreshLoadButton()
        {
            this.btnLoad.Enabled = isSlxModelInitialized && isDbHandlerInitialized;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadFieldInformations(true);
        }

        /// <summary>
        /// Triggers Field Information load and treeview rebuil
        /// </summary>
        /// <param name="doCleanLog"></param>
        private void LoadFieldInformations(bool doCleanLog)
        {
            if (doCleanLog) this.CleanLog();

            var loadOk = fields.LoadFieldInformations();

            if (!loadOk) return;

            this.FillTreeView();
        }

        /// <summary>
        /// Builds the treeview from FieldInformation data
        /// </summary>
        private void FillTreeView()
        {
            treeFields.Nodes.Clear();

            foreach (string tableName in fields.GetTablesList())
            {
                var tableNode = treeFields.Nodes.Add(tableName);
                tableNode.ImageIndex = (int)StatusIcons.Table;

                //tableNode.Nodes
                var tableFields = fields[tableName];
                foreach (var fieldName in tableFields.Keys)
                {
                    var field = tableFields[fieldName];
                    // Skipping fields with no slx Text type or no Sql type
                    // Could leverage some special error?
                    if (!field.IsATextField) continue;

                    var fieldNode = tableNode.Nodes.Add(fieldName);

                    // link the node to the corresponding FieldInformation object
                    fieldNode.Tag = field;

                    StatusIcons nodeIconIndex = GetIconIndexForField(field);

                    fieldNode.SelectedImageIndex = fieldNode.ImageIndex = (int)nodeIconIndex;

                    // error condition propagates to table node
                    if (nodeIconIndex == StatusIcons.Error)
                        tableNode.SelectedImageIndex = tableNode.ImageIndex = (int)nodeIconIndex;
                }

                // collapse all table nodes
                tableNode.Collapse();
            }

            // switch tab
            lblFieldDesc.Text = String.Empty;

            this.tabControlUpper.SelectedTab = this.tabFields;
        }

        /// <summary>
        /// Gets the icon index to show for each field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static StatusIcons GetIconIndexForField(FieldInformation field)
        {
            if (field.State == FieldState.Ansi)
                return StatusIcons.Text;
            if (field.State == FieldState.Unicode)
                return StatusIcons.Unicode;
            
            return StatusIcons.Error;
        }

        /// <summary>
        /// Refresh the field description label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeFields_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!chkKeepSize.Checked)
                cmbNewSize.Text = strKeepCurrent;

            lblFieldDesc.Text = String.Empty;

            FieldInformation selectedField = e.Node.Tag as FieldInformation;

            if (selectedField != null) lblFieldDesc.Text = selectedField.ToString();
        }

        /// <summary>
        /// Set-to-Unicode button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetUnicode_Click(object sender, EventArgs e)
        {
            SetUnicodeness(FieldState.Unicode);
        }

        /// <summary>
        /// Set-to-Text button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetAnsi_Click(object sender, EventArgs e)
        {
            SetUnicodeness(FieldState.Ansi);
        }

        /// <summary>
        /// Sets the new state of a field to Unicode or text
        /// </summary>
        /// <param name="newState"></param>
        private void SetUnicodeness(FieldState newState)
        {
            if (treeFields.SelectedNode == null) return;

            FieldInformation selectedField = treeFields.SelectedNode.Tag as FieldInformation;

            if (selectedField != null)
            {
                selectedField.NewState = newState;

                int newSize;

                if (Int32.TryParse(cmbNewSize.Text, out newSize))
                {
                    selectedField.NewLength = newSize;
                }

                treeFields.SelectedNode.SelectedImageIndex = treeFields.SelectedNode.ImageIndex =
                    (int)(newState == FieldState.Unicode ? StatusIcons.ToUnicode : StatusIcons.ToText);
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

            if (this.ConfirmActions() == DialogResult.Yes)
            {
                fields.RunActions();

                this.Log("");
                this.Log("Reloading entity data...");

                LoadFieldInformations(false);
            }
        }

        /// <summary>
        /// Displays a message box asking for confirmation of the selected
        /// actions
        /// </summary>
        /// <param name="actions">The list of the actions to be performed
        /// onto fields</param>
        /// <returns></returns>
        private DialogResult ConfirmActions()
        {
            FieldAction[] actions = fields.GetActionArray();
            var sb = new StringBuilder();
            sb.AppendLine("Fields to be updated:");
            
            for (int i = 0; i < Math.Min(10, actions.Length); i++)
                sb.AppendLine(actions[i].ToString());

            if (actions.Length > 10)
                sb.AppendLine((actions.Length - 10).ToString() + " more actions to be performed.");

            sb.AppendLine("Are you sure you want to continue?");

            return MessageBox.Show(sb.ToString(), "Watch out!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// If using VFS, disable model folder button and text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseVFS_CheckedChanged(object sender, EventArgs e)
        {
            btnSelModel.Enabled = txtFolderModel.Enabled = !chkUseVFS.Checked;
            
            this.Refresh();
        }

    }
}
