﻿using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CommandCenter.UI.WinForms {
    public partial class Main : Form {

        private CommandsOrchestratorWinForms _orchestrator;

        private List<CommandConfiguration> _loadedCommandConfigurations;
        public Main() {
            InitializeComponent();
            _orchestrator = new CommandsOrchestratorWinForms(_reportReceiver);
        }

        delegate void SetTextCallback(string message);
        private void _reportReceiver(BaseCommand command, CommandReportArgs e) {
            appendStatusText($"{e.ReportType}: {e.Message}");
        }

        private void appendStatusText(string message) {
            if (this.statusWindow.InvokeRequired) {
                SetTextCallback cb = new SetTextCallback(appendStatusText);
                this.Invoke(cb, new object[] { message });
            }
            else {
                statusWindow.AppendText(message);
                statusWindow.AppendText(Environment.NewLine);
            }
        }

        private void btnLoadConfig_Click(object sender, EventArgs e) {
            loadCommands();
        }

        private void loadCommands() {
            if (string.IsNullOrWhiteSpace(txtConfigFile.Text)) return;

            if (!File.Exists(txtConfigFile.Text)) {
                displayError($"Config file {txtConfigFile.Text} does not exist");
                return;
            }
            displayCommands();
        }

        private void displayCommands() {
            try {
                _loadedCommandConfigurations = _orchestrator.GetCommands(txtConfigFile.Text);
            }
            catch {
                displayError($"File {txtConfigFile.Text} is not a valid Command Center configuration file");
                return;
            }

            commandsList.Nodes.Clear();
            foreach (var command in _loadedCommandConfigurations) {
                var commandName = new FullTypeNameEntry(command.TypeActivationName).TypeName;
                var commandDisplayText = commandName.Substring(commandName.LastIndexOf('.') + 1);
                if (!string.IsNullOrWhiteSpace(command.ShortDescription)) {
                    commandDisplayText += " - " + command.ShortDescription;
                }
                var cmdNode = commandsList.Nodes.Add(commandDisplayText);
                cmdNode.Checked = true;
                cmdNode.Tag = command;
            }
        }

        private void displayError(string message) {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void commandsList_AfterSelect(object sender, TreeViewEventArgs e) {
            var selectedCommand = e.Node.Tag as CommandConfiguration;
            if (selectedCommand != null) {
                commandParametersList.DataSource = selectedCommand.ConstructorArgs.Select(a => new { Key = a.Key, Value = a.Value }).ToList();
            }
        }

        private void btnRun_Click(object sender, EventArgs e) {
            runCommands();
        }

        private void runCommands() {
            var commandConfigList = new List<CommandConfiguration>();
            TreeNodeCollection nodes = commandsList.Nodes;
            foreach (TreeNode commandNode in nodes) {
                if (commandNode.Checked) {
                    commandConfigList.Add(commandNode.Tag as CommandConfiguration);
                }
            }
            if (!commandConfigList.Any()) {
                MessageBox.Show("At least 1 command has to be checked", "No command selected", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            statusWindow.Clear();
            ThreadStart starter = new ThreadStart(() => _orchestrator.Run(commandConfigList));
            starter += () => {
                appendStatusText(Environment.NewLine);
                var status = _orchestrator.DidCommandsSucceed ? "SUCCEEDED" : "FAILED";
                appendStatusText($"Commands {status}");
            };
            var thread = new Thread(starter);
            thread.Start();
        }

        private void btnBrowseConfig_Click(object sender, EventArgs e) {
            browseForConfigFile();
        }

        private void browseForConfigFile() {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "*|*.config";
            var response = fileDialog.ShowDialog();
            if (response == DialogResult.OK) {
                txtConfigFile.Text = fileDialog.FileName;
            }
        }

        private void txtConfigFile_TextChanged(object sender, EventArgs e) {
            btnLoadConfig.Enabled = txtConfigFile.Text.Length > 0;
        }

        private void commandsList_AfterCheck(object sender, TreeViewEventArgs e) {
            TreeNodeCollection nodes = commandsList.Nodes;
            foreach (TreeNode commandNode in nodes) {
                if (commandNode.Checked) {
                    btnRun.Enabled = true;
                    return;
                }
            }
            btnRun.Enabled = false;
        }
    }
}