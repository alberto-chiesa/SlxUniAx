namespace SlxUniAx
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSetup = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnTestDb = new System.Windows.Forms.Button();
            this.txtFolderModel = new System.Windows.Forms.TextBox();
            this.btnSelModel = new System.Windows.Forms.Button();
            this.btnTestModel = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.tabFields = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.treeFields = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSetUnicode = new System.Windows.Forms.Button();
            this.btnSetAnsi = new System.Windows.Forms.Button();
            this.lblFieldDesc = new System.Windows.Forms.Label();
            this.tabOutput = new System.Windows.Forms.TabControl();
            this.tabPageOutput = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.modelBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.btnDoDamage = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSetup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabFields.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tabOutput.SuspendLayout();
            this.tabPageOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            label1.Location = new System.Drawing.Point(3, 8);
            label1.MinimumSize = new System.Drawing.Size(80, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(144, 13);
            label1.TabIndex = 5;
            label1.Text = "SQL Server Name";
            // 
            // label2
            // 
            label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            label2.Location = new System.Drawing.Point(3, 38);
            label2.MinimumSize = new System.Drawing.Size(80, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(144, 13);
            label2.TabIndex = 6;
            label2.Text = "DataBase Name";
            // 
            // label3
            // 
            label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            label3.Location = new System.Drawing.Point(3, 68);
            label3.MinimumSize = new System.Drawing.Size(80, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(144, 13);
            label3.TabIndex = 7;
            label3.Text = "SQL User";
            // 
            // label4
            // 
            label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            label4.Location = new System.Drawing.Point(3, 98);
            label4.MinimumSize = new System.Drawing.Size(80, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(144, 13);
            label4.TabIndex = 8;
            label4.Text = "SQL Password";
            // 
            // label5
            // 
            label5.Dock = System.Windows.Forms.DockStyle.Fill;
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label5.Location = new System.Drawing.Point(3, 0);
            label5.Name = "label5";
            this.tableLayoutPanel3.SetRowSpan(label5, 4);
            label5.Size = new System.Drawing.Size(74, 120);
            label5.TabIndex = 3;
            label5.Text = "Table:\r\nColumn:\r\nSlxType:\r\nDbType:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabOutput);
            this.splitContainer1.Size = new System.Drawing.Size(581, 437);
            this.splitContainer1.SplitterDistance = 311;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSetup);
            this.tabControl1.Controls.Add(this.tabFields);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(581, 311);
            this.tabControl1.TabIndex = 0;
            // 
            // tabSetup
            // 
            this.tabSetup.Controls.Add(this.tableLayoutPanel1);
            this.tabSetup.Location = new System.Drawing.Point(4, 22);
            this.tabSetup.Name = "tabSetup";
            this.tabSetup.Padding = new System.Windows.Forms.Padding(3);
            this.tabSetup.Size = new System.Drawing.Size(573, 285);
            this.tabSetup.TabIndex = 0;
            this.tabSetup.Text = "Setup";
            this.tabSetup.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.txtServer, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtDatabase, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtUser, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtPassword, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnTestDb, 1, 4);
            this.tableLayoutPanel1.Controls.Add(label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtFolderModel, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnSelModel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnTestModel, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.btnLoad, 0, 8);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(567, 279);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txtServer
            // 
            this.txtServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServer.Location = new System.Drawing.Point(153, 5);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(411, 20);
            this.txtServer.TabIndex = 0;
            this.txtServer.Text = "localhost";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabase.Location = new System.Drawing.Point(153, 35);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(411, 20);
            this.txtDatabase.TabIndex = 1;
            this.txtDatabase.Text = "saleslogix_eval";
            // 
            // txtUser
            // 
            this.txtUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUser.Location = new System.Drawing.Point(153, 65);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(411, 20);
            this.txtUser.TabIndex = 2;
            this.txtUser.Text = "sysdba";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(153, 95);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(411, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.Text = "masterkey";
            // 
            // btnTestDb
            // 
            this.btnTestDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestDb.Location = new System.Drawing.Point(153, 123);
            this.btnTestDb.Name = "btnTestDb";
            this.btnTestDb.Size = new System.Drawing.Size(411, 23);
            this.btnTestDb.TabIndex = 4;
            this.btnTestDb.Text = "Init Db Connection";
            this.btnTestDb.UseVisualStyleBackColor = true;
            this.btnTestDb.Click += new System.EventHandler(this.btnTestDb_Click);
            // 
            // txtFolderModel
            // 
            this.txtFolderModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderModel.Location = new System.Drawing.Point(153, 185);
            this.txtFolderModel.Name = "txtFolderModel";
            this.txtFolderModel.Size = new System.Drawing.Size(411, 20);
            this.txtFolderModel.TabIndex = 9;
            this.txtFolderModel.Text = "C:\\Users\\ACA.GIANOS\\Documents\\Dev\\bvweb\\Model";
            // 
            // btnSelModel
            // 
            this.btnSelModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelModel.Location = new System.Drawing.Point(3, 183);
            this.btnSelModel.Name = "btnSelModel";
            this.btnSelModel.Size = new System.Drawing.Size(144, 23);
            this.btnSelModel.TabIndex = 10;
            this.btnSelModel.Text = "Select Model Folder";
            this.btnSelModel.UseVisualStyleBackColor = true;
            this.btnSelModel.Click += new System.EventHandler(this.btnSelModel_Click);
            // 
            // btnTestModel
            // 
            this.btnTestModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestModel.Location = new System.Drawing.Point(153, 213);
            this.btnTestModel.Name = "btnTestModel";
            this.btnTestModel.Size = new System.Drawing.Size(411, 23);
            this.btnTestModel.TabIndex = 11;
            this.btnTestModel.Text = "Test Model Folder";
            this.btnTestModel.UseVisualStyleBackColor = true;
            this.btnTestModel.Click += new System.EventHandler(this.btnTestModel_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.btnLoad, 2);
            this.btnLoad.Enabled = false;
            this.btnLoad.Location = new System.Drawing.Point(3, 253);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(561, 23);
            this.btnLoad.TabIndex = 12;
            this.btnLoad.Text = "Load Field Data!";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // tabFields
            // 
            this.tabFields.Controls.Add(this.tableLayoutPanel2);
            this.tabFields.Location = new System.Drawing.Point(4, 22);
            this.tabFields.Name = "tabFields";
            this.tabFields.Padding = new System.Windows.Forms.Padding(3);
            this.tabFields.Size = new System.Drawing.Size(573, 285);
            this.tabFields.TabIndex = 1;
            this.tabFields.Text = "Field Details";
            this.tabFields.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.btnDoDamage, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.treeFields, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(567, 279);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // treeFields
            // 
            this.treeFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeFields.ImageIndex = 0;
            this.treeFields.ImageList = this.imageList1;
            this.treeFields.Location = new System.Drawing.Point(3, 3);
            this.treeFields.Name = "treeFields";
            this.treeFields.SelectedImageIndex = 0;
            this.treeFields.Size = new System.Drawing.Size(277, 243);
            this.treeFields.TabIndex = 0;
            this.treeFields.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeFields_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "table.ico");
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(label5, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnSetUnicode, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.btnSetAnsi, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.lblFieldDesc, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(286, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 8;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(278, 243);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // btnSetUnicode
            // 
            this.btnSetUnicode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.btnSetUnicode, 2);
            this.btnSetUnicode.Location = new System.Drawing.Point(3, 123);
            this.btnSetUnicode.Name = "btnSetUnicode";
            this.btnSetUnicode.Size = new System.Drawing.Size(272, 23);
            this.btnSetUnicode.TabIndex = 0;
            this.btnSetUnicode.Text = "Set Field As Unicode";
            this.btnSetUnicode.UseVisualStyleBackColor = true;
            this.btnSetUnicode.Click += new System.EventHandler(this.btnSetUnicode_Click);
            // 
            // btnSetAnsi
            // 
            this.btnSetAnsi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.btnSetAnsi, 2);
            this.btnSetAnsi.Location = new System.Drawing.Point(3, 153);
            this.btnSetAnsi.Name = "btnSetAnsi";
            this.btnSetAnsi.Size = new System.Drawing.Size(272, 23);
            this.btnSetAnsi.TabIndex = 1;
            this.btnSetAnsi.Text = "Set Field as Ansi Text";
            this.btnSetAnsi.UseVisualStyleBackColor = true;
            this.btnSetAnsi.Click += new System.EventHandler(this.btnSetAnsi_Click);
            // 
            // lblFieldDesc
            // 
            this.lblFieldDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFieldDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFieldDesc.Location = new System.Drawing.Point(83, 0);
            this.lblFieldDesc.Name = "lblFieldDesc";
            this.tableLayoutPanel3.SetRowSpan(this.lblFieldDesc, 4);
            this.lblFieldDesc.Size = new System.Drawing.Size(192, 120);
            this.lblFieldDesc.TabIndex = 2;
            this.lblFieldDesc.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tabOutput
            // 
            this.tabOutput.Controls.Add(this.tabPageOutput);
            this.tabOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabOutput.Location = new System.Drawing.Point(0, 0);
            this.tabOutput.Name = "tabOutput";
            this.tabOutput.SelectedIndex = 0;
            this.tabOutput.Size = new System.Drawing.Size(581, 122);
            this.tabOutput.TabIndex = 0;
            // 
            // tabPageOutput
            // 
            this.tabPageOutput.Controls.Add(this.textBox1);
            this.tabPageOutput.Location = new System.Drawing.Point(4, 22);
            this.tabPageOutput.Name = "tabPageOutput";
            this.tabPageOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOutput.Size = new System.Drawing.Size(573, 96);
            this.tabPageOutput.TabIndex = 0;
            this.tabPageOutput.Text = "Output";
            this.tabPageOutput.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(567, 90);
            this.textBox1.TabIndex = 0;
            // 
            // btnDoDamage
            // 
            this.btnDoDamage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.btnDoDamage, 2);
            this.btnDoDamage.Location = new System.Drawing.Point(3, 252);
            this.btnDoDamage.Name = "btnDoDamage";
            this.btnDoDamage.Size = new System.Drawing.Size(561, 23);
            this.btnDoDamage.TabIndex = 2;
            this.btnDoDamage.Text = "Apply Field Changes";
            this.btnDoDamage.UseVisualStyleBackColor = true;
            this.btnDoDamage.Click += new System.EventHandler(this.btnDoDamage_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 437);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabSetup.ResumeLayout(false);
            this.tabSetup.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabFields.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tabOutput.ResumeLayout(false);
            this.tabPageOutput.ResumeLayout(false);
            this.tabPageOutput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabOutput;
        private System.Windows.Forms.TabPage tabPageOutput;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSetup;
        private System.Windows.Forms.TabPage tabFields;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnTestDb;
        private System.Windows.Forms.FolderBrowserDialog modelBrowser;
        private System.Windows.Forms.TextBox txtFolderModel;
        private System.Windows.Forms.Button btnSelModel;
        private System.Windows.Forms.Button btnTestModel;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TreeView treeFields;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnSetUnicode;
        private System.Windows.Forms.Button btnSetAnsi;
        private System.Windows.Forms.Label lblFieldDesc;
        private System.Windows.Forms.Button btnDoDamage;
    }
}

