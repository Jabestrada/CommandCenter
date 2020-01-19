using CommandCenter.Infrastructure.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CommandCenter.Infrastructure.Tests {
    [TestClass]
    public class TokensConfigurationTests {
        [TestMethod]
        public void itShouldLoadTokens() {
            var tokenKey1 = "TOKEN1";
            var tokenValue1 = "TOKEN1VALUE";
            var tokenKey2 = "TOKEN2";
            var tokenValue2 = "TOKEN2VALUE";
            var xmlConfig = $@"
                <commands>
                    <tokens>
                        <token key='{tokenKey1}' value='{tokenValue1}' />
                        <token key='{tokenKey2}' value='{tokenValue2}' />
                    </tokens>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var tokens = xmlConfigSource.Tokens;

            Assert.AreEqual(2, xmlConfigSource.Tokens.Count);
            var token = tokens.First();
            Assert.IsNotNull(token);
            Assert.AreEqual(token.Key, tokenKey1);
            Assert.AreEqual(token.Value, tokenValue1);

            token = tokens.Last();
            Assert.IsNotNull(token);
            Assert.AreEqual(token.Key, tokenKey2);
            Assert.AreEqual(token.Value, tokenValue2);
        }

        [TestMethod]
        public void itShouldNotThrowExceptionIfThereAreNoTokens() {

            var xmlConfig = @"
                <commands>
                    
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var tokens = xmlConfigSource.Tokens;

            Assert.AreEqual(0, tokens.Count);


            xmlConfig = @"
                <commands>
                    <tokens>
                    </tokens>
                </commands>
                ";
            xmlDoc.LoadXml(xmlConfig);
            xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            tokens = xmlConfigSource.Tokens;

            Assert.AreEqual(0, tokens.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenAttributeNotFoundException))]
        public void itShouldThrowExceptionIfAttributeKeyIsMissing() {

            var xmlConfig = @"
                <commands>
                    <tokens>
                        <token value='TOKEN1VALUE' />
                    </tokens>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var tokens = xmlConfigSource.Tokens;

            Assert.AreEqual(0, tokens.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenAttributeValueBlankException))]
        public void itShouldThrowExceptionIfValueOfAttributeKeyIsBlank() {

            var xmlConfig = @"
                <commands>
                    <tokens>
                        <token key='' value='TOKEN1VALUE' />
                    </tokens>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var tokens = xmlConfigSource.Tokens;

            Assert.AreEqual(0, tokens.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenAttributeNotFoundException))]
        public void itShouldThrowExceptionIfAttributeValueIsMissing() {

            var xmlConfig = @"
                <commands>
                    <tokens>
                        <token key='KEY1' />
                    </tokens>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var tokens = xmlConfigSource.Tokens;

            Assert.AreEqual(0, tokens.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenAttributeValueBlankException))]
        public void itShouldThrowExceptionIfValueOfAttributeValueIsBlank() {

            var xmlConfig = @"
                <commands>
                    <tokens>
                        <token key='KEY1' value=' ' />
                    </tokens>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var tokens = xmlConfigSource.Tokens;

            Assert.AreEqual(0, tokens.Count);
        }

         [TestMethod]
         [ExpectedException(typeof(DuplicateTokenKeyException))]
        public void itShouldThrowExceptionOnDuplicateTokenKeys() {
            var tokenKey1 = "TOKEN1";
            var tokenValue1 = "TOKEN1VALUE";
            var tokenKey2 = "TOKEN1";
            var tokenValue2 = "TOKEN2VALUE";
            var xmlConfig = $@"
                <commands>
                    <tokens>
                        <token key='{tokenKey1}' value='{tokenValue1}' />
                        <token key='{tokenKey2}' value='{tokenValue2}' />
                    </tokens>
                </commands>
                ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlConfig);
            var xmlConfigSource = new CommandsConfigurationXmlSource(xmlDoc);

            var tokens = xmlConfigSource.Tokens;
        }
    }
}
