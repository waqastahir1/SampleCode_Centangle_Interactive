using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class AdditionalRequests
    {
        public override string ApiControllerName => "AdditionalRequest";

        private List<SelectListItem> Operations = new List<SelectListItem> {
         new SelectListItem() { Text = "Allocate", IntValue=2}
        ,new SelectListItem() { Text = "DeAllocate", IntValue=4}};

        private List<SelectListItem> typeOfRequest = new List<SelectListItem> {
         new SelectListItem() { Text = "Gaurds", IntValue=2}
        ,new SelectListItem() { Text = "Weapons", IntValue=8}
        ,new SelectListItem() { Text = "Aimunition", IntValue=16}};

    }
}
