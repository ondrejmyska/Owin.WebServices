using System;
using System.Collections.Generic;
using System.Linq;

namespace Owin.WebServices
{
    static class SoapMessageFormatter
    {
        const string soap11 = "<?xml version=\"1.0\"?>\r\n" +
                              "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n" +
                              "<soap:Body>\r\n" +
                              "{0}\r\n" +
                              "</soap:Body>\r\n" +
                              "</soap:Envelope>";

        public static string CreateSoap11Response(string body)
        {
            var bodyWithoutXmlDeclaration = (body.Split(new char[] { '\r', '\n' }, 2)[1]).Trim();
            return String.Format(soap11, bodyWithoutXmlDeclaration);
        }
    }
}
