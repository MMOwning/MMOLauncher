using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace MMOwningLauncher.Web.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get[@"/"] = parameters =>
            {
                return Response.AsFile("app/index.html", "text/html");
            };
        }
    }
}
