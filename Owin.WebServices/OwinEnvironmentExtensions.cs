using Owin.WebServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Owin.WebServices
{
    public static class OwinEnvironmentExtensions
    {
        const string soap11MimeType = "text/xml";
        const string soap12MimeType = "application/soap+xml";
        const string soapAction = "SOAPAction";

        public static SoapVersion GetSoapVersion(this string[] contentTypeValues)
        {
            foreach (var value in contentTypeValues)
            {
                if (value.Contains(soap11MimeType) && value.Contains(soapAction))
                    return SoapVersion.Version11;
                else if (contentTypeValues.Contains(soap12MimeType))
                    return SoapVersion.Version12;
            }
            return SoapVersion.None;
        }
    }
}
