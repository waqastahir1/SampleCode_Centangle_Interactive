using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Components
{
    public partial class PaginationStrip
    {
        [Parameter] public int TotalPages { get; set; }

        [Parameter]  public int TotalRows { get; set; }

        private void UpdatePage(int currentIndex)
        {
            Value = currentIndex;
            ValueChanged.InvokeAsync(Value);
        }

    }
     
}
