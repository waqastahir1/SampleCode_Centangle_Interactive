using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SOS.OrderTracking.Web.Common.Data.Services
{
    public class SequenceService
    {
        private readonly AppDbContext _context;

        public static readonly string CITOrdersSeq = "CITOrdersSeq";

        public static readonly string CITDeliveriesSeq = "CITDeliveriesSeq";

        public static readonly string CPCOrdersSeq = "CPCOrdersSeq";

        public static readonly string CPCTransactionsSeq = "CPCTransactionsSeq";


        public static readonly string ATMROrdersSeq = "ATMROrdersSeq";

        public static readonly string PartiesSeq = "PartiesSeq";
        public static readonly string IntraPartyDistanceSeq = "IntraPartyDistanceSeq";

        //public static readonly string LocationsSeq = "LocationsSeq";

        /// <summary>
        /// Used for assets and asset-allocations table
        /// </summary>
        public static readonly string AssetsSeq = "AssetsSeq";

        public static readonly string NotificationSeq = "NotificationSeq";

        public static readonly string DedicatedVehiclesSeq = "DedicatedVehiclesSeq";

        public SequenceService(AppDbContext context)
        {
            _context = context;
        }

        private int GetNextValueOfSequence(string sequenceName)
        {
            SqlParameter result = new SqlParameter("@result", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            _context.Database.ExecuteSqlRaw($"SELECT @result = (NEXT VALUE FOR dbo.{sequenceName})", result);

            return (int)result.Value;
        }

        public int GetNextCitOrdersSequence()
        {
            return GetNextValueOfSequence(CITOrdersSeq);
        }
        public int GetNextCPCSequence()
        {
            return GetNextValueOfSequence(CPCOrdersSeq);
        }



        public int GetNextDeliverySequence()
        {
            return GetNextValueOfSequence(CITDeliveriesSeq);
        }

        public int GetNextDenominationSequence()
        {
            return GetNextValueOfSequence(ATMROrdersSeq);
        }


        /// <summary>
        /// User for Parties and Party Relationships
        /// </summary>
        /// <returns></returns>
        public int GetNextPartiesSequence()
        {
            return GetNextValueOfSequence(PartiesSeq);
        }

        public int GetNextCommonSequence()
        {
            return GetNextValueOfSequence(AssetsSeq);
        }

        /// <summary>
        /// Used only for notifications
        /// </summary>
        /// <returns></returns>
        public int GetNextNotificationSequence()
        {
            return GetNextValueOfSequence(NotificationSeq);
        }

        public int GetNextIntraPartyDistanceSequence()
        {
            return GetNextValueOfSequence(IntraPartyDistanceSeq);
        }
    }
}
