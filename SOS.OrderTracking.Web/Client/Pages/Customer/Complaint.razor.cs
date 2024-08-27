using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Customer
{
    public partial class Complaint
    {
        private int ratingValue;

        public int RatingValue
        {
            get { return ratingValue; }
            set
            {
                ratingValue = value;

                NotifyPropertyChanged();
            }
        }

        private string category;

        public string Category
        {
            get { return category; }
            set { category = value;

                NotifyPropertyChanged();
            }
        }

        public Complaint()
        {
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(RatingValue) || q.PropertyName == nameof(Category))
                {
                    AdditionalParams = $"&RatingValue={RatingValue}&Category={Category}";
                    await LoadItems(true);
                }
            };
        }
    }
}
