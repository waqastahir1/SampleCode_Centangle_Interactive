using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using BoldReports.Web.ReportViewer;
using Microsoft.AspNetCore.Hosting;
using SOS.OrderTracking.Web.Server.Models;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Shared.Enums;
using System.Text.RegularExpressions;
using System.Text;
using BoldReports.Writer;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.StaticClasses;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data.Models;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Controller]
    [Route("pages/[controller]/[action]")]
    public class ConsignmentReceiptController : Controller
    {
        //Report Viewer requires a memory cache to store the information of consecutive client request and have the rendered report viewer information in server
        // Report viewer requires a memory cache to store the information of consecutive client request and
        // have the rendered Report Viewer information in server.
        private Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;
        private readonly AppDbContext context;
        // IHostingEnvironment used with sample to get the application data from wwwroot.
        private IWebHostEnvironment _hostingEnvironment;

        // Post action to process the report from server based json parameters and send the result back to the client.
        public ConsignmentReceiptController(Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache,
            IWebHostEnvironment hostingEnvironment, AppDbContext appDbContext)
        {
            _cache = memoryCache;
            _hostingEnvironment = hostingEnvironment;
            context = appDbContext;
        }

        
        public IActionResult Index(int id)
        {
            return View(id);
        }

        [HttpGet]
        public IActionResult Export(string writerFormat, int consignmentId)
        {

            //var consignmentId = Convert.ToInt32(reportOption.ControlId.Split('-').Last());
            #region Actual Data
            var model = (from c in context.Consignments
                         join f in context.Parties on c.FromPartyId equals f.Id
                         join t in context.Parties on c.ToPartyId equals t.Id
                         join s in context.Parties on c.MainCustomerId equals s.Id
                         join b in context.Parties on c.BillBranchId equals b.Id
                         join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                         from n in context.Denominations.Where(x => x.ConsignmentId == c.Id).DefaultIfEmpty()
                         from cr in context.Parties.Where(x => x.Id == d.CrewId).DefaultIfEmpty()
                         where c.Id == consignmentId
                         select new ConsignmentReportViewModel()
                         {
                             AC_Code = c.Id.ToString(),
                             Amount = c.Amount.ToString("N0"),
                             AmountInWords = c.CurrencySymbol.ToString() + " " + CurrencyHelper.AmountInWords(c.Amount),
                             AmountInWordsPKR = "PKR " + CurrencyHelper.AmountInWords(c.AmountPKR),
                             AmountType = n.DenominationType.ToString(),
                             Date = DateTime.Now.ToString("dd/MM/yyyy"),
                             PickupBranch = f.ShortName + " - " + f.FormalName,
                             DeliveryBranch = t.ShortName + " - " + t.FormalName,
                             FirstCopyName = f.ShortName,
                             ThirdCopyName = t.ShortName,
                             CurrencySymbol = c.CurrencySymbol.ToString(),
                             CurrencySymbol_ = c.CurrencySymbol,
                             ConsignedByName1 = c.CreatedBy,
                             ConsignedByName2 = c.ApprovedBy, 
                             AcceptedByName1 = (from r in context.PartyRelationships
                                                join p in context.Parties on r.FromPartyId equals p.Id
                                                where r.ToPartyId == d.CrewId
                                                && r.FromPartyRole == RoleType.CheifCrewAgent && r.IsActive
                                                select p).FirstOrDefault().FormalName,
                             AcceptedByName2 = (from r in context.PartyRelationships
                                                join p in context.Parties on r.FromPartyId equals p.Id
                                                where r.ToPartyId == d.CrewId && r.IsActive
                                                && r.FromPartyRole == RoleType.AssistantCrewAgent
                                                select p).FirstOrDefault().FormalName,
                             PickupTime = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.InTransit).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm") == "01/01/0001 00:00" 
                             ? "" : context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.InTransit).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm"),
                             DeliveryTime = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.Delivered).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm") == "01/01/0001 00:00"
                             ? "" : context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.Delivered).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm"),
                             SealNos = context.ShipmentSealCodes.Where(x => x.ConsignmentId == c.Id).Select(x => x.SealCode).ToList(),
                             VehicleNo = context.AssetAllocations.FirstOrDefault(x => x.PartyId == d.CrewId).Asset.Description,
                             CustomerToBeBilledName = b.ShortName + " - " + b.FormalName,
                             CustomerToBeBilled = b.ShortName + " - " + b.FormalName,
                             ShipmentRecieptNo = c.ShipmentCode,
                             Currency5000x = n.Currency5000x,
                             Currency1000x = n.Currency1000x,
                             Currency500x = n.Currency500x,
                             Currency100x = n.Currency100x,
                             Currency50x = n.Currency50x,
                             Currency75x = n.Currency75x,
                             Currency20x = n.Currency20x,
                             Currency10x = n.Currency10x,
                             Currency5x = n.Currency5x,
                             Currency2x = n.Currency2x,
                             Currency1x = n.Currency1x,
                             Currency5000xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency5000x, 5000, n.DenominationType),
                             Currency1000xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency1000x, 1000, n.DenominationType),
                             Currency500xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency500x, 500, n.DenominationType),
                             Currency100xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency100x, 100, n.DenominationType),
                             Currency75xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency75x, 75, n.DenominationType),
                             Currency50xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency50x, 50, n.DenominationType),
                             Currency20xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency20x, 20, n.DenominationType),
                             Currency10xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency10x, 10, n.DenominationType),
                             Currency5xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency5x, 5, n.DenominationType),
                             Currency2xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency2x, 2, n.DenominationType),
                             Currency1xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency1x, 1, n.DenominationType),
                             Comments = c.Comments,
                             Valueables = c.Valueables,
                             NoOfBags = c.NoOfBags,
                             SealedBags = c.SealedBags
                         }).FirstOrDefault();

       

            var comments = model.Comments != null ? JsonConvert.DeserializeObject<List<ShipmentComment>>(model.Comments).ToList() : new List<ShipmentComment>();
            StringBuilder concatedComment = new();
            foreach(var comment in comments)
            {
                concatedComment.Append(comment.Description + ",");
            }
            model.Comments = concatedComment.ToString();
            var reportName = @"\Reports\ConsignmentReceipt.rdlc";
            switch (model.CurrencySymbol_)
            {
                case CurrencySymbol.USD:
                case CurrencySymbol.EURO:
                    model.AmountInWords += $"\n\r{model.AmountInWordsPKR}";
                    break;

                case CurrencySymbol.MixCurrency:
                case CurrencySymbol.PrizeBond:
                case CurrencySymbol.Other:
                    model.AmountInWords = model.AmountInWordsPKR;
                    reportName = @"\Reports\ConsignmentReceiptMulti.rdlc";
                    break;
            }

            string basePath = _hostingEnvironment.WebRootPath;
            // Here, we have loaded the Product List.rdlc sample report from application the Resources folder.
            FileStream reportStream = new FileStream(basePath + reportName, FileMode.Open, FileAccess.Read);
            BoldReports.Writer.ReportWriter writer = new BoldReports.Writer.ReportWriter(reportStream);
            writer.ReportProcessingMode = (BoldReports.Writer.ProcessingMode)BoldReports.ReportViewerEnums.ProcessingMode.Local;


            int index = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var sealNo in model.SealNos)
            {
                index++;
                if (index == model.SealNos.Count())
                    sb.Append(sealNo);
                else
                    sb.Append(sealNo + ",");
            }
            model.SealNo = sb.ToString();
            model.NoOfSeals = model.SealNos.Count();
            #endregion
             
            List<ConsignmentReportViewModel> datasource = new List<ConsignmentReportViewModel>();
            datasource.Add(model);

            // Pass the dataset collection for report
            writer.DataSources.Clear();
            writer.DataSources.Add(new BoldReports.Web.ReportDataSource { Name = "ConsignmentDataSet", Value = datasource });

            string fileName = null;
            WriterFormat format;
            string type = null;

            if (writerFormat == "PDF")
            {
                fileName = $"CR-{model.ShipmentRecieptNo}.pdf";
                type = "pdf";
                format = WriterFormat.PDF;
            }
            else if (writerFormat == "Word")
            {
                fileName = $"CR-{model.ShipmentRecieptNo}.doc";
                type = "doc";
                format = WriterFormat.Word;
            }
            else if (writerFormat == "CSV")
            {
                fileName = $"CR-{model.ShipmentRecieptNo}.csv";
                type = "csv";
                format = WriterFormat.CSV;
            }
            else
            {
                fileName = "ConsignmentReceipt.xls";
                type = "xls";
                format = WriterFormat.Excel;
            }

            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, format);

            // Download the generated export document to the client side.
            memoryStream.Position = 0;
            memoryStream.Position = 0;
            return Ok(new FileViewModel()
            {
                Data = memoryStream.ToArray()
            });
            //FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/" + type);
            //fileStreamResult.FileDownloadName = fileName;
            //return fileStreamResult;
        }



    }
}
