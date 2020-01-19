using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CommandCenter.Infrastructure.Configuration {
    public class CommandsConfigurationXmlSource : IConfigurationSource {
        private List<Token> _tokens = null;
        public List<Token> Tokens {
            set {
                if (value == null) throw new ArgumentNullException(nameof(value));

                _tokens = value;
            }
            get {
                if (_tokens == null) {
                    readTokens();
                }
                return _tokens;
            }
        }

        private readonly XmlNode _rootConfigNode;
        public CommandsConfigurationXmlSource(XmlNode rootConfigNode) {
            _rootConfigNode = rootConfigNode;
        }

        private void readTokens() {
            var tokenNodes = getTokenNodes();
            var tokenList = new List<Token>();
            foreach (XmlNode node in tokenNodes) {
                var keyAttributeValue = getKeyAttributeValue(node, tokenList);

                var newToken = new Token {
                    Key = keyAttributeValue,
                    Value = getAttributeValueFromNode(node, "value")
                };

                tokenList.Add(newToken);
            }
            _tokens = tokenList;
        }

        private string getKeyAttributeValue(XmlNode node, List<Token> tokenList) {
            var keyAttributeValue = getAttributeValueFromNode(node, "key");
            if (tokenList.Any(t => t.Key == keyAttributeValue)) throw new DuplicateTokenKeyException(keyAttributeValue);

            return keyAttributeValue;
        }

        private string getAttributeValueFromNode(XmlNode node, string attribKey) {
            var attribute = node.Attributes[attribKey];
            if (attribute == null) throw new TokenAttributeNotFoundException(attribKey);

            var attributeValue = attribute.Value;
            if (string.IsNullOrWhiteSpace(attributeValue)) throw new TokenAttributeValueBlankException(attribKey);


            return attributeValue;
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
        private XmlNodeList getTokenNodes() {
            return _rootConfigNode.SelectNodes("//token");
        }


        #endregion
    }


}
