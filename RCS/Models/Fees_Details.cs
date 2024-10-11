using static RCS.ApplicationEntityModels.EntityModels;
using static RCS.Controllers.DashboardController;

namespace RCS.Models
{
    public class Fees_Details
    {
        public string TXN_ID { get; set; }
        public string TXN_CODE { get; set; }
        public string CUST_NAME { get; set; }
        public string RO_CODE { get; set; }
        public string DO_CODE { get; set; }
        public string STATUS { get; set; }
        public string OFFICE_CODE { get; set; }
        public string SETTLEMENT_TYPE { get; set; }
        public string TXN_DATE { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string TOTAL_DUE { get; set; }
        public List<TXN_CHARGES> TXN_CHARGES { get; set; }

    }
}
