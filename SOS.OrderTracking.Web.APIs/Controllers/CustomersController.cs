using Microsoft.AspNetCore.Mvc;
using SOS.OrderTracking.Web.Common.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SOS.OrderTracking.Web.APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CustomersController : Controller
    { 
        public CustomersController(AppDbContext context)
        { 
            _context = context;
        }

        private readonly AppDbContext _context;

       
    }
}
