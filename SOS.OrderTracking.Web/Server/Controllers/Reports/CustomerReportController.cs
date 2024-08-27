using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BoldReports.Writer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Common.GBMS.Models;
using SOS.OrderTracking.Web.Server.Models;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Reports;
using SOS.OrderTracking.Web.Shared.StaticClasses;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Reports;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class CustomerReportController : ControllerBase, ICustomerReportService
    {
        //Report Viewer requires a memory cache to store the information of consecutive client request and have the rendered report viewer information in server
        // Report viewer requires a memory cache to store the information of consecutive client request and
        // have the rendered Report Viewer information in server.
        private Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;
        private readonly AppDbContext context;
        // IHostingEnvironment used with sample to get the application data from wwwroot.
        private IWebHostEnvironment _hostingEnvironment;

        // Post action to process the report from server based json parameters and send the result back to the client.
        public CustomerReportController(Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache,
            IWebHostEnvironment hostingEnvironment, AppDbContext appDbContext)
        {
            _cache = memoryCache;
            _hostingEnvironment = hostingEnvironment;
            context = appDbContext;
        }


        [HttpGet]
        public async Task<IndexViewModel<CustomerReportViewModel>> GetPageAsync([FromQuery] CustomerReportIndexViewModel vm)
        {
         
            if (vm.FromDate == null || vm.ThruDate == null)
            {
                vm.FromDate = DateTime.Now;
                vm.ThruDate = DateTime.Now;
            }
            var query =   GetConsignments(vm.BillBranchId, vm.FromDate.GetValueOrDefault(), vm.ThruDate.GetValueOrDefault(), vm.ConsignmentStatus,
                vm.RegionId.GetValueOrDefault(), vm.SubRegionId.GetValueOrDefault(), vm.StationId.GetValueOrDefault());
             
            var totalRows = query.Count();

            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToListAsync();
            items.ForEach(p =>
            {
                if (p.PickupTime == "01-01-01 00:00")
                    p.PickupTime = string.Empty;
                if (p.DeliveryTime == "01-01-01 00:00")
                    p.DeliveryTime = string.Empty;
            });
            return new IndexViewModel<CustomerReportViewModel>(items, totalRows);
        }



        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] CustomerReportIndexViewModel vm)
        {
            string basePath = _hostingEnvironment.WebRootPath;
            // Here, we have loaded the Product List.rdlc sample report from application the Resources folder.
            FileStream reportStream = new FileStream(basePath + @"\Reports\CustomerReport.rdl", FileMode.Open, FileAccess.Read);
            ReportWriter writer = new ReportWriter(reportStream);
            writer.ReportProcessingMode = ProcessingMode.Local;
            #region Actual Data
            var query =  GetConsignments(vm.BillBranchId, vm.FromDate.GetValueOrDefault(), vm.ThruDate.GetValueOrDefault(), vm.ConsignmentStatus,
                vm.RegionId.GetValueOrDefault(), vm.SubRegionId.GetValueOrDefault(), vm.StationId.GetValueOrDefault());

            var datasource = await query.ToListAsync();
            datasource.ForEach(p =>
            {
                if (p.PickupTime == "01-01-01 00:00")
                    p.PickupTime = string.Empty;

                if(p.DeliveryTime == "01-01-01 00:00")
                    p.DeliveryTime = string.Empty;

                p.CreatedByOrgName = User.Identity.Name;
                p.Dedicated = p.ConsignmentStatus.ToString();
                p.ShipmentMedium = p.ShipmentType.ToString();

                p.CollectionQR = p.CollectionQR == "True" ? "Y" : "N";
                p.DeliveryQR = p.DeliveryQR == "True" ? "Y" : "N";

            });
            #endregion 
            writer.DataSources.Add(new BoldReports.Web.ReportDataSource { Name = "CustomerDataSet", Value = datasource });

            string fileName = null;
            WriterFormat format;
            string type = null;

            if (vm.WriterFormat == "pdf")
            {
                fileName = $"CR-{"CustomerReport"}.pdf";
                type = "pdf";
                format = WriterFormat.PDF;
            }
            else if (vm.WriterFormat == "word")
            {
                fileName = $"CR-{"CustomerReport"}.doc";
                type = "doc";
                format = WriterFormat.Word;
            }
            else if (vm.WriterFormat == "csv")
            {
                fileName = $"CR-{"CustomerReport"}.csv";
                type = "csv";
                format = WriterFormat.CSV;
            }
            else
            {
                fileName = "CustomerReport.xls";
                type = "xls";
                format = WriterFormat.Excel;
            }

            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, format);

            // Download the generated export document to the client side.
            memoryStream.Position = 0;
            return Ok(new FileViewModel()
            {
                Data = memoryStream.ToArray()
            });
            //FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/" + type);
            //fileStreamResult.FileDownloadName = fileName;
            //return fileStreamResult;
        }


        private  IQueryable<CustomerReportViewModel> GetConsignments(int billBranchId, DateTime fromDate, 
            DateTime thruDate, ConsignmentStatus ConsignmentStatus, int regionId, int subRegionId, int stationId)
        {
            var partyId = context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name)?.PartyId;
           
            var query = (from c in context.Consignments
                         join f in context.Parties on c.FromPartyId equals f.Id
                         join t in context.Parties on c.ToPartyId equals t.Id
                         join b in context.Parties on c.BillBranchId equals b.Id
                         from d in context.ConsignmentDeliveries.Where(x => x.ConsignmentId == c.Id).DefaultIfEmpty()
                         where (c.FromPartyId == partyId || c.ToPartyId == partyId || c.BillBranchId == partyId ||
                        c.MainCustomerId == partyId)
                         && c.CreatedAt.Date >= fromDate.Date && c.CreatedAt.Date <= thruDate.Date
                         && (regionId == 0 || b.RegionId == regionId)
                         && (subRegionId == 0 || b.SubregionId == subRegionId)
                         && (stationId == 0 || b.StationId == stationId)
                         && c.ConsignmentStateType == ConsignmentDeliveryState.Delivered
                         orderby c.DueTime
                         select new CustomerReportViewModel()
                         {
                             ShipmentCode = c.ShipmentCode,
                             Amount = c.Amount.ToString("N0"),
                             //AmountInRupees = c.AmountPKR,
                             AmountInWords = CurrencyHelper.AmountInWords(c.Amount),
                             PickupBranch = f.ShortName + " - " + f.FormalName,
                             DeliveryBranch = t.ShortName + " - " + t.FormalName,
                             CurrentDate = DateTime.Now.ToString("dd/MM/yyyy"),
                             BillTo = b.ShortName,
                             PickupTime = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.InTransit).TimeStamp.GetValueOrDefault().ToString("dd/MM/yy hh:mm tt"),
                             DeliveryTime = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.Delivered).TimeStamp.GetValueOrDefault().ToString("dd/MM/yy hh:mm tt"),
                             VehicleNo = context.AssetAllocations.FirstOrDefault(x => x.PartyId == d.CrewId).Asset.Description,
                             ConsignmentStatus = c.ConsignmentStatus,
                             ShipmentType = c.ShipmentType.ToString(),
                             OrderBy = c.CreatedBy,
                             OrderAt = c.DueTime.ToString("dd/MM/yy hh:mm tt"),
                             CollectionQR = (context.ConsignmentDeliveries.FirstOrDefault(x=>x.ConsignmentId == c.Id).CollectionMode == 1).ToString(),
                             DeliveryQR = (context.ConsignmentDeliveries.OrderByDescending(x=>x.Id).FirstOrDefault(x => x.ConsignmentId == c.Id).DeliveryMode == 1).ToString()
                         });
            if(ConsignmentStatus > ConsignmentStatus.All)
            {
                query = query.Where(x => x.ConsignmentStatus == ConsignmentStatus);
            }
            return query;
        }

        public Task<CustomerReportViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> PostAsync(CustomerReportViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
