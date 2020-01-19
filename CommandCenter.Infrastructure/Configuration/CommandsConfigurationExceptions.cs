using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Infrastructure.Configuration {
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

    public class TokenConfigurationException : ApplicationException {
        public TokenConfigurationException(string message) :
            base(message) {

        }
    }
    public class TokenAttributeNotFoundException : TokenConfigurationException {
        public TokenAttributeNotFoundException(string attribute)
            : base($"Token attribute {attribute} not found") {
        }
    }

    public class TokenAttributeValueBlankException : TokenConfigurationException {
        public TokenAttributeValueBlankException(string attribute)
            : base($"Value of Token attribute {attribute} should not be blank") {

        }
    }

    public class DuplicateTokenKeyException : TokenConfigurationException {
        public DuplicateTokenKeyException(string keyValue) :
            base($"Token key {keyValue} should be unique") {

        }
    }
    #endregion
}
