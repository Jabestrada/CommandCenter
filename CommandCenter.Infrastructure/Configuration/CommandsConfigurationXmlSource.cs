using System;
using System.Collections.Generic;
using System.Xml;

namespace CommandCenter.Infrastructure.Configuration {
    public class CommandsConfigurationXmlSource : IConfigurationSource {

        private readonly XmlNode _rootConfigNode;
        public CommandsConfigurationXmlSource(XmlNode rootConfigNode) {
            _rootConfigNode = rootConfigNode;
        }

        public List<CommandConfiguration> GetCommandConfigurations() {
            var commandNodes = getCommandNodes();
            var commandConfigurations = new List<CommandConfiguration>();
            foreach (XmlNode commandNode in commandNodes) {
                CommandConfiguration commandConfig = createCommandConfiguration(commandNode);
                commandConfigurations.Add(commandConfig);
            }
            return commandConfigurations;
        }

        #region Private methods
        private CommandConfiguration createCommandConfiguration(XmlNode commandNode) {
            var commandConfig = new CommandConfiguration();
            setTypeActivationName(commandConfig, commandNode);
            setShortDescription(commandConfig, commandNode);
            setEnabled(commandConfig, commandNode);
            setCtorArgs(commandConfig, commandNode);
            return commandConfig;
        }

        private void setEnabled(CommandConfiguration commandConfig, XmlNode commandNode) {
            var enabledAttr = commandNode.Attributes["enabled"];
            if (enabledAttr == null) {
                commandConfig.Enabled = true;
                return;
            }

            var enableAttribValue = enabledAttr.Value.Trim().ToUpperInvariant();
            if (!string.IsNullOrWhiteSpace(enableAttribValue) && (enableAttribValue != "TRUE" && enableAttribValue != "FALSE")) {
                throw new InvalidEnabledAttributeValueException($"Invalid value for command[@enabled] attribute: {enableAttribValue}");
            }
            commandConfig.Enabled = string.IsNullOrWhiteSpace(enableAttribValue) || enabledAttr.Value.Trim().ToUpperInvariant() == "TRUE";
        }

        private void setShortDescription(CommandConfiguration commandConfig, XmlNode commandNode) {
            XmlNode typeNameNode = getChildNode(commandNode, "shortDescription");
            commandConfig.ShortDescription = typeNameNode == null ? string.Empty : typeNameNode.InnerText;
        }

        private void setCtorArgs(CommandConfiguration commandConfig, XmlNode commandNode) {
            var ctorArgs = commandNode.SelectNodes("ctorArgs/ctorArg");
            foreach (XmlNode ctorArgNode in ctorArgs) {
                var ctorArgValueAttribute = ctorArgNode.Attributes["value"];
                if (ctorArgValueAttribute == null) throw new CtorValueAttributeNotFoundException();

                var ctorArgNameAttribute = ctorArgNode.Attributes["name"];
                var ctorArgName = ctorArgNameAttribute != null ? ctorArgNameAttribute.Value : Guid.NewGuid().ToString();

                if (commandConfig.ConstructorArgs.ContainsKey(ctorArgName)) {
                    throw new DuplicateCtorArgNameException(ctorArgName);
                }
                commandConfig.ConstructorArgs.Add(ctorArgName, ctorArgValueAttribute.Value);
            }
        }

        private void setTypeActivationName(CommandConfiguration commandConfig, XmlNode commandNode) {
            XmlNode typeNameNode = getChildNode(commandNode, "typeName", new TypeNameNodeNotFoundException());
            commandConfig.TypeActivationName = typeNameNode.InnerText;
        }

        private XmlNode getChildNode(XmlNode sourceNode, string nodeName, Exception exToThrowIfNotFound) { 
            XmlNode requestedNode = sourceNode.SelectSingleNode(nodeName);
            if (requestedNode == null && exToThrowIfNotFound != null) {
                throw exToThrowIfNotFound;
            }
            return requestedNode;
        }

        private XmlNode getChildNode(XmlNode sourceNode, string nodeName) {
            return getChildNode(sourceNode, nodeName, null);
        }

        private XmlNodeList getCommandNodes() {
            XmlNodeList commandNodes = _rootConfigNode.SelectNodes("//command");
            if (commandNodes == null || commandNodes.Count == 0) throw new CommandNodeNotFoundException();

            return commandNodes;
        }
        #endregion
    }

    #region Exception types
    public class CommandNodeNotFoundException : ArgumentException {
        public CommandNodeNotFoundException() :
            base("<command> node not found in configuration") {

        }
    }

    public class TypeNameNodeNotFoundException : ArgumentException {
        public TypeNameNodeNotFoundException()
            : base("<typeName> node not found in configuration") {

        }
    }

    public class CtorValueAttributeNotFoundException : ArgumentException {
        public CtorValueAttributeNotFoundException()
            : base("ctorArg value attribute not found") {

        }
    }

    public class InvalidEnabledAttributeValueException : ApplicationException {
        public InvalidEnabledAttributeValueException(string enableAttribValue) :
            base($"Invalid value for command[@enabled] attribute: {enableAttribValue}") {

        }
    }
    public class DuplicateCtorArgNameException : ArgumentException {
        public DuplicateCtorArgNameException(string ctorArgName)
            : base($"Duplicate ctorArg name {ctorArgName} found in configuration") {

        }
    }
    #endregion
}
