using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Custom.HttpMessageHandler.Controllers
{
    public class HomeController : ApiController
    {
        [HttpPost]
        public string GetMessage()
        {
            ClaimsPrincipal principal = User as ClaimsPrincipal;
            var name = principal.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault();
            return "Success";
        }
    }
}
