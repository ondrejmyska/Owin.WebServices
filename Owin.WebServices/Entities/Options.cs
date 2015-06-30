using System;
using System.Collections.Generic;
using System.Linq;

namespace Owin.WebServices.Entities
{
    public class Options
    {
        public Func<IEnumerable<Type>> WebServiceTypesSelector { get; set; }
        public Func<Type, object> WebServiceCreator { get; set; }
        public bool EnableDetailErrorResponse { get; set; }
    }
}
