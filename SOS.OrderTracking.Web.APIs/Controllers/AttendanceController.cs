using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SOS.OrderTracking.Web.APIs.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        [HttpPost]
        public IActionResult Checkin(AttendanceModel attendanceModel)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult CheckOut(AttendanceModel attendanceModel)
        {
            return Ok();
        }
    }

    public class AttendanceModel
    {
        public string EmployeeCode { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
