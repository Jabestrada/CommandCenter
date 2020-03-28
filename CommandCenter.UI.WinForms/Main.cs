﻿using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Configuration;
using CommandCenter.Infrastructure.Orchestration;
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

        private bool _wasCommandCancelled = false;
        private CommandsControllerWinForms _controller;
        private List<CommandConfiguration> _loadedCommandConfigurations;
        private List<CommandConfiguration> _selectedCommandConfigurations;

        private List<Token> _loadedTokens;
        private string _lastLoadedConfig = string.Empty;
        public Main() {
            InitializeComponent();
            _controller = new CommandsControllerWinForms(_reportReceiver);
            //var defaultConfig = Path.Combine(Application.StartupPath, "CommandCenter.config");
            //if (string.IsNullOrWhiteSpace(txtConfigFile.Text) && File.Exists(defaultConfig)) {
            //    txtConfigFile.Text = defaultConfig;
            //}
            //loadCommands();
            if (IsAnAdministrator()) {
                this.Text = this.Text + " - launched as Admin";
            }
            else {
                this.Text = this.Text + " - not launched as Admin";
            }
            cancelButton.Location = btnRun.Location;

            FormMode = FormModeEnum.Ready;
        }

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
            var finalReports = _controller.Reports.Where(r => isDoneTaskReport(r.ReportType));
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

            var finalReports = _controller.Reports.Where(r => isDonePreflightTaskReport(r.ReportType));
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

        private bool isDoneTaskReport(ReportType r) {
            return r == ReportType.DoneTaskWithFailure || r == ReportType.DoneTaskWithSuccess;
        }

        private bool isDonePreflightTaskReport(ReportType r) {
            return r == ReportType.DonePreFlightWithFailure || r == ReportType.DonePreflightWithSuccess;
        }
        #endregion

        #region commandsList Treeview events
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

    }
}
