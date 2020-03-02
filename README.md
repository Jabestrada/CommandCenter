# CommandCenter
Framework for automating commands that are mostly related to (but not limited to) DevOps tasks

I was motivated to write this framework/application while working with the IT department of a remote client. Their CI/CD infrastructure was at the nascent stages and their deployment procedures, while properly documented, were entirely manual. I figured it was a great opportunity and mental exercise to build something that will somehow automate the process. 

## How It Works
1. A configuration file contains an ordered list of commands that one wishes to automate. For instance, via the configuration file, one can assemble a series of commands that will:
   1. Update a local repository copy in preparation for a build.
   2. Build the pertinent projects.
   3. Publish the web app.
   4. Zip the published web app.
2. User runs the WinForms app (console client also available) from which the user selects and loads the configuration file from the file system.
3. User clicks on a button to run the commands, and let them do their thing.
<!--
## Highlights
-->

## Available Commands Out-of-the-Box
<!-- While the Commands framework is extensible so that etc. -->
1. SvnUpdateCommand: runs svn update command on a particular repo.
2. MsCleanRebuildCommand: runs MsBuild.exe to clean and rebuild solutions
3. MsPublishWebAppCommand: runs MsBuild.exe to publish web apps
4. IisAppPoolStartCommand/IisAppPoolStartCommand: starts/stops an IIS App pool
5. Zip7CompressCommand: runs 7-zip to compress files and folders
6. File system manipulation commands:
   * FileCopyCommand
   * FileDeleteCommand
   * MultiFileRenameWithPatternCommand
   * DirectoryCopyContentsCommand
   * DirectoryDeleteCommand
   * DirectoryDeleteContentOnlyCommand
<!--
A rundown of the assemblies/projects in this repo is as follows:
- CommandCenter.Infrastructure contains the building blocks of the framework and is responsible for configuration, orchestration and command definition/creation. At the heart of this project is the BaseCommand type from which all commands should inherit from. Therefore, any application that wishes to define new Command types should add a reference to this assembly.
- CommandCenter.Commands contains BaseCommand implementations for commonly used tasks such as:
  * SvnUpdateCommand: runs svn update command on a particular repo.
  * Zip7CompressCommand: runs 7-zip to compress files and folders
  * MsCleanRebuildCommand: runs MsBuild.exe to clean and rebuild solutions
  * MsPublishWebAppCommand: runs MsBuild.exe to publish web apps
  * IisAppPoolStartCommand/IisAppPoolStartCommand: starts/stops an IIS App pool
  * Commands under /FileSystem: various commands for common file system manipulation (dir/file copy, delete, etc.)  
- CommandCenter.UI.Console and CommandCenter.UI.WinForms contains console-based/Windows Forms-based clients for invoking commands defined in a configuration file.
-->

