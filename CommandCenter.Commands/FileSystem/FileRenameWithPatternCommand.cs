using CommandCenter.Commands.FileSystem.BaseDefinitions;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommandCenter.Commands.FileSystem {
    public class FileRenameWithPatternCommand : BaseFileCommand {
        public string NewNamePattern { get; protected set; }
        public string ComputedNewName { get; protected set; }
        public DateTime DateTimeReference { get; protected set; }
        public override bool IsUndoable => true;

        protected Dictionary<string, Func<string, string, string, string>> Tokens;

        public FileRenameWithPatternCommand(string currentName, string newNamePattern) {
            SourceFilename = currentName;
            NewNamePattern = newNamePattern;
            initializeTokens();
        }
        public FileRenameWithPatternCommand(string currentName, string newNamePattern, IFileSystemCommandsStrategy fileSystemCommands)
                : this(currentName, newNamePattern) {
            FileSystemCommands = fileSystemCommands;
        }

        public override void Do() {
            if (!FileExists(SourceFilename)) {
                DidCommandSucceed = false;
                SendReport($"Failed to rename {SourceFilename} because it does not exist", ReportType.DoneTaskWithFailure);
                return;
            }
            try {

                doRename();
                DidCommandSucceed = true;
                SendReport($"File {SourceFilename} renamed with pattern {NewNamePattern} to {ComputedNewName}", ReportType.DoneTaskWithSuccess);
            }
            catch (Exception exc) {
                DidCommandSucceed = false;
                SendReport($"FAILED to rename {SourceFilename} to {ComputedNewName}. {exc.Message}.", ReportType.DoneCleanupWithFailure);
            }
        }

        public override void Undo() {
            if (DidCommandSucceed) FileMove(ComputedNewName, SourceFilename);
        }

        private void doRename() {
            List<string> parsedTokens = parseTokensFromPattern();
            ComputedNewName = NewNamePattern;
            DateTimeReference = DateTime.Now;
            foreach (var token in parsedTokens) {
                var tokenKey = token.Substring(0, 1);
                if (!Tokens.ContainsKey(tokenKey)) throw new UnrecognizedReplacementToken(token);

                var replacerFunc = Tokens[tokenKey];
                ComputedNewName = replacerFunc(token, SourceFilename, ComputedNewName);
            }

            ComputedNewName = Path.Combine(Path.GetDirectoryName(SourceFilename), ComputedNewName);

            if (FileExists(ComputedNewName)) {
                throw new Exception($"File {ComputedNewName} already exists");
            }

            FileSystemCommands.FileMove(SourceFilename, ComputedNewName);
        }

        private List<string> parseTokensFromPattern() {
            var tokensFromPattern = new List<string>();
            bool foundOpenBracket = false;
            string currentToken = string.Empty;
            for (int j = 0; j < NewNamePattern.Length; j++) {
                var currentChar = NewNamePattern[j];
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
    }

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
}
