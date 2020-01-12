using CommandCenter.Infrastructure.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;

namespace CommandCenter.Infrastructure.Tests {
    [TestClass]
    public class CommandsConfigurationXmlSourceTests {
        [TestMethod]
        public void itShouldLoadCommandConfigurationsWithParameterElements() {
            var xmlConfig = @"
                <commands>
                    <command>
                        <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        <ctorArgs>
                            <ctorArg name='dummyName' value='dummyValue' />
                        </ctorArgs>
                    </command>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();

            Assert.AreEqual(1, cmdConfigs.Count);
        }

        [TestMethod]
        public void itShouldLoadCommandConfigurationsWithoutParameterElements() {
            var xmlConfig = @"
                <commands>
                    <command>
                        <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                    </command>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();

            Assert.AreEqual(1, cmdConfigs.Count);
        }




        [TestMethod]
        [ExpectedException(typeof(CommandNodeNotFoundException))]
        public void itShouldRaiseExceptionWhenCommandNodeNotFound() {
            var xmlConfig = @"
                <commands>
                    <commandNodeNot>
                    </commandNodeNot>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();

            Assert.AreEqual(0, cmdConfigs.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeNameNodeNotFoundException))]
        public void itShouldRaiseExceptionWhenTypeNameNotFound() {
            var xmlConfig = @"
                <commands>
                    <command>
                        <ctorArgs>
                            <ctorArg name='dummyName' value='dummyValue' />
                        </ctorArgs>
                    </command>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();

            Assert.AreEqual(0, cmdConfigs.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(CtorValueAttributeNotFoundException))]
        public void itShouldRaiseExceptionWhenCtorValueAttributeNotFound() {
            var xmlConfig = @"
                <commands>
                    <command>
                        <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        <ctorArgs>
                            <ctorArg name='dummyName'/>
                        </ctorArgs>
                    </command>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();

            Assert.AreEqual(0, cmdConfigs.Count);
        }

        [TestMethod]
        public void itShouldRaiseNotExceptionWhenCtorNameAttributeIsOmitted() {
            var xmlConfig = @"
                <commands>
                    <command>
                        <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        <ctorArgs>
                            <ctorArg value='someValue'/>
                        </ctorArgs>
                    </command>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();

            Assert.AreEqual(1, cmdConfigs.Count);
        }

        [TestMethod]
        public void itShouldNotRaiseExceptionWhenCtorArgsHasNonctorArgsElements() {
            var xmlConfig = @"
                <commands>
                    <command>
                        <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        <ctorArgs>
                            <ctorArg value='someValue'/>
                            <!-- <ctorArg value='commentedOutForTesting'/> -->
                            <anotherNonCtorArgElement />
                        </ctorArgs>
                    </command>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();

            Assert.AreEqual(1, cmdConfigs.Count);
        }
    }
}
