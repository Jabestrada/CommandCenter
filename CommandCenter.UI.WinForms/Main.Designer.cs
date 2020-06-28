namespace CommandCenter.UI.WinForms {
    partial class Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.txtConfigFile = new System.Windows.Forms.TextBox();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.commandsList = new System.Windows.Forms.TreeView();
            this.commandsListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.uncheckAllButThisMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAllButThisMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.checkAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.commandParametersList = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusWindow = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnBrowseConfig = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCommands = new System.Windows.Forms.TabPage();
            this.tabTokens = new System.Windows.Forms.TabPage();
            this.tokensList = new System.Windows.Forms.DataGridView();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPreflight = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentConfigFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configFileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openFileInNotepadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openContainingFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandsListContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.commandParametersList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabCommands.SuspendLayout();
            this.tabTokens.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tokensList)).BeginInit();
            this.mainMenu.SuspendLayout();
            this.configFileContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtConfigFile
            // 
            this.txtConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConfigFile.ContextMenuStrip = this.configFileContextMenu;
            this.txtConfigFile.Location = new System.Drawing.Point(33, 55);
            this.txtConfigFile.Name = "txtConfigFile";
            this.txtConfigFile.Size = new System.Drawing.Size(908, 20);
            this.txtConfigFile.TabIndex = 1;
            this.txtConfigFile.TextChanged += new System.EventHandler(this.txtConfigFile_TextChanged);
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadConfig.Location = new System.Drawing.Point(972, 52);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(97, 26);
            this.btnLoadConfig.TabIndex = 2;
            this.btnLoadConfig.Text = "Load";
            this.btnLoadConfig.UseVisualStyleBackColor = true;
            this.btnLoadConfig.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // commandsList
            // 
            this.commandsList.CheckBoxes = true;
            this.commandsList.ContextMenuStrip = this.commandsListContextMenu;
            this.commandsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandsList.FullRowSelect = true;
            this.commandsList.HideSelection = false;
            this.commandsList.Location = new System.Drawing.Point(0, 0);
            this.commandsList.Name = "commandsList";
            this.commandsList.ShowRootLines = false;
            this.commandsList.Size = new System.Drawing.Size(547, 267);
            this.commandsList.TabIndex = 3;
            this.commandsList.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.commandsList_AfterCheck);
            this.commandsList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.commandsList_AfterSelect);
            this.commandsList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.commandsList_MouseDown);
            // 
            // commandsListContextMenu
            // 
            this.commandsListContextMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.commandsListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uncheckAllButThisMenuItem,
            this.checkAllButThisMenuItem,
            this.toolStripSeparator1,
            this.checkAllMenuItem,
            this.uncheckAllMenuItem});
            this.commandsListContextMenu.Name = "commandsListContextMenu";
            this.commandsListContextMenu.Size = new System.Drawing.Size(166, 98);
            // 
            // uncheckAllButThisMenuItem
            // 
            this.uncheckAllButThisMenuItem.Name = "uncheckAllButThisMenuItem";
            this.uncheckAllButThisMenuItem.Size = new System.Drawing.Size(165, 22);
            this.uncheckAllButThisMenuItem.Text = "Check this only";
            this.uncheckAllButThisMenuItem.Click += new System.EventHandler(this.uncheckAllButThisMenuItem_Click);
            // 
            // checkAllButThisMenuItem
            // 
            this.checkAllButThisMenuItem.Name = "checkAllButThisMenuItem";
            this.checkAllButThisMenuItem.Size = new System.Drawing.Size(165, 22);
            this.checkAllButThisMenuItem.Text = "Check all but this";
            this.checkAllButThisMenuItem.Click += new System.EventHandler(this.checkAllButThisMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(162, 6);
            // 
            // checkAllMenuItem
            // 
            this.checkAllMenuItem.Name = "checkAllMenuItem";
            this.checkAllMenuItem.Size = new System.Drawing.Size(165, 22);
            this.checkAllMenuItem.Text = "Check all";
            this.checkAllMenuItem.Click += new System.EventHandler(this.checkAllMenuItem_Click);
            // 
            // uncheckAllMenuItem
            // 
            this.uncheckAllMenuItem.Name = "uncheckAllMenuItem";
            this.uncheckAllMenuItem.Size = new System.Drawing.Size(165, 22);
            this.uncheckAllMenuItem.Text = "Uncheck all";
            this.uncheckAllMenuItem.Click += new System.EventHandler(this.uncheckAllMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.commandsList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.commandParametersList);
            this.splitContainer1.Size = new System.Drawing.Size(1035, 267);
            this.splitContainer1.SplitterDistance = 547;
            this.splitContainer1.TabIndex = 4;
            // 
            // commandParametersList
            // 
            this.commandParametersList.AllowUserToAddRows = false;
            this.commandParametersList.AllowUserToDeleteRows = false;
            this.commandParametersList.AllowUserToResizeRows = false;
            this.commandParametersList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.commandParametersList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.commandParametersList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.commandParametersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandParametersList.Location = new System.Drawing.Point(0, 0);
            this.commandParametersList.Name = "commandParametersList";
            this.commandParametersList.RowHeadersVisible = false;
            this.commandParametersList.RowHeadersWidth = 102;
            this.commandParametersList.RowTemplate.Height = 40;
            this.commandParametersList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.commandParametersList.Size = new System.Drawing.Size(484, 267);
            this.commandParametersList.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Key";
            this.Column1.HeaderText = "Parameter";
            this.Column1.MinimumWidth = 12;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Value";
            this.Column2.HeaderText = "Value";
            this.Column2.MinimumWidth = 12;
            this.Column2.Name = "Column2";
            // 
            // statusWindow
            // 
            this.statusWindow.BackColor = System.Drawing.SystemColors.Info;
            this.statusWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusWindow.Location = new System.Drawing.Point(0, 0);
            this.statusWindow.Multiline = true;
            this.statusWindow.Name = "statusWindow";
            this.statusWindow.ReadOnly = true;
            this.statusWindow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.statusWindow.Size = new System.Drawing.Size(1035, 177);
            this.statusWindow.TabIndex = 5;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.statusWindow);
            this.splitContainer2.Size = new System.Drawing.Size(1035, 448);
            this.splitContainer2.SplitterDistance = 267;
            this.splitContainer2.TabIndex = 6;
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Enabled = false;
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(900, 570);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(175, 39);
            this.btnRun.TabIndex = 7;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnBrowseConfig
            // 
            this.btnBrowseConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseConfig.Location = new System.Drawing.Point(942, 52);
            this.btnBrowseConfig.Name = "btnBrowseConfig";
            this.btnBrowseConfig.Size = new System.Drawing.Size(28, 26);
            this.btnBrowseConfig.TabIndex = 8;
            this.btnBrowseConfig.Text = "...";
            this.toolTip1.SetToolTip(this.btnBrowseConfig, "Browse for config file");
            this.btnBrowseConfig.UseVisualStyleBackColor = true;
            this.btnBrowseConfig.Click += new System.EventHandler(this.btnBrowseConfig_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabCommands);
            this.tabControl1.Controls.Add(this.tabTokens);
            this.tabControl1.Location = new System.Drawing.Point(26, 83);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1049, 480);
            this.tabControl1.TabIndex = 9;
            // 
            // tabCommands
            // 
            this.tabCommands.Controls.Add(this.splitContainer2);
            this.tabCommands.Location = new System.Drawing.Point(4, 22);
            this.tabCommands.Name = "tabCommands";
            this.tabCommands.Padding = new System.Windows.Forms.Padding(3);
            this.tabCommands.Size = new System.Drawing.Size(1041, 454);
            this.tabCommands.TabIndex = 0;
            this.tabCommands.Text = "Commands";
            this.tabCommands.UseVisualStyleBackColor = true;
            // 
            // tabTokens
            // 
            this.tabTokens.Controls.Add(this.tokensList);
            this.tabTokens.Location = new System.Drawing.Point(4, 22);
            this.tabTokens.Name = "tabTokens";
            this.tabTokens.Padding = new System.Windows.Forms.Padding(3);
            this.tabTokens.Size = new System.Drawing.Size(1041, 454);
            this.tabTokens.TabIndex = 1;
            this.tabTokens.Text = "Tokens";
            this.tabTokens.UseVisualStyleBackColor = true;
            // 
            // tokensList
            // 
            this.tokensList.AllowUserToAddRows = false;
            this.tokensList.AllowUserToDeleteRows = false;
            this.tokensList.AllowUserToResizeRows = false;
            this.tokensList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tokensList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tokensList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Key,
            this.Value});
            this.tokensList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tokensList.Location = new System.Drawing.Point(3, 3);
            this.tokensList.Name = "tokensList";
            this.tokensList.ReadOnly = true;
            this.tokensList.RowHeadersVisible = false;
            this.tokensList.RowHeadersWidth = 102;
            this.tokensList.RowTemplate.Height = 40;
            this.tokensList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tokensList.Size = new System.Drawing.Size(1035, 448);
            this.tokensList.TabIndex = 0;
            // 
            // Key
            // 
            this.Key.DataPropertyName = "Key";
            this.Key.HeaderText = "Token";
            this.Key.MinimumWidth = 12;
            this.Key.Name = "Key";
            this.Key.ReadOnly = true;
            // 
            // Value
            // 
            this.Value.DataPropertyName = "Value";
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 12;
            this.Value.Name = "Value";
            this.Value.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Config File";
            // 
            // btnPreflight
            // 
            this.btnPreflight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreflight.Enabled = false;
            this.btnPreflight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreflight.Location = new System.Drawing.Point(719, 570);
            this.btnPreflight.Name = "btnPreflight";
            this.btnPreflight.Size = new System.Drawing.Size(175, 39);
            this.btnPreflight.TabIndex = 11;
            this.btnPreflight.Text = "Pre-flight Check";
            this.btnPreflight.UseVisualStyleBackColor = true;
            this.btnPreflight.Click += new System.EventHandler(this.preFlight_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(34, 581);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(35, 13);
            this.statusLabel.TabIndex = 12;
            this.statusLabel.Text = "label2";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(538, 570);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(175, 39);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1101, 24);
            this.mainMenu.TabIndex = 14;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openLogFileToolStripMenuItem,
            this.recentConfigFilesToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openLogFileToolStripMenuItem
            // 
            this.openLogFileToolStripMenuItem.Name = "openLogFileToolStripMenuItem";
            this.openLogFileToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.openLogFileToolStripMenuItem.Text = "&Open configuration file...";
            this.openLogFileToolStripMenuItem.Click += new System.EventHandler(this.openLogFileToolStripMenuItem_Click);
            // 
            // recentConfigFilesToolStripMenuItem
            // 
            this.recentConfigFilesToolStripMenuItem.Name = "recentConfigFilesToolStripMenuItem";
            this.recentConfigFilesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.recentConfigFilesToolStripMenuItem.Text = "&Recent configuration files";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // configFileContextMenu
            // 
            this.configFileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openFileInNotepadToolStripMenuItem,
            this.openContainingFolderToolStripMenuItem});
            this.configFileContextMenu.Name = "configFileContextMenu";
            this.configFileContextMenu.Size = new System.Drawing.Size(201, 142);
            this.configFileContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.configFileContextMenu_Opening);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(197, 6);
            // 
            // openFileInNotepadToolStripMenuItem
            // 
            this.openFileInNotepadToolStripMenuItem.Name = "openFileInNotepadToolStripMenuItem";
            this.openFileInNotepadToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.openFileInNotepadToolStripMenuItem.Text = "Open file in Notepad++";
            this.openFileInNotepadToolStripMenuItem.Click += new System.EventHandler(this.openFileInNotepadToolStripMenuItem_Click);
            // 
            // openContainingFolderToolStripMenuItem
            // 
            this.openContainingFolderToolStripMenuItem.Name = "openContainingFolderToolStripMenuItem";
            this.openContainingFolderToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.openContainingFolderToolStripMenuItem.Text = "Open containing folder";
            this.openContainingFolderToolStripMenuItem.Click += new System.EventHandler(this.openContainingFolderToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1101, 618);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.btnPreflight);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnBrowseConfig);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnLoadConfig);
            this.Controls.Add(this.txtConfigFile);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Command Center";
            this.commandsListContextMenu.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.commandParametersList)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabCommands.ResumeLayout(false);
            this.tabTokens.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tokensList)).EndInit();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.configFileContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtConfigFile;
        private System.Windows.Forms.Button btnLoadConfig;
        private System.Windows.Forms.TreeView commandsList;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView commandParametersList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.TextBox statusWindow;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnBrowseConfig;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip commandsListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem checkAllButThisMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllButThisMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem checkAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabCommands;
        private System.Windows.Forms.TabPage tabTokens;
        private System.Windows.Forms.DataGridView tokensList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPreflight;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentConfigFilesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip configFileContextMenu;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openFileInNotepadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openContainingFolderToolStripMenuItem;
    }
}

