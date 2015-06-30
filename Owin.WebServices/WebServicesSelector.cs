using Owin.WebServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Services;

namespace Owin.WebServices
{
    public static class WebServicesSelector
    {
        public static IDictionary<string, WebServiceInfo> CreateWebServicesInfos(IEnumerable<Type> webServiceTypes)
        {
            webServiceTypes.Where(t => t.BaseType == typeof(WebService)).ToList();
            var webServiceInfos = new Dictionary<string, WebServiceInfo>();
            foreach (var webServiceType in webServiceTypes)
            {
                var serviceAttribute = webServiceType.GetCustomAttribute(typeof(WebServiceAttribute))
                    as WebServiceAttribute;
                if (serviceAttribute == null)
                    continue;
                var methodInfos = webServiceType.GetMethods().
                    Where(m => m.GetCustomAttribute(typeof(WebMethodAttribute), false) != null).
                    ToDictionary(k => GetMethodName(k));

                var webserviceInfo = new WebServiceInfo(webServiceType, serviceAttribute.Namespace, methodInfos);
                webServiceInfos.Add(webServiceType.Name, webserviceInfo);
            }
            return webServiceInfos;
        }

        private static string GetMethodName(MethodInfo methodInfo)
        {
            var webMetodAttribute = methodInfo.GetCustomAttribute(typeof(WebMethodAttribute), true)
                as WebMethodAttribute;
            var methodCustomName = webMetodAttribute.MessageName;
            return String.IsNullOrEmpty(methodCustomName) ? methodInfo.Name : methodCustomName;
        }
    }
}
