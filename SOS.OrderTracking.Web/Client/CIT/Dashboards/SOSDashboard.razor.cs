using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.CIT.Dashboards
{
    public partial class SOSDashboard
    {

        public List<double> ShipmentWiseChartSeries { get; set; }
        public List<double> RegionWiseChartSeries { get; set; }
        public List<double> CustomerWiseChartSeries { get; set; }
        public List<string> ShipmentWiseChartLabels { get; set; }
        public List<string> RegionWiseChartLabels { get; set; }
        public List<string> CustomerWiseChartLabels { get; set; }

        protected async override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            await RenderCharts();
        }
        private async Task RenderCharts()
        {
            var item = Items?.FirstOrDefault();
            if (item != null)
            {
                ShipmentWiseChartSeries = new List<double>()
                            {
                                 item.DomensticShipmentCount,
                                item.DedicatedShipmentCount,
                                 item.LocalShipmentCount,
                                     item.AtmShipmentCount
                             };
                ShipmentWiseChartLabels = new List<string>()
                                {
                                 "Domestic Shipments",
                                     "Dedicated Shipments",
                                 "Local Shipments",
                                 "Atm Shipments",
                             };
                RegionWiseChartSeries = new();
                RegionWiseChartLabels = new();
                if (item.RegionWiseShipmentsList.Count > 0)
                {
                    foreach (var regionWise in item.RegionWiseShipmentsList)
                    {
                        RegionWiseChartSeries.Add(regionWise.ShipmentsCount);
                        RegionWiseChartLabels.Add(regionWise.FormalName);
                    }
                }
                CustomerWiseChartSeries = new();
                CustomerWiseChartLabels = new();
                if (item.MainCustomerShipmentsList.Count > 0)
                {
                    foreach (var mainCust in item.MainCustomerShipmentsList)
                    {
                        CustomerWiseChartSeries.Add(mainCust.ShipmentsCount);
                        CustomerWiseChartLabels.Add(mainCust.FormalName);
                    }
                }
                await JSRuntime.InvokeVoidAsync("apexChart", ShipmentWiseChartSeries, ShipmentWiseChartLabels, "#shipmentWiseChart");
                await JSRuntime.InvokeVoidAsync("apexChart", RegionWiseChartSeries, RegionWiseChartLabels, "#regionWiseChart");
                await JSRuntime.InvokeVoidAsync("apexChart", CustomerWiseChartSeries, CustomerWiseChartLabels, "#customerWiseChart");

            }

        }

    }
}
