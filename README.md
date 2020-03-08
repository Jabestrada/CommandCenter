# CommandCenter
Framework for automating commands that are mostly related to (but not limited to) DevOps tasks

I was motivated to write this framework/application while working with the IT department of a remote client. Their CI/CD infrastructure was at the nascent stages and their deployment procedures, while properly documented, were entirely manual. I figured it was a great opportunity and mental exercise to build something that will somehow automate the process. 

## How It Works
1. User creates a configuration file that contains an ordered list of commands that one wishes to automate. For instance, via the configuration file, one can assemble a series of commands that will:
   1. Update a local repository copy in preparation for a build.
   2. Build the pertinent projects.
   3. Publish the web app.
   4. Zip the published web app.
2. User runs the WinForms app (console client also available) from which the user selects and loads the configuration file from the file system.
3. User clicks on a button to run the commands, and let them do their thing.

## Highlights
1. Undo/rollback support (where applicable). Part of the core interface is an Undo method that gets invoked if one of the commands in the chain were to fail. For instance, if a command chain uses the FileDeleteCommand and then a subsequent command fails, the file deleted by the FileDeleteCommand is restored as part of the rollback process. Implementing Undo is optional, and the WinForms client will warn the user if there are any selected commands that don't support Undo.
2. Several commands available out-of-the-box such as MsPublishWebAppCommand, IisAppPoolStartCommand/IisAppPoolStopCommand and common file system commands to copy, delete and rename files using patterns.
3. Extensible Command framework. Developers can build their own commands and plug them into the application using a self-explanatory interface:
   * Do
   * Undo
   * IsUndoable
   * Cleanup
4. Support for primitive type constructor arguments to commands via configuration. Commands can be made more flexible and reusable via configuration values that are passed as constructor parameters during command instantiation by the framework.
5. Token support in configuration file. Instead of repetitively using long command typenames or file/folder locations in the configuration file, you can define a token once and then reference that token anywhere the token value is used in the configuration file.
6. Pre-flight check. One can trigger a pre-flight check to verify if the selected commands are likely to succeed when actually run. 

## Getting Started
1. Download this repo.
2. Open CommandCenter.sln (tested on Visual Studio 2019 only) and set startup project to CommandCenter.UI.WinForms.
3. Build and run the solution.
4. Follow along a basic [walkthrough](https://github.com/Jabestrada/CommandCenter/blob/master/Documentation/Walkthrough.pdf). 
5. Learn more by reading the [wiki](https://github.com/Jabestrada/CommandCenter/wiki)

