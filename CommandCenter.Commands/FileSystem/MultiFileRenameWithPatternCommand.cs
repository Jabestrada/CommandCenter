using CommandCenter.Commands.FileSystem.BaseDefinitions;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandCenter.Commands.FileSystem {
    public class MultiFileRenameWithPatternCommand : BaseFileCommand {

        public string Pattern { get; protected set; }
        public List<string> SourceFiles { get; protected set; }
        public Dictionary<string, string> RenamedFiles { get; protected set; }

        protected Dictionary<string, Func<string, string, string, string>> Tokens;
        public DateTime DateTimeReference { get; protected set; }

        public MultiFileRenameWithPatternCommand(string pattern, params string[] sourceFiles) {
            Pattern = pattern;
            SourceFiles = sourceFiles.ToList();
            RenamedFiles = new Dictionary<string, string>();
            initializeTokens();
        }

        public MultiFileRenameWithPatternCommand(string pattern, IFileSystemCommandsStrategy fileSystemCommands,
                                                                 params string[] sourceFiles)
            : this(pattern, sourceFiles) {
            FileSystemCommands = fileSystemCommands;
        }

        public override bool IsUndoable => true;
        public override bool HasPreFlightCheck => true;
        public override void Do() {
            foreach (var file in SourceFiles) {
                if (!FileExists(file)) {
                    DidCommandSucceed = false;
                    SendReport($"Failed to rename {file} because it does not exist, or application does not have sufficient permissions", ReportType.DoneTaskWithFailure);
                    return;
                }
            }

            try {
                doRename();
                DidCommandSucceed = true;
                SendReport($"Renamed {SourceFiles.Count} file(s) with pattern {Pattern}", ReportType.DoneTaskWithSuccess);
            }
            catch (Exception exc) {
                DidCommandSucceed = false;
                SendReport($"FAILED to rename all files using pattern {Pattern}. {exc.Message}.", ReportType.DoneTaskWithFailure);
            }
        }

        public override void Undo() {
            foreach (var file in RenamedFiles) {
                FileMove(file.Value, file.Key);
                SendReport($"Reverting file {file.Value} to its original name {file.Key}", ReportType.Progress);
            }
        }

        public override bool PreFlightCheck() {
            foreach (var file in SourceFiles) {
                if (FileSystemCommands.DirectoryExists(file)) {
                    if (!PreflightCheckDirectoryReadWriteAccess(file)) return false;
                }

                if (!FileExists(file)) {
                    SendReport($"{ShortName} is likely to FAIL because at least one of its sources ({file}) does not exist, or application does not have sufficient permissions.", ReportType.DonePreFlightWithFailure);
                    return false;
                }
            }
            return DefaultPreFlightCheckSuccess();
        }

        #region Private
        private void doRename() {
            List<string> parsedTokens = parseTokensFromPattern();
            DateTimeReference = DateTime.Now;
            foreach (var file in SourceFiles) {
                var computedNewName = Pattern;
                foreach (var token in parsedTokens) {
                    var tokenKey = token.Substring(0, 1);
                    if (!Tokens.ContainsKey(tokenKey)) throw new UnrecognizedReplacementToken(token);

                    var replacerFunc = Tokens[tokenKey];
                    computedNewName = replacerFunc(token, file, computedNewName);
                }

                computedNewName = Path.Combine(Path.GetDirectoryName(file), computedNewName);

                if (FileExists(computedNewName)) {
                    throw new Exception($"Cannot rename {file} to {computedNewName} because the latter already exists");
                }

                FileSystemCommands.FileMove(file, computedNewName);
                SendReport($"Renamed {file} to {computedNewName} using pattern {Pattern}", ReportType.Progress);
                RenamedFiles.Add(file, computedNewName);
            }
        }

        private List<string> parseTokensFromPattern() {
            var tokensFromPattern = new List<string>();
            bool foundOpenBracket = false;
            string currentToken = string.Empty;
            for (int j = 0; j < Pattern.Length; j++) {
                var currentChar = Pattern[j];
                if (currentChar == '[') {
                    if (foundOpenBracket) throw new MismatchedSquareBracket(j, true);

                    foundOpenBracket = true;
                }
                else if (currentChar == ']') {
                    if (!foundOpenBracket) throw new MismatchedSquareBracket(j, false);

                    if (currentToken.Length > 0) {
                        if (!tokensFromPattern.Contains(currentToken)) {
                            tokensFromPattern.Add(currentToken);
                        }
                    }
                    // Reset for next token match
                    currentToken = string.Empty;
                    foundOpenBracket = false;
                }
                else {
                    if (foundOpenBracket) currentToken += currentChar;
                }
            }
            return tokensFromPattern;
        }

        private void initializeTokens() {
            Tokens = new Dictionary<string, Func<string, string, string, string>>();
            // All tokens should have single-char key, although a token itself can have multiple characters.
            Tokens.Add("n", fileNameReplacer);
            Tokens.Add("d", datetimeReplacer);
            Tokens.Add("e", filenameExtensionReplacer);
        }

        private string filenameExtensionReplacer(string replacerToken, string sourceFileName, string currentName) {
            var currentExt = Path.GetExtension(sourceFileName);
            return currentName.Replace($"[{replacerToken}]", currentExt);
        }

        private string datetimeReplacer(string replacerToken, string sourceFileName, string currentName) {
            // ex. [d:yyyyMM]
            var dateTimeFormatSpecifiers = replacerToken.Substring(2);
            return currentName.Replace($"[{replacerToken}]", DateTimeReference.ToString(dateTimeFormatSpecifiers));
        }

        private string fileNameReplacer(string replacerToken, string sourceFileName, string currentName) {
            return currentName.Replace($"[{replacerToken}]", Path.GetFileNameWithoutExtension(sourceFileName));
        }

        #endregion

        #region Exception types
        public class UnrecognizedReplacementToken : ApplicationException {
            public UnrecognizedReplacementToken(string token)
                : base($"Unrecognized filename replacement token {token}") {

            }
        }

        public class MismatchedSquareBracket : ApplicationException {
            public MismatchedSquareBracket(int index, bool isOpenBracket)
                : base(string.Format("{0} bracket at position {1} has no matching {2} bracket",
                        isOpenBracket ? "Opening" : "Closing",
                        index,
                        isOpenBracket ? "closing" : "opening")) {

            }
        }
        #endregion
    }


}
