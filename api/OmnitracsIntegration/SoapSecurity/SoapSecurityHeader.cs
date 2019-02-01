using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace OmnitracsIntegration.SoapSecurity
{
    public class SoapSecurityHeader : MessageHeader
    {
        private readonly string _username;
        private readonly string _password;

        public SoapSecurityHeader(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public override string Name { get; } = "Security";

        public override string Namespace { get; } = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        private const string WsuNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("wsse", "UsernameToken", Namespace);
            writer.WriteAttributeString("wsu", "Id", WsuNamespace, Guid.NewGuid().ToString());

            writer.WriteStartElement("wsse", "Username", Namespace);
            writer.WriteValue(_username);
            writer.WriteEndElement();

            writer.WriteStartElement("wsse", "Password", Namespace);
            writer.WriteAttributeString("Type", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText");
            writer.WriteValue(_password);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("wsse", Name, Namespace);
            writer.WriteXmlnsAttribute("wsse", Namespace);
        }
    }

}
