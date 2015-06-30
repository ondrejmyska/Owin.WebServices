using Owin.WebServices.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Reflection;

namespace Owin.WebServices
{
    public class Soap11Service : BaseSoapService
    {
        public Soap11Service(Func<Type, object> webServiceCreator)
            : base(webServiceCreator)
        {
        }

        public override SoapRequest ParseSoapRequest(Stream request, string webServiceName,
            IDictionary<string, WebServiceInfo> supportedWebServices)
        {
            var bodyElement = GetBodyElement(request);
            var methodName = bodyElement.First().Name.LocalName;
            var webService = supportedWebServices[webServiceName];
            var arguments = GetMethodArguments(webService, methodName, bodyElement.First());
            return new SoapRequest(methodName, webService, arguments);
        }

        public override string CreateResponseBody<T>(T returnValue, WebServiceInfo webService)
        {
            var nmsp = returnValue.GetType().GetCustomAttribute(typeof(XmlTypeAttribute));
            var nmsps = nmsp == null ? webService.WebServiceNamespace : ((XmlTypeAttribute)nmsp).TypeName;

            var memoryStream = new MemoryStream();
            var serializer = new XmlSerializer(returnValue.GetType(), nmsps);
            serializer.Serialize(memoryStream, returnValue);
            memoryStream.Position = 0;
            using(var streamReader = new StreamReader(memoryStream))
            {
                var rawResponseObject = streamReader.ReadToEnd();
                return SoapMessageFormatter.CreateSoap11Response(rawResponseObject);
            }
        }

        private static IEnumerable<XElement> GetBodyElement(Stream request)
        {
            using (StreamReader reader = new StreamReader(request))
            {
                var requestString = reader.ReadToEnd();
                var soapDocument = XDocument.Parse(requestString);
                return soapDocument.Document.
                    Descendants((XNamespace)"http://schemas.xmlsoap.org/soap/envelope/" + "Body").
                    Elements().Where(e => e.FirstNode != null);
            }
        }

        public override string CreateResponseErrorBody(Exception e)
        {
            throw new NotImplementedException();
        }
    }
}
