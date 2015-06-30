using System;
using System.Collections.Generic;
using System.Linq;

namespace Owin.WebServices.Entities
{
    public class SoapRequest
    {
        public string MethodName { get; private set; }
        public object[] Arguments { get; private set; }
        public WebServiceInfo WebService {get; private set;}

        public SoapRequest(string methodName, WebServiceInfo webService, object[] arguments)
        {
            MethodName = methodName;
            WebService = webService;
            Arguments = arguments;
        }
    }
}
