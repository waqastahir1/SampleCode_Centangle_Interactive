using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public abstract class CrudBaseController<TFormViewModel, TListViewModel,TFiltersViewModel , TKey> : ControllerBase
    {
        protected AppDbContext context;

        public CrudBaseController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public abstract Task<IndexViewModel<TListViewModel>> GetPage([FromQuery] TFiltersViewModel vm);

        [HttpGet]
        public abstract Task<TFormViewModel> Get(TKey id);

        [HttpPost]
        public abstract Task<IActionResult> Post([FromBody] TFormViewModel selectedItem);

        protected IndexViewModel<TListViewModel> Page(IEnumerable<TListViewModel> items, int totalRows)
        {
            return new IndexViewModel<TListViewModel>(items.ToList(), totalRows);
        }
    }
}
