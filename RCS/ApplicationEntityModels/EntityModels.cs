using Microsoft.EntityFrameworkCore;
using RCS.Models;

namespace RCS.ApplicationEntityModels
{
    public class EntityModels
    {
        public class TransactionPayments
        {
            public string? TXN_ID { get; set; }
            public string? PAYMENT_MODE_CODE { get; set; }
            public string? PAYMENT_MODE_DESC { get; set; }
            public string? TOTAL_ID { get; set; }
            public string? PROC_FEE_IND { get; set; }
            public string? CURR_CODE { get; set; }
            [Precision(10, 2)]
            public decimal? AMOUNT { get; set; }
            public string? CHEQUE_TYPE_CODE { get; set; }
            public string? CHEQUE_TYPE_DESC { get; set; }
            public DateTime? CHEQUE_DATE { get; set; }
            public string? REFERENCE_NO { get; set; }
            public string? BASE_CURR { get; set; }
            [Precision(7, 4)]
            public decimal? EXCH_RATE { get; set; }
            public string? REPORT_ENTRY_ID { get; set; }
            public string? CONFIRMATION_NO { get; set; }
            public string? BANK_CODE { get; set; }
        }
        public class OrDetailsVM
        {
            public string OR_NO { get; set; }
            public DateTime OR_DATE { get; set; }
            public string OFFICE_CODE { get; set; }
            public string OFFICE_NAME { get; set; }
            public string CUSTOMER_NAME { get; set; }
            public string CUSTOMER_ADDRESS { get; set; }
            public string TIN { get; set; }
            public string TXN_ID { get; set; }
            public string SEQ_INDICATOR { get; set; }
            public double OR_AMT { get; set; }
            public string AMT_IN_WORDS { get; set; }
            public string CASHIER_ID { get; set; }
            public string DO_CHIEF_ID { get; set; }
            public List<ORPaymentMode> ORPaymentModes { get; set; }
            public List<TransactionTxnCode> TransactionTxnCodes { get; set; }
            public List<PaidTransactionFee> PaidTransactionFees { get; set; }
            public MVRSPaymentDetails MVRSPaymentDetails { get; set; }
            public int EOR_PRINT_COPIES { get; set; }
        }

        public class ORPaymentMode
        {
            public string PAYMENT_MODE { get; set; }
            public string PAYMENT_MODE_DESC { get; set; }
            public string REFERENCE_NO { get; set; }
            public double PAYMENT_AMOUNT { get; set; }
        }

        public class TransactionTxnCode
        {
            public string TXN_CODE { get; set; }
        }

        public class PaidTransactionFee
        {
            public string FEE_CODE { get; set; }
            public string FEE_DESC { get; set; }
            public double FEE_AMOUNT { get; set; }
            public string FEE_TYPE { get; set; }
            public string SEQ_INDICATOR { get; set; }
        }

        public class MVRSPaymentDetails
        {
            public string MV_FILE_NO { get; set; }
            public string MV_DESCRIPTION { get; set; }
            public string PLATE_NO { get; set; }
            public string GROSS_WT { get; set; }
            public string OR_REMARKS { get; set; }
            public string AIRCON_REF_DETAILS { get; set; }
            public string REGISTRATION_VALIDITY { get; set; }
        }
        public class AlertMessage
        {
            public string? Message { get; set; }
            public string? Status { get; set; }

        }
        public class ApiResponseChecque
        {
            public BaseResult BaseResult { get; set; }
            public ChequeModel Data { get; set; }
        }
        public class ChequeModel
        {
            public string? CHEQUE_TYPE_CODE { get; set; }
            public string? CHEQUE_TYPE { get; set; }
            public string? BANK_CODE { get; set; }
            public string? BANK_NAME { get; set; }
            public string? BANK_CLASSIFICATION { get; set; }
            public string? BANK_MNEMONIC { get; set; }

        }
        public class PaymentResult
        {
            public string? TXN_ID { get; set; }
            public string? OR_NO { get; set; }

        }
      
        public class RequestParameter
        {
            public string? ParameterName { get; set; }
            public string? ParameterValue { get; set; }
            public string? ParameterType { get; set; }

        }
        public class TokenResult
        {
            public string? Token { get; set; }
            public string? Status { get; set; }
            public string? Error { get; set; }
        }
        public class MV_Response
        {
            public MV_Details BaseResult { get; set; }
            public string Data { get; set; }
        }
        public class jwtparam
        {
            public string? Username { get; set; }
            public string? AppCode { get; set; }
            public string? Roles { get; set; }

        }
        public class MVDetail
        {
            public string TXN_ID { get; set; }
            public string MV_FILE_NO { get; set; }
            public string PLATE_NO { get; set; }
            public string ENGINE_NO { get; set; }
            public string CHASSIS_NO { get; set; }
            public string MAKE { get; set; }
            public string SERIES { get; set; }
            public string CUSTOMER_ID { get; set; }
            public string CUSTOMER_NAME { get; set; }
        }
        public class TXN_HEADER
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
        }

        public class TXN_CHARGES
        {
            public string TXN_ID { get; set; }
            public string FEE_CODE { get; set; }
            public string FEE_DESC { get; set; }
            public string SEQ_INDICATOR { get; set; }
            public string BATCH_NO { get; set; }
            public string BATCH_FLAG { get; set; }
            public string CHARGED_AMT { get; set; }
            public string CURR_CODE { get; set; }
            public string WAIVED_INDICATOR { get; set; }
            public string OR_NO { get; set; }
            public string OR_DATE { get; set; }
            public string CASHIER_ID { get; set; }
            public string CASHIER_OFFICE { get; set; }
            public string TOTAL_ID { get; set; }
            public string PROC_FEE_IND { get; set; }
            public string RO_CODE { get; set; }
            public string DO_CODE { get; set; }
            public string OPMODE { get; set; }
            public string GL_PRODUCT_CODE { get; set; }
        }
        public class FeesModelList
        {
            public TXN_HEADER TXN_HEADER { get; set; }
            public List<TXN_CHARGES> TXN_CHARGES { get; set; }
        }
        public class Fee
        {
            public string FEE_DESC { get; set; }
            public decimal CHARGED_AMT { get; set; }
        }

        public class ModelList
        {
            public MVDetail MV_DETAIL { get; set; }
            public List<Fee> FEES { get; set; }
        }
        public class AppSettings
        {
            public Logging Logging { get; set; }
            public string AllowedHosts { get; set; }
            public IP IP { get; set; }
            public ApiUrls ApiUrls { get; set; }
        }

        public class Logging
        {
            public LogLevel LogLevel { get; set; }
        }

        public class LogLevel
        {
            public string Default { get; set; }
            public string MicrosoftAspNetCore { get; set; }
        }

        public class IP
        {
            public string IpAddress { get; set; }
            public string Jwt { get; set; }
        }

        public class ApiUrls
        {
            public string GetSettlementTypeList { get; set; }
            public string GetPaymentModelist { get; set; }
            public string GetCurrencyList { get; set; }
            public string GetCollectionTypeList { get; set; }
            public string GetORStatusList { get; set; }
            public string GetChequeTypeList { get; set; }
        }
    }
}
