using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Services;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class PeopleController : ControllerBase
    {
        

        private readonly AppDbContext context;

        private readonly EmployeeService peopleService;

        public PeopleController(AppDbContext appDbContext,
           
           EmployeeService peopleService)
        {
            context = appDbContext;
            
            this.peopleService = peopleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAtmManagers(int id)
        {
            //var query = await peopleService.GetAtmManager(id).ToListAsync();
            return Ok();
        }
    }
}
