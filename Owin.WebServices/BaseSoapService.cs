using Owin.WebServices.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace Owin.WebServices
{
    public abstract class BaseSoapService : ISoapService
    {
        protected readonly Func<Type, object> webServiceCreator;

        protected BaseSoapService(Func<Type, object> webServiceCreator)
        {
            this.webServiceCreator = webServiceCreator;
        }

        public string ParseWebServiceClassName(IDictionary<string, string[]> headers)
        {
            throw new NotImplementedException();
        }

        public object Invoke(SoapRequest soapRequest)
        {
            var webService = soapRequest.WebService;
            var instance = webServiceCreator(webService.WebServiceType);
            var methodInfo = webService.WebMethods[soapRequest.MethodName];
            return methodInfo.Invoke(instance, soapRequest.Arguments);
        }

        protected static object[] GetMethodArguments(WebServiceInfo webService, string methodName, XElement request)
        {
            var deserializedArguments = new List<object>();
            var argumentsRaw = request.Elements().ToArray();

            if (webService.WebMethods.ContainsKey(methodName))
            {
                var methodInfo = webService.WebMethods[methodName];
                var parameters = methodInfo.GetParameters();

                for (var i = 0; i < argumentsRaw.Count(); i++)
                {
                    var argumentType = parameters[i].ParameterType;
                    var parametrName = parameters[i].Name;
                    if (argumentType.IsPrimitive)
                    {
                        deserializedArguments.Add(Convert.ChangeType(argumentsRaw[i].Value, argumentType));
                    }
                    else if (argumentType == typeof(string))
                    {
                        deserializedArguments.Add(argumentsRaw[i].Value);
                    }
                    else
                    {
                        var argumentRaw = argumentsRaw[i];
                        var strings = argumentRaw.ToString().Replace(parametrName, argumentType.Name);
                        argumentRaw.Name = XName.Get(argumentType.Name, webService.WebServiceNamespace);
                        var xmlSerializer = new XmlSerializer(argumentType, webService.WebServiceNamespace);

                        var deserializedArgument = xmlSerializer.Deserialize(new StringReader(strings));
                        deserializedArguments.Add(deserializedArgument);
                    }
                }
            }
            return deserializedArguments.ToArray();
        }


        public abstract SoapRequest ParseSoapRequest(Stream request, string webServiceName,
            IDictionary<string, WebServiceInfo> supportedWebServices);

        public abstract string CreateResponseBody<T>(T returnValue, WebServiceInfo webService);

        public abstract string CreateResponseErrorBody(Exception e);
    }
}