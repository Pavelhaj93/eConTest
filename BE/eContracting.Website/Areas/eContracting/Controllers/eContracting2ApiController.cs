using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class eContracting2ApiController : ApiController
    {
        public IHttpActionResult GetFiles()
        {
            return this.NotFound();
        }
    }
}
