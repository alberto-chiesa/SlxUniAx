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

    }
}
