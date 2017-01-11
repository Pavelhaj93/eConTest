using eContracting.Kernel.Helpers;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class BaseController<T> : GlassController<T> where T : class
    {
    }
}
