using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Configuration;
using CommandCenter.Infrastructure.Orchestration;
using CommandCenter.UI.WinForms.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace CommandCenter.UI.WinForms {
    public partial class Main : Form {
        private bool _wasCommandCancelled = false;
        private CommandsControllerWinForms _controller;
        private List<CommandConfiguration> _loadedCommandConfigurations;
        private List<CommandConfiguration> _selectedCommandConfigurations;

        private List<Token> _loadedTokens;
        private string _lastLoadedConfig = string.Empty;
        private AppState _appState = new AppState();

        public Main() {
            InitializeComponent();
            _controller = new CommandsControllerWinForms(_reportReceiver);
            //var defaultConfig = Path.Combine(Application.StartupPath, "CommandCenter.config");
            //if (string.IsNullOrWhiteSpace(txtConfigFile.Text) && File.Exists(defaultConfig)) {
            //    txtConfigFile.Text = defaultConfig;
            //}
            //loadCommands();
            if (IsRunByAdmin()) {
                this.Text = this.Text + " - launched as Admin";
            }
            else {
                this.Text = this.Text + " - not launched as Admin";
            }
            cancelButton.Location = btnRun.Location;

            loadAppState();

            FormMode = FormModeEnum.Ready;
        }

        #region FormMode and CancelStatus form props
        private enum FormModeEnum {
            Ready,
            RunningCommands,
            RunningPreflight
        }

        private FormModeEnum _formMode;
        private FormModeEnum FormMode {
            get {
                return _formMode;
            }
            set {
                _formMode = value;
                didChangeFormMode(value);
            }
        }

        private void didChangeFormMode(FormModeEnum value) {
            var isFormReady = value == FormModeEnum.Ready;
            var selectedCommandCount = countSelectedCommands();

            setEnabled(txtConfigFile, isFormReady);
            setEnabled(btnBrowseConfig, isFormReady);
            setEnabled(btnLoadConfig, isFormReady);
            setEnabled(commandsList, isFormReady);
            setEnabled(btnRun, isFormReady && selectedCommandCount > 0);
            setVisible(btnRun, value == FormModeEnum.Ready || value == FormModeEnum.RunningPreflight);
            setEnabled(btnPreflight, isFormReady && selectedCommandCount > 0);
            setEnabled(btnLoadConfig, txtConfigFile.Text.Trim().Length > 0);
            setVisible(cancelButton, value == FormModeEnum.RunningCommands);

            refreshStatusText(selectedCommandCount);
        }

        private enum CancellationStatusEnum {
            None,
            Pending,
            Done
        }
        private CancellationStatusEnum _cancelStatus;
        private CancellationStatusEnum CancelStatus {
            get {
                return _cancelStatus;
            }
            set {
                _cancelStatus = value;
                didChangeCancelStatus(value);
            }
        }

        private void didChangeCancelStatus(CancellationStatusEnum value) {
            switch (value) {
                case CancellationStatusEnum.Pending:
                    setText(cancelButton, "Cancelling...");
                    setEnabled(cancelButton, false);
                    break;
                case CancellationStatusEnum.None:
                case CancellationStatusEnum.Done:
                default:
                    setText(cancelButton, "Cancel");
                    setEnabled(cancelButton, true);
                    break;
            }
            _wasCommandCancelled = value == CancellationStatusEnum.Pending;
        }
        #endregion

        #region Control thread-safe delegates
        delegate void EnableControlCallback(Control control, bool enabled);
        private void setEnabled(Control control, bool enabled) {
            if (control.InvokeRequired) {
                var cb = new EnableControlCallback(setEnabled);
                Invoke(cb, control, enabled);
            }
            else {
                control.Enabled = enabled;
            }
        }

        delegate void SetVisibleCallback(Control control, bool isVisible);
        private void setVisible(Control control, bool isVisible) {
            if (control.InvokeRequired) {
                var cb = new EnableControlCallback(setVisible);
                Invoke(cb, control, isVisible);
            }
            else {
                control.Visible = isVisible;
            }
        }

        delegate void AppendTextCallback(string message);
        private void appendStatusText(string message) {
            if (this.statusWindow.InvokeRequired) {
                AppendTextCallback cb = new AppendTextCallback(appendStatusText);
                this.Invoke(cb, new object[] { message });
            }
            else {
                statusWindow.AppendText(message);
                statusWindow.AppendText(Environment.NewLine);
            }
        }

        delegate void SetTextCallback(Control control, string message);
        private void setText(Control control, string text) {
            if (control.InvokeRequired) {
                SetTextCallback cb = new SetTextCallback(setText);
                Invoke(cb, new object[] { control, text });
            }
            else {
                control.Text = text;
            }
        }
        #endregion

        #region Browsing/loading commands from config
        private void openLogFileToolStripMenuItem_Click(object sender, EventArgs e) {
            browseForConfigFile();
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
            var hasConfigFileInput = txtConfigFile.Text.Trim().Length > 0;
            btnLoadConfig.Enabled = hasConfigFileInput;
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

        private void displayCommands() {
            var configFile = txtConfigFile.Text.Trim();
            try {
                _loadedCommandConfigurations = _controller.GetCommands(configFile);
            }
            catch {
                displayError($"File {configFile} is not a valid Command Center configuration file");
                _lastLoadedConfig = string.Empty;
                return;
            }

            _loadedTokens = _controller.GetTokens(configFile);
            tokensList.DataSource = _loadedTokens.Select(a => new { a.Key, a.Value }).ToList();

            foreach (var command in _loadedCommandConfigurations) {
                commandsList.Nodes.Add(createTreeNodeFromCommand(command));
            }

            _lastLoadedConfig = configFile;
            pushMRUItem(configFile);
            saveAppState();
        }
        #endregion

        #region AppState-related
        private void saveAppState() {
            var serializer = new DataContractSerializer(typeof(AppState));
            string xmlString;
            using (var sw = new StringWriter())
            using (var writer = new XmlTextWriter(sw)) {
                writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                serializer.WriteObject(writer, _appState);
                writer.Flush();
                xmlString = sw.ToString();
            }

            string appStateFile = getAppStateFilename();
            using (StreamWriter sw = new StreamWriter(appStateFile)) {
                sw.Write(xmlString);
                sw.Flush();
                sw.Close();
            }
        }

        private void loadAppState() {
            string appStateFile = getAppStateFilename();
            if (!File.Exists(appStateFile)) {
                recentConfigFilesToolStripMenuItem.Enabled = false;
                return;
            }
            using (StreamReader sw = new StreamReader(appStateFile)) {
                var reader = new XmlTextReader(sw);
                var deserializer = new DataContractSerializer(typeof(AppState));
                var result = deserializer.ReadObject(reader);
                _appState = (AppState)result;

            }
            foreach (var mruItem in _appState.MRUConfigList.Items) {
                var mruMenuItem = createMruMenuItem(mruItem);
                recentConfigFilesToolStripMenuItem.DropDownItems.Add(mruMenuItem);
            }
            recentConfigFilesToolStripMenuItem.Enabled = _appState.MRUConfigList.Items.Count > 0;
        }

        private string getAppStateFilename() {
            var appDir = Path.GetDirectoryName(Application.ExecutablePath);
            var datFile = $"{Path.GetFileNameWithoutExtension(Application.ExecutablePath)}.dat";
            return Path.Combine(appDir, datFile);
        }


        private void pushMRUItem(string item) {
            _appState.MRUConfigList.AddItem(item);

            recentConfigFilesToolStripMenuItem.DropDownItems.Clear();
            foreach (var mruItem in _appState.MRUConfigList.Items) {
                var menuItem = createMruMenuItem(mruItem);
                recentConfigFilesToolStripMenuItem.DropDownItems.Add(menuItem);
            }

            if (!recentConfigFilesToolStripMenuItem.Enabled) recentConfigFilesToolStripMenuItem.Enabled = true;
        }

        private ToolStripMenuItem createMruMenuItem(KeyValuePair<int, string> i) {
            var menuItem = new ToolStripMenuItem($"{i.Key + 1} {i.Value}");
            menuItem.Tag = i.Value;
            menuItem.Click += mruItem_Click;
            return menuItem;
        }

        private void mruItem_Click(object sender, EventArgs e) {
            var mruItem = (ToolStripMenuItem)sender;
            txtConfigFile.Text = mruItem.Tag.ToString();
            loadCommands();
        }
        #endregion

        #region Command run/preflight/cancellation
        private void btnRun_Click(object sender, EventArgs e) {
            try {
                runCommands();
            }
            catch (Exception exc) {
                MessageBox.Show($"Error running command(s):{Environment.NewLine}{exc.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void preFlight_Click(object sender, EventArgs e) {
            if (!tryGetSelectedCommands()) return;

            statusWindow.Clear();
            ThreadStart starter = new ThreadStart(() => {
                FormMode = FormModeEnum.RunningPreflight;
                try {
                    _controller.RunPreflight(_selectedCommandConfigurations);
                }
                catch (Exception exc) {
                    MessageBox.Show($"Error occurred while attempting to run Preflight Check: {exc.Message}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
            starter += () => {
                writePreflightSummary();
                FormMode = FormModeEnum.Ready;
            };

            var thread = new Thread(starter);
            thread.Start();

            //FormMode = FormModeEnum.RunningPreflight;

            //FormMode = FormModeEnum.Ready;
        }
        private void cancelButton_Click(object sender, EventArgs e) {
            if (MessageBox.Show($"Are you sure you want to cancel?{Environment.NewLine}Cancelling will undo the commands in this run.",
                                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) {
                return;
            }

            _controller.Cancel();
            CancelStatus = CancellationStatusEnum.Pending;
        }
        private void runCommands() {
            if (!tryGetSelectedCommands()) return;

            if (!confirmContinueWithUndoables(_selectedCommandConfigurations)) {
                return;
            }

            statusWindow.Clear();
            ThreadStart starter = new ThreadStart(() => {
                FormMode = FormModeEnum.RunningCommands;
                CancelStatus = CancellationStatusEnum.None;

                _controller.Run(_selectedCommandConfigurations);
            });
            starter += () => {
                writeSummary();
                FormMode = FormModeEnum.Ready;
                CancelStatus = CancellationStatusEnum.None;
            };
            var thread = new Thread(starter);
            thread.Start();
        }
        private bool confirmContinueWithUndoables(List<CommandConfiguration> commandConfigList) {
            var undoableCommands = _controller.GetUndoableCommmands(commandConfigList);
            if (!undoableCommands.Any()) return true;

            Func<BaseCommand, string> getTypeName = c => {
                var fullTypeName = c.GetType().ToString();
                return fullTypeName.Substring(fullTypeName.LastIndexOf('.') + 1);
            };
            var commandList = string.Join(Environment.NewLine, undoableCommands.Select(c => $"* {getTypeName(c)} - {c.ShortDescription}").ToArray());
            var message = $"The following selected command(s) cannot be undone:{Environment.NewLine}{commandList}{Environment.NewLine}{Environment.NewLine}Continue?";
            return MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
        private List<CommandConfiguration> getSelectedCommands() {
            var commandConfigList = new List<CommandConfiguration>();
            TreeNodeCollection nodes = commandsList.Nodes;
            foreach (TreeNode commandNode in nodes) {
                if (commandNode.Checked) {
                    commandConfigList.Add(commandNode.Tag as CommandConfiguration);
                }
            }
            return commandConfigList;
        }
        #endregion

        #region Command status-reporting
        private void _reportReceiver(BaseCommand command, CommandReportArgs e) {
            switch (e.ReportType) {
                case ReportType.Progress:
                    appendStatusText($"{command.ShortName} => {e.Message}");
                    break;
                case ReportType.RunningCommandStatistics:
                    setText(statusLabel, e.Message);
                    break;
                default:
                    appendStatusText($"{command.ShortName}::{e.ReportType} => {e.Message}");
                    break;
            }
        }

        private void writeSummary() {
            appendStatusText(Environment.NewLine);
            appendStatusText("================== SUMMARY ==================");
            var finalReports = _controller.Reports.Where(r => r.ReportType.IsDoneTaskReport());
            foreach (var finalReport in finalReports) {
                appendStatusText($"{finalReport.Reporter.ShortDescription} ({finalReport.Reporter.ShortName}): {finalReport.Message}");
            }
            var status = _controller.DidCommandsSucceed ? "SUCCEEDED" : "FAILED";
            appendStatusText($"FINAL RESULT => Commands {status}");
            if (_wasCommandCancelled) {
                appendStatusText($"User cancelled command execution");
            }
            var elapsed = _controller.LastRunElapsedTime.ToString(@"hh\:mm\:ss");
            appendStatusText($"Time elapsed: {elapsed}");
        }

        private void writePreflightSummary() {
            appendStatusText(Environment.NewLine);
            appendStatusText("================== PRE-FLIGHT CHECK SUMMARY ==================");

            var finalReports = _controller.Reports.Where(r => r.ReportType.IsDonePreflightTaskReport());
            foreach (var finalReport in finalReports) {
                appendStatusText($"{finalReport.Reporter.ShortDescription} ({finalReport.Reporter.ShortName}): {finalReport.Message}");
            }
            var status = _controller.DidCommandsSucceed ? "SUCCEEDED" : "FAILED";
            appendStatusText($"FINAL PRE-FLIGHT CHECK RESULT => {status}");
            var elapsed = _controller.LastRunElapsedTime.ToString(@"hh\:mm\:ss");
            appendStatusText($"Time elapsed: {elapsed}");
        }
        private void refreshStatusText(int selectedCommandCount) {
            setText(statusLabel, $"{selectedCommandCount} command(s) selected");
        }

        #endregion

        #region commandsList Treeview events/helpers

        private int countSelectedCommands() {
            int ctr = 0;
            foreach (TreeNode commandNode in commandsList.Nodes) {
                if (commandNode.Checked) {
                    ctr++;
                }
            }
            return ctr;
        }

        private bool tryGetSelectedCommands() {
            _selectedCommandConfigurations = getSelectedCommands();
            if (!_selectedCommandConfigurations.Any()) {
                MessageBox.Show("At least 1 command has to be checked", "No command selected",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private TreeNode createTreeNodeFromCommand(CommandConfiguration command) {
            var commandName = new FullTypeNameEntry(command.TypeActivationName).TypeName;
            var commandDisplayText = commandName.Substring(commandName.LastIndexOf('.') + 1);
            if (!string.IsNullOrWhiteSpace(command.ShortDescription)) {
                commandDisplayText += " - " + command.ShortDescription;
            }
            TreeNode cmdNode = new TreeNode(commandDisplayText);
            cmdNode.Checked = true;
            cmdNode.Tag = command;
            return cmdNode;
        }
        private void commandsList_AfterSelect(object sender, TreeViewEventArgs e) {
            var selectedCommand = e.Node.Tag as CommandConfiguration;
            if (selectedCommand != null) {
                commandParametersList.DataSource = selectedCommand.ConstructorArgs.Select(a => new { a.Key, a.Value }).ToList();
            }
        }
        private void commandsList_AfterCheck(object sender, TreeViewEventArgs e) {
            FormMode = FormModeEnum.Ready;
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
        #endregion

        private void resetForm() {
            commandsList.Nodes.Clear();
            commandParametersList.DataSource = null;
            tokensList.DataSource = null;
            statusWindow.Text = string.Empty;
        }

        private void displayError(string message) {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        bool IsRunByAdmin() {
            try {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch {
                return false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        #region Config file context menu
        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            Clipboard.SetText(txtConfigFile.Text);
            txtConfigFile.Clear();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            Clipboard.SetText(txtConfigFile.Text);
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            txtConfigFile.Text = Clipboard.GetText();
        }
        private void configFileContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
            var hasConfigFile = !string.IsNullOrWhiteSpace(txtConfigFile.Text);
            cutToolStripMenuItem.Enabled = hasConfigFile;
            copyToolStripMenuItem.Enabled = hasConfigFile;
            pasteToolStripMenuItem.Enabled = !string.IsNullOrWhiteSpace(Clipboard.GetText());
            openContainingFolderToolStripMenuItem.Enabled = hasConfigFile;
            openFileInNotepadToolStripMenuItem.Enabled = hasConfigFile;
        }

        private void openFileInNotepadToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start("notepad++", txtConfigFile.Text);
        }

        private void openContainingFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start("explorer", Path.GetDirectoryName(txtConfigFile.Text));
        }
        #endregion

      
    }
}
