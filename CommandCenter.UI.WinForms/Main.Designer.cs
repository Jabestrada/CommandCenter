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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.commandParametersList = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusWindow = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnBrowseConfig = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.commandParametersList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtConfigFile
            // 
            this.txtConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConfigFile.Location = new System.Drawing.Point(26, 37);
            this.txtConfigFile.Name = "txtConfigFile";
            this.txtConfigFile.Size = new System.Drawing.Size(1699, 38);
            this.txtConfigFile.TabIndex = 1;
            this.txtConfigFile.Text = "C:\\JABE\\JABELabs\\CommandCenter\\CommandCenter.UI.WinForms\\CommandCenter.config";
            this.txtConfigFile.TextChanged += new System.EventHandler(this.txtConfigFile_TextChanged);
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadConfig.Location = new System.Drawing.Point(1837, 27);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(238, 57);
            this.btnLoadConfig.TabIndex = 2;
            this.btnLoadConfig.Text = "Load";
            this.btnLoadConfig.UseVisualStyleBackColor = true;
            this.btnLoadConfig.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // commandsList
            // 
            this.commandsList.CheckBoxes = true;
            this.commandsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandsList.HideSelection = false;
            this.commandsList.Location = new System.Drawing.Point(0, 0);
            this.commandsList.Name = "commandsList";
            this.commandsList.Size = new System.Drawing.Size(1093, 699);
            this.commandsList.TabIndex = 3;
            this.commandsList.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.commandsList_AfterCheck);
            this.commandsList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.commandsList_AfterSelect);
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
            this.splitContainer1.Size = new System.Drawing.Size(2063, 699);
            this.splitContainer1.SplitterDistance = 1093;
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
            this.commandParametersList.Size = new System.Drawing.Size(966, 699);
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
            this.statusWindow.Size = new System.Drawing.Size(2063, 469);
            this.statusWindow.TabIndex = 5;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(12, 113);
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
            this.splitContainer2.Size = new System.Drawing.Size(2063, 1172);
            this.splitContainer2.SplitterDistance = 699;
            this.splitContainer2.TabIndex = 6;
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(1743, 1342);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(318, 86);
            this.btnRun.TabIndex = 7;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnBrowseConfig
            // 
            this.btnBrowseConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseConfig.Location = new System.Drawing.Point(1743, 27);
            this.btnBrowseConfig.Name = "btnBrowseConfig";
            this.btnBrowseConfig.Size = new System.Drawing.Size(75, 57);
            this.btnBrowseConfig.TabIndex = 8;
            this.btnBrowseConfig.Text = "...";
            this.toolTip1.SetToolTip(this.btnBrowseConfig, "Browse for config file");
            this.btnBrowseConfig.UseVisualStyleBackColor = true;
            this.btnBrowseConfig.Click += new System.EventHandler(this.btnBrowseConfig_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2087, 1493);
            this.Controls.Add(this.btnBrowseConfig);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.btnLoadConfig);
            this.Controls.Add(this.txtConfigFile);
            this.Name = "Main";
            this.Text = "Command Center";
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
    }
}

