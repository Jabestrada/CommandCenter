using CommandCenter.Commands.FileSystem.BaseDefinitions;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommandCenter.Commands.FileSystem {
    public class FileSystemCommands : IFileSystemCommandsStrategy {
        public void FileCopy(string source, string destination) {
            File.Copy(source, destination);
        }
        public void FileDelete(string filename) {
            File.Delete(filename);
        }

        public bool FileExists(string filename) {
            return File.Exists(filename);
        }

        public void FileMove(string sourceFilename, string destinationFilename) {
            File.Move(sourceFilename, destinationFilename);
        }

        public bool DirectoryExists(string dirName) {
            return Directory.Exists(dirName);
        }

        public void DirectoryMoveContents(string sourceDirName, string destinationDirName) {
            if (!DirectoryExists(sourceDirName)) {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");
            }

            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destinationDirName)) {
                Directory.CreateDirectory(destinationDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                string temppath = Path.Combine(destinationDirName, file.Name);
                file.MoveTo(temppath);  
            }

            // Copy subdirectories
            foreach (DirectoryInfo subdir in dirs) {
                string temppath = Path.Combine(destinationDirName, subdir.Name);
                subdir.MoveTo(temppath);
            }
        }

        public void DirectoryCopyContents(string sourceDirName, string destinationDirName, Func<string, string, bool> preCopyCallback, Action<string, string> postCopyCallback) {

            if (!DirectoryExists(sourceDirName)) {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");
            }

            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destinationDirName)) {
                Directory.CreateDirectory(destinationDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                string temppath = Path.Combine(destinationDirName, file.Name);
                if (preCopyCallback != null) {
                    if (!preCopyCallback(file.FullName, temppath)) return;
                }

                file.CopyTo(temppath, false);

                if (postCopyCallback != null) postCopyCallback(file.FullName, temppath);
            }

            // Copy subdirectories
            foreach (DirectoryInfo subdir in dirs) {
                string temppath = Path.Combine(destinationDirName, subdir.Name);
                DirectoryCopyContents(subdir.FullName, temppath, preCopyCallback, postCopyCallback);
            }
        }

        public void DirectoryDelete(string dirName) {
            Directory.Delete(dirName, true);
        }
        public void DirectoryMove(string sourceDir, string targetDir) {
            Directory.Move(sourceDir, targetDir);
        }

        public void DirectoryDeleteContentsOnly(string sourceDirectory, Action<string, FileSystemItemType> progressCallback) {
            DirectoryInfo sourceDir = new DirectoryInfo(sourceDirectory);

            FileInfo[] files = sourceDir.GetFiles();
            foreach (var file in files) {
                FileDelete(file.FullName);
                if (progressCallback != null) {
                    progressCallback(file.FullName, FileSystemItemType.File);
                }
            }

            DirectoryInfo[] dirs = sourceDir.GetDirectories();
            foreach (var dir in dirs) {
                DirectoryDelete(dir.FullName);
                if (progressCallback != null) {
                    progressCallback(dir.FullName, FileSystemItemType.Directory);
                }
            }


        }

        public IEnumerable<string> DirectoryGetFiles(string sourceDir) {
            return Directory.GetFiles(sourceDir);
        }
    }
}
