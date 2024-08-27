using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SOS.OrderTracking.Web.Common.Exceptions;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class ErrorsController : ControllerBase
    {
        public IActionResult Index()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error; // Your exception

            if (exception is BadRequestException || exception is InvalidOperationException)
            {
                return BadRequest(exception.Message);
            }
            else if (exception is NotFoundException)
            {
                return NotFound(exception.Message);
            }
            else if (exception is UnAuthorizedException)
            {
                return Unauthorized(exception.Message);
            }

            throw exception;
        }
    } 
     
}
