using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Owin.WebServices.Entities
{
    public class WebServiceInfo
    {
        public Type WebServiceType { get; private set; }
        public IDictionary<string, MethodInfo> WebMethods { get; private set; }
        public string WebServiceNamespace { get; private set; }

        public WebServiceInfo(Type webServiceType, string webServiceNamespace,
            IDictionary<string, MethodInfo> webMethods)
        {
            WebServiceType = webServiceType;
            WebServiceNamespace = webServiceNamespace;
            WebMethods = webMethods;
        }
    }
}
