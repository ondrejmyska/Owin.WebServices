using Owin.WebServices.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Owin.WebServices
{
    public class Middleware
    {
        readonly Func<IDictionary<string, object>, Task> nextMiddleware;
        readonly IDictionary<string, WebServiceInfo> supportedServiceInfos;
        readonly Func<Type, object> webServiceCreator;

        public Middleware(Func<IDictionary<string, object>, Task> next, Options options)
        {
            var registeredTypes = options.WebServiceTypesSelector() ?? Enumerable.Empty<Type>();
            supportedServiceInfos = WebServicesSelector.CreateWebServicesInfos(registeredTypes);
            webServiceCreator = options.WebServiceCreator == null
                ? (t) => Activator.CreateInstance(t)
                : options.WebServiceCreator;
            nextMiddleware = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var headers = environment["owin.RequestHeaders"] as IDictionary<string, string[]>;
            if (headers["Content-Type"].GetSoapVersion() != SoapVersion.None)
            {
                await Task.Factory.StartNew(() => HandleRequest(environment, new Soap11Service(webServiceCreator)));
            }
            else
                await nextMiddleware(environment);

        }

        public void HandleRequest(IDictionary<string, object> environment, ISoapService soapService)
        {   //TODO: exception handling
            var requestStream = environment["owin.RequestBody"] as Stream;
            var headers = environment["owin.RequestHeaders"] as IDictionary<string, string[]>;

            var serviceName = soapService.ParseWebServiceClassName(headers);
            var request = soapService.ParseSoapRequest(requestStream, serviceName, supportedServiceInfos);

            var returnValue = soapService.Invoke(request);
            var serialize = soapService.CreateResponseBody(returnValue, request.WebService);
            var memoryStream = new MemoryStream();
            using(StreamWriter writer = new StreamWriter(memoryStream))
            {
                writer.Write(serialize);
                memoryStream.CopyTo((Stream)environment["owin.ResponseBody"]);
            }   
        }
    }
}
