using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;

namespace SOS.OrderTracking.Web.Shared.StaticClasses
{
    public static class CurrencyHelper
    {
        public static int? CalculateAmount(CitDenominationViewModel CitDenominationViewModel, DenominationType denominationType)
        {
            int mf = 1;
            if (denominationType == DenominationType.Packets) mf = 100;
            else if (denominationType == DenominationType.Bundles) mf = 1000;

            var TotalAmount =
                              (CitDenominationViewModel.Currency1x.GetValueOrDefault() * 1 * mf)
                              + (CitDenominationViewModel.Currency2x.GetValueOrDefault() * 2 * mf)
                              + (CitDenominationViewModel.Currency5x.GetValueOrDefault() * 5 * mf)
                              + (CitDenominationViewModel.Currency10x.GetValueOrDefault() * 10 * mf)
                              + (CitDenominationViewModel.Currency20x.GetValueOrDefault() * 20 * mf)
                              + (CitDenominationViewModel.Currency50x.GetValueOrDefault() * 50 * mf)
                              + (CitDenominationViewModel.Currency75x.GetValueOrDefault() * 75 * mf)
                              + (CitDenominationViewModel.Currency100x.GetValueOrDefault() * 100 * mf)
                              + (CitDenominationViewModel.Currency500x.GetValueOrDefault() * 500 * mf)
                              + (CitDenominationViewModel.Currency1000x.GetValueOrDefault() * 1000 * mf)
                              + (CitDenominationViewModel.Currency5000x.GetValueOrDefault() * 5000 * mf);
            return TotalAmount;

        }
        public static int? CalculatePrizeBondAmount(CitDenominationViewModel CitDenominationViewModel, DenominationType denominationType)
        {
            int mf = 1;
            if (denominationType == DenominationType.Packets) mf = 100;
            else if (denominationType == DenominationType.Bundles) mf = 1000;

            var TotalAmount =
                              (CitDenominationViewModel.Currency100x.GetValueOrDefault() > 0 ? ((CitDenominationViewModel.Currency100x.GetValueOrDefault() * 100 * mf) + CitDenominationViewModel.PrizeMoney100x) : 0)
                              + (CitDenominationViewModel.Currency200x.GetValueOrDefault() > 0 ? ((CitDenominationViewModel.Currency200x.GetValueOrDefault() * 200 * mf) + CitDenominationViewModel.PrizeMoney200x) : 0)
                              + (CitDenominationViewModel.Currency750x.GetValueOrDefault() > 0 ? ((CitDenominationViewModel.Currency750x.GetValueOrDefault() * 750 * mf) + CitDenominationViewModel.PrizeMoney750x) : 0)
                              + (CitDenominationViewModel.Currency1500x.GetValueOrDefault() > 0 ? ((CitDenominationViewModel.Currency1500x.GetValueOrDefault() * 1500 * mf) + CitDenominationViewModel.PrizeMoney1500x) : 0)
                              + (CitDenominationViewModel.Currency7500x.GetValueOrDefault() > 0 ? ((CitDenominationViewModel.Currency7500x.GetValueOrDefault() * 7500 * mf) + CitDenominationViewModel.PrizeMoney7500x) : 0)
                              + (CitDenominationViewModel.Currency15000x.GetValueOrDefault() > 0 ? ((CitDenominationViewModel.Currency15000x.GetValueOrDefault() * 15000 * mf) + CitDenominationViewModel.PrizeMoney15000x) : 0)
                              + (CitDenominationViewModel.Currency25000x.GetValueOrDefault() > 0 ? ((CitDenominationViewModel.Currency25000x.GetValueOrDefault() * 25000 * mf) + CitDenominationViewModel.PrizeMoney25000x) : 0)
                              + (CitDenominationViewModel.Currency40000x.GetValueOrDefault() > 0 ? ((CitDenominationViewModel.Currency40000x.GetValueOrDefault() * 40000 * mf) + CitDenominationViewModel.PrizeMoney40000x) : 0);
            return TotalAmount;

        }
        public static int? PrizeBondFormula(int? Currency, int? CurrencyAmount, int? PrizeMoney, DenominationType denominationType)
        {
            if (CurrencyAmount < 1) return 0;

            int mf = 1;
            if (denominationType == DenominationType.Packets) mf = 100;
            else if (denominationType == DenominationType.Bundles) mf = 1000;

            return (((Currency * CurrencyAmount * mf) + PrizeMoney));

        }


        public static int? CalculateAmount(DenominationBaseViewModel denominationViewModel, DenominationType denominationType)
        {

            int mf = 1;
            if (denominationType == DenominationType.Packets)
            {
                mf = 100;
            }
            else if (denominationType == DenominationType.Bundles)
            {
                mf = 1000;
            }

            var TotalAmount = (denominationViewModel.Currency10x.GetValueOrDefault() * 10 * mf)
                              + (denominationViewModel.Currency20x.GetValueOrDefault() * 20 * mf)
                              + (denominationViewModel.Currency50x.GetValueOrDefault() * 50 * mf)
                              + (denominationViewModel.Currency75x.GetValueOrDefault() * 75 * mf)
                              + (denominationViewModel.Currency100x.GetValueOrDefault() * 100 * mf)
                              + (denominationViewModel.Currency500x.GetValueOrDefault() * 500 * mf)
                              + (denominationViewModel.Currency1000x.GetValueOrDefault() * 1000 * mf)
                              + (denominationViewModel.Currency5000x.GetValueOrDefault() * 5000 * mf);


            return TotalAmount;

        }

        public static int? CalculateAmountOneByOne(int? currency, int currencyMultiplier, DenominationType denominationType)
        {
            int mf = 1;

            if (denominationType == DenominationType.Packets)
            {
                mf = 100;
            }
            else if (denominationType == DenominationType.Bundles)
            {
                mf = 1000;
            }

            int? totalAmount = (currency.GetValueOrDefault() * currencyMultiplier * mf);

            return totalAmount.GetValueOrDefault();

        }
        public static string AmountInWords(int number)
        {
            bool isUK = true;
            if (number == 0) return "Zero";
            string and = isUK ? "and " : ""; // deals with UK or US numbering
            //if (number == -2147483648) return "Minus Two Billion One Hundred " + and +
            //"Forty Seven Million Four Hundred " + and + "Eighty Three Thousand " +
            //"Six Hundred " + and + "Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
            string[] words3 = { "Thousand ", "Million ", "Billion " };
            num[0] = number % 1000;           // units
            num[1] = number / 1000;
            num[2] = number / 1000000;
            num[1] = num[1] - 1000 * num[2];  // thousands
            num[3] = number / 1000000000;     // billions
            num[2] = num[2] - 1000 * num[3];  // millions
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10;              // ones
                t = num[i] / 10;
                h = num[i] / 100;             // hundreds
                t = t - 10 * h;               // tens

                if (h > 0)
                    sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (i < first)  //if (h > 0 || i < first) 
                        sb.Append(and);
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0)
                    sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }
    }
}
