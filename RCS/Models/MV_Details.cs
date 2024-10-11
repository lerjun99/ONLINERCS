namespace RCS.Models
{
    public class MV_Details
    {
        public string TXN_ID { get; set; }
        public string MV_FILE_NO { get; set; }
        public string PLATE_NO { get; set; }
        public string ENGINE_NO { get; set; }
        public string CHASIS_NO { get; set; }
        public string MAKE { get; set; }
        public string SERIES { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string Total { get; set; }
        public List<Fees> Fees { get; set; }

    }
}
