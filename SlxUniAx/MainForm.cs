﻿using System;
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
        private DbHandler dbHandler;
        private bool isDbHandlerInitialized;

        //private SLXModelHandler slxModelHandler;
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

        public Form1()
        {
            InitializeComponent();
            isDbHandlerInitialized = false;
            isSlxModelInitialized = false;
            this.fields = new FieldInformationManager(this.Log);
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

            string[] tables = fields.GetTablesList();

            foreach(string tableName in tables)
            {
                var tableNode = treeFields.Nodes.Add(tableName);
                tableNode.ImageIndex = (int)StatusIcons.Table;

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

                    // link the node to the corresponding FieldInformation object
                    fieldNode.Tag = tableFields[fieldName];

                    StatusIcons nodeIcon =
                        tableFields[fieldName].State == FieldState.Ansi ? StatusIcons.Text :
                        tableFields[fieldName].State == FieldState.Unicode ? StatusIcons.Unicode :
                        StatusIcons.Error;

                    fieldNode.SelectedImageIndex = fieldNode.ImageIndex = (int)nodeIcon;

                    // error condition propagates to table node
                    if (nodeIcon == StatusIcons.Error)
                        tableNode.SelectedImageIndex = tableNode.ImageIndex = (int)nodeIcon;
                }

                // collapse all table nodes
                tableNode.Collapse();
            }

            // switch tab
            this.tabControlUpper.SelectedTab = this.tabFields;
        }

        /// <summary>
        /// Refresh the field description label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeFields_AfterSelect(object sender, TreeViewEventArgs e)
        {
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
            FieldInformation selectedField = treeFields.SelectedNode.Tag as FieldInformation;

            if (selectedField != null)
            {
                selectedField.NewState = newState;

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
                fields.PerformActions();

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
            FieldAction[] actions = fields.GetActions();

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
