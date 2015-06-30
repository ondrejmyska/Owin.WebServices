using Owin.WebServices.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Owin.WebServices
{
    public interface ISoapService
    {
        /// <summary>
        /// Parse webservice class name from headers. It could be done via parse POST or GET header
        /// </summary>
        /// <param name="headers">HTTP headers</param>
        /// <returns>Method name</returns>
        string ParseWebServiceClassName(IDictionary<string, string[]> headers);
        SoapRequest ParseSoapRequest(Stream request, string webServiceName, IDictionary<string, WebServiceInfo> supportedWebServices);
        object Invoke(SoapRequest soapRequest/*, IDictionary<string, WebServiceInfo> webServices*/);
        string CreateResponseBody<T>(T returnValue, WebServiceInfo webService);
        /// <summary>
        /// Serialize exception which could be raised during processing request.
        /// </summary>
        /// <param name="e">Exception risen while web request has been proceeded.</param>
        /// <returns></returns>
        string CreateResponseErrorBody(Exception e);
    }
}
