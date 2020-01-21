using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace CommandCenter.UI.WinForms {
    public partial class Main : Form {

        private CommandsControllerWinForms _controller;

        private List<CommandConfiguration> _loadedCommandConfigurations;
        private List<Token> _loadedTokens;
        private string _lastLoadedConfig = string.Empty;
        public Main() {
            InitializeComponent();
            _controller = new CommandsControllerWinForms(_reportReceiver);
            var defaultConfig = Path.Combine(Application.StartupPath, "CommandCenter.config");
            if (string.IsNullOrWhiteSpace(txtConfigFile.Text) && File.Exists(defaultConfig)) {
                txtConfigFile.Text = defaultConfig;
            }
            loadCommands();
            if (IsAnAdministrator()) {
                this.Text = this.Text + " - launched as Admin";
            }
            else {
                this.Text = this.Text + " - not launched as Admin";
            }
        }

        bool IsAnAdministrator() {
            try {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch {
                return false;
            }
        }
        private void _reportReceiver(BaseCommand command, CommandReportArgs e) {
            appendStatusText($"{e.ReportType}: {e.Message}");
        }

        delegate void SetTextCallback(string message);

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

        delegate void EnableRunButtonCallback();
        private void enableRunButton() {
            if (this.btnRun.InvokeRequired) {
                var cb = new EnableRunButtonCallback(enableRunButton);
                this.Invoke(cb);
            }
            else {
                btnRun.Enabled = true;
            }
        }

        private void btnLoadConfig_Click(object sender, EventArgs e) {
            loadCommands();
        }

        private void loadCommands() {
            if (string.IsNullOrWhiteSpace(txtConfigFile.Text)) return;
            if (!File.Exists(txtConfigFile.Text)) {
                displayError($"Config file {txtConfigFile.Text} does not exist");
                if (!string.IsNullOrWhiteSpace(_lastLoadedConfig)) {
                    txtConfigFile.Text = _lastLoadedConfig;
                }
                return;
            }

            resetForm();
            displayCommands();
        }

        private void resetForm() {
            commandsList.Nodes.Clear();
            commandParametersList.DataSource = null;
            tokensList.DataSource = null;
            statusWindow.Text = string.Empty;
        }

        private void displayCommands() {
            try {
                _loadedCommandConfigurations = _controller.GetCommands(txtConfigFile.Text);
            }
            catch {
                displayError($"File {txtConfigFile.Text} is not a valid Command Center configuration file");
                _lastLoadedConfig = string.Empty;
                return;
            }

            _loadedTokens = _controller.GetTokens(txtConfigFile.Text);
            tokensList.DataSource = _loadedTokens.Select(a => new { a.Key, a.Value }).ToList();

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
            _lastLoadedConfig = txtConfigFile.Text;
        }

        private void displayError(string message) {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void commandsList_AfterSelect(object sender, TreeViewEventArgs e) {
            var selectedCommand = e.Node.Tag as CommandConfiguration;
            if (selectedCommand != null) {
                commandParametersList.DataSource = selectedCommand.ConstructorArgs.Select(a => new { a.Key, a.Value }).ToList();
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
                return;
            }

            btnRun.Enabled = false;
            statusWindow.Clear();
            ThreadStart starter = new ThreadStart(() => _controller.Run(commandConfigList));
            starter += () => {
                appendStatusText(Environment.NewLine);
                var status = _controller.DidCommandsSucceed ? "SUCCEEDED" : "FAILED";
                appendStatusText($"Commands {status}");
                enableRunButton();
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
                loadCommands();
            }
        }

        private void txtConfigFile_TextChanged(object sender, EventArgs e) {
            btnLoadConfig.Enabled = txtConfigFile.Text.Length > 0;
        }

        private void commandsList_AfterCheck(object sender, TreeViewEventArgs e) {
            foreach (TreeNode commandNode in commandsList.Nodes) {
                if (commandNode.Checked) {
                    btnRun.Enabled = true;
                    return;
                }
            }
            btnRun.Enabled = false;
        }

        private void commandsList_MouseDown(object sender, MouseEventArgs e) {
            commandsList.SelectedNode = commandsList.GetNodeAt(e.X, e.Y);
        }

        private void checkAllButThisMenuItem_Click(object sender, EventArgs e) {
            checkAll(true, commandsList.SelectedNode, false);
        }

        private void uncheckAllButThisMenuItem_Click(object sender, EventArgs e) {
            checkAll(false, commandsList.SelectedNode, true);
        }

        private void checkAllMenuItem_Click(object sender, EventArgs e) {
            checkAll(true);
        }

        private void uncheckAllMenuItem_Click(object sender, EventArgs e) {
            checkAll(false);
        }

        private void checkAll(bool isChecked, TreeNode excludedNode = null, bool excludedNodeSetting = false) {
            if (excludedNode != null) {
                excludedNode.Checked = excludedNodeSetting;
            }
            foreach (TreeNode node in commandsList.Nodes) {
                if (node != excludedNode) { 
                   node.Checked = isChecked;
                }
            }
        }
    }
}
