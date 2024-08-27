using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{

    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class BaseController : ControllerBase
    {


        protected readonly AppDbContext context;


        public BaseController(AppDbContext appDbContext)
        {
            context = appDbContext;

        }

        
    }
}
