using CommandCenter.Infrastructure.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml;

namespace CommandCenter.Infrastructure.Tests {
    [TestClass]
    public class CommandsConfigurationXmlSourceTests {
        [TestMethod]
        public void itShouldLoadCommandConfigurationsWithParameterElements() {
            var xmlConfig = @"
                <commandCenter>
                    <commands>
                        <command>
                            <typeName>CommandCenter.Tests, CommandCenter.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                            <ctorArgs>
                                <ctorArg name='dummyName' value='dummyValue' />
                            </ctorArgs>
                        </command>
                    </commands>
                </commandCenter>
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
                <commandCenter>
                    <commands>
                        <command>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        </command>
                    </commands>
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();

            Assert.AreEqual(1, cmdConfigs.Count);
        }


        [TestMethod]
        public void itShouldNotLoadCommentedOutCommandConfigurations() {
            var xmlConfig = @"
                <commandCenter>
                    <commands>
                        <command>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        </command>
                        <!--
                        <command>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        </command>
                        -->
                    </commands>
                </commandCenter>
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
                <commandCenter>
                    <commands>
                        <commandNodeNot>
                        </commandNodeNot>
                    </commands>
                </commandCenter>
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
                <commandCenter>
                    <commands>
                        <command>
                            <ctorArgs>
                                <ctorArg name='dummyName' value='dummyValue' />
                            </ctorArgs>
                        </command>
                    </commands>
                </commandCenter>
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
                <commandCenter>
                    <commands>
                        <command>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                            <ctorArgs>
                            <ctorArg name='dummyName'/>
                            </ctorArgs>
                        </command>
                    </commands>
                </commandCenter>
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
                <commandCenter>
                    <commands>
                        <command>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                            <ctorArgs>
                                <ctorArg value='someValue'/>
                            </ctorArgs>
                        </command>
                    </commands>
                </commandCenter>
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
                <commandCenter>
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
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();

            Assert.AreEqual(1, cmdConfigs.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateCtorArgNameException))]
        public void itShouldRaiseExceptionWhenCtorArgsHasDuplicateName() {
            var xmlConfig = @"
                <commandCenter>
                    <commands>
                        <command>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                            <ctorArgs>
                                <ctorArg name='key1' value='someValue' />
                                <ctorArg name='key1' value='someValue' />
                            </ctorArgs>
                        </command>
                    </commands>
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();

            Assert.AreEqual(1, cmdConfigs.Count);
        }

        [TestMethod]
        public void itShouldGetShortDesriptionWhenItExists() {
            var shortDescText = "This is a test short description";
            var xmlConfig = $@"
                <commandCenter>
                    <commands>
                        <command>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                            <shortDescription>{shortDescText}</shortDescription>
                        </command>
                    </commands>
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();
            var cmdConfig = cmdConfigs.FirstOrDefault();

            Assert.IsNotNull(cmdConfig);
            Assert.AreEqual(cmdConfig.ShortDescription, shortDescText);
        }

        [TestMethod]
        public void itShouldAssignEmptyStringIfShortDesriptionDoesNotExist() {
            var xmlConfig = $@"
                <commandCenter>
                    <commands>
                        <command>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        </command>
                    </commands>
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();
            var cmdConfig = cmdConfigs.FirstOrDefault();

            Assert.IsNotNull(cmdConfig);
            Assert.AreEqual(cmdConfig.ShortDescription, string.Empty);
        }

        [TestMethod]
        public void itShouldSetEnabledToTrueIfEnabledAttributeDoesNotExist() {
            var xmlConfig = $@"
                <commandCenter>
                    <commands>
                        <command>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        </command>
                    </commands>
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();
            var cmdConfig = cmdConfigs.FirstOrDefault();

            Assert.IsNotNull(cmdConfig);
            Assert.IsTrue(cmdConfig.Enabled);
        }

         [TestMethod]
        public void itShouldSetEnabledToTrueIfEnabledAttributeValueIsTrue() {
            var xmlConfig = $@"
                <commandCenter>
                    <commands>
                        <command enabled='true'>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        </command>
                    </commands>
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();
            var cmdConfig = cmdConfigs.FirstOrDefault();

            Assert.IsNotNull(cmdConfig);
            Assert.IsTrue(cmdConfig.Enabled);
        }

            [TestMethod]
        public void itShouldSetEnabledToTrueIfEnabledAttributeValueIsTrueCaseInsensitive() {
            var xmlConfig = $@"
                <commandCenter>
                    <commands>
                        <command enabled='tRuE'>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        </command>
                    </commands>
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();
            var cmdConfig = cmdConfigs.FirstOrDefault();

            Assert.IsNotNull(cmdConfig);
            Assert.IsTrue(cmdConfig.Enabled);
        }

         [TestMethod]
        public void itShouldSetEnabledToTrueIfEnabledAttributeValueIsFalse() {
            var xmlConfig = $@"
                <commandCenter>
                    <commands>
                        <command enabled='false'>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        </command>
                    </commands>
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();
            var cmdConfig = cmdConfigs.FirstOrDefault();

            Assert.IsNotNull(cmdConfig);
            Assert.IsFalse(cmdConfig.Enabled);
        }

         [TestMethod]
        public void itShouldSetEnabledToTrueIfEnabledAttributeValueIsEmpty() {
            var xmlConfig = $@"
                <commandCenter>
                    <commands>
                        <command enabled=''>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        </command>
                    </commands>
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();
            var cmdConfig = cmdConfigs.FirstOrDefault();

            Assert.IsNotNull(cmdConfig);
            Assert.IsTrue(cmdConfig.Enabled);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidEnabledAttributeValueException))]
        public void itShouldThrowExceptionIfEnabledAttributeValueIsNeitherTrueNorFalseNorEmpty() {
            var xmlConfig = $@"
                <commandCenter>
                    <commands>
                        <command enabled='werjwjekqrl'>
                            <typeName>CommandCenter.Infrastructure.Tests, CommandCenter.Infrastructure.Tests.MockCommands.MockCommandWithNonDefaultConstructor</typeName>
                        </command>
                    </commands>
                </commandCenter>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var cmdConfigs = xmlConfigSource.GetCommandConfigurations();
        }
    }
}
