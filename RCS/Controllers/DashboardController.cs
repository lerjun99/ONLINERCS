using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RCS.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using static RCS.Controllers.DashboardController;
using System.Collections;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using System.Globalization;
using Tools;
using System.Text.Json;
using System.Web.Razor.Parser.SyntaxTree;
using static RCS.ApplicationEntityModels.EntityModels;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace RCS.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly Connection _Ipsettings;
        public DashboardController(ILogger<DashboardController> logger, IOptions<Connection> ipsettings)
        {
            _logger = logger;
            _Ipsettings = ipsettings.Value;
        }
    
        public IActionResult Index()
        {
            string datefrom_res = "";
            string datefrom_wi_res = "";
            var datefrom = getDetails().GetAwaiter().GetResult().Where(a => a.SETTLEMENT_TYPE == "OP").OrderBy(a => a.TXN_DATE).FirstOrDefault();
            var datefrom_wi = getDetails().GetAwaiter().GetResult().Where(a => a.SETTLEMENT_TYPE == "WI").OrderBy(a => a.TXN_DATE).FirstOrDefault();
           
                datefrom_res = datefrom == null?  DateTime.Now.ToString("yyyy-MM-dd") : datefrom.TXN_DATE;
                datefrom_wi_res = datefrom_wi == null ?DateTime.Now.ToString("yyyy-MM-dd") : datefrom_wi.TXN_DATE;
            
            var model = new datefilter
            {
                datefrom = datefrom_res,
                datefrom_wi = datefrom_wi_res

            };
            return View(model);
        }
        public IActionResult AlertMessage(string message,string status)
        {
            var model = new AlertMessage
            {
                Message = message,
                Status = status
            };
            return PartialView("AlertMessage", model);

        }
        public IActionResult PrintOR(string ornum)
        {
            var result = getOR(ornum).GetAwaiter().GetResult().FirstOrDefault();
            if (result != null)
            {
                var model = new OrDetailsVM
                {
                    OR_NO = ornum,
                    OR_DATE = result.OR_DATE,
                    OFFICE_CODE = result.OFFICE_CODE,
                    OFFICE_NAME = result.OFFICE_NAME,
                    CUSTOMER_ADDRESS = result.CUSTOMER_ADDRESS,
                    CUSTOMER_NAME = result.CUSTOMER_NAME,
                    TIN = result.TIN,
                    TXN_ID = result.TXN_ID,
                    SEQ_INDICATOR = result.SEQ_INDICATOR,
                    OR_AMT = result.OR_AMT,
                    AMT_IN_WORDS = result.AMT_IN_WORDS,
                    CASHIER_ID = result.CASHIER_ID,
                    DO_CHIEF_ID = result.DO_CHIEF_ID,
                    ORPaymentModes = result.ORPaymentModes.ToList(),
                    TransactionTxnCodes = result.TransactionTxnCodes.ToList(),
                    PaidTransactionFees = result.PaidTransactionFees.ToList(),

                    // Adding MVRSPaymentDetails mapping
                    MVRSPaymentDetails = new MVRSPaymentDetails
                    {
                        MV_FILE_NO = result.MVRSPaymentDetails.MV_FILE_NO,
                        MV_DESCRIPTION = result.MVRSPaymentDetails.MV_DESCRIPTION,
                        PLATE_NO = result.MVRSPaymentDetails.PLATE_NO,
                        GROSS_WT = result.MVRSPaymentDetails.GROSS_WT,
                        OR_REMARKS = result.MVRSPaymentDetails.OR_REMARKS,
                        AIRCON_REF_DETAILS = result.MVRSPaymentDetails.AIRCON_REF_DETAILS,
                        REGISTRATION_VALIDITY = result.MVRSPaymentDetails.REGISTRATION_VALIDITY
                    }
                };

                return PartialView("PrintOR", model);
            }
            else
            {
                ViewBag.ErrorMessage = "No Data Found";
                return PartialView("PrintOR");
            }

        }
        public IActionResult Process(string transid)
        {
          
            var result = getFeespayment(transid).GetAwaiter().GetResult().FirstOrDefault();
            if (result != null)

            {
                
                var model = new Fees_Details
                {
                    TXN_ID = transid,
                    TXN_CODE = result.TXN_CODE,
                    CUST_NAME=result.CUST_NAME,
                    RO_CODE= result.RO_CODE,
                    DO_CODE = result.DO_CODE,
                    STATUS = result.STATUS,
                    OFFICE_CODE = result.OFFICE_CODE,
                    SETTLEMENT_TYPE = result.SETTLEMENT_TYPE,
                    TXN_DATE = result.TXN_DATE,
                    CUSTOMER_ID = result.CUSTOMER_ID,
                    TOTAL_DUE = result.TOTAL_DUE,

                    TXN_CHARGES = result.TXN_CHARGES.ToList()

                };
                var res = result.TXN_CHARGES.Where(a => a.FEE_DESC == "Comp Fee").FirstOrDefault();
                HttpContext.Session.SetString("ChargeAmount", res.CHARGED_AMT.ToString());
                HttpContext.Session.SetString("FeeDesc", res.FEE_DESC.ToString());
                ViewBag.ChargeAmount = HttpContext.Session.GetString("ChargeAmount");
                ViewBag.FeeDesc = HttpContext.Session.GetString("FeeDesc");
                return PartialView("Process", model);
            }
            else
            {
                ViewBag.ErrorMessage = "No Data Found";
                return RedirectToAction("Index", "Dashboard");
            }
        }
     
        public IActionResult OR_Print(string transid)
        {
            var result = getMV(transid).GetAwaiter().GetResult().FirstOrDefault();
            if(result != null)

            { 
               
                var model = new MV_Details
                {
                    TXN_ID = transid,
                    MV_FILE_NO = result.MV_FILE_NO,
                    CUSTOMER_ID = result.CUSTOMER_ID,
                    PLATE_NO = result.PLATE_NO,
                    ENGINE_NO = result.ENGINE_NO,
                    CHASIS_NO = result.CHASIS_NO,
                    MAKE = result.MAKE,
                    SERIES = result.SERIES,
                    CUSTOMER_NAME = result.CUSTOMER_NAME,
                    Total= result.Total,

                    Fees = result.Fees.ToList().Count != 0 ? result.Fees.ToList() : ViewBag.ErrorMessage = "No Data Found"

                };
                return PartialView("OR_Print", model);
            }
            else
            {
                ViewBag.ErrorMessage = "No Data Found";
                return PartialView("OR_Print");
            }
        
            
            
        
        }
        public async Task<List<ChequeModel>> GetChequeList()
        {

            var transcations = (dynamic)null;
            List<ChequeModel> response_result = new List<ChequeModel>();
            string res = "";
            var data = new List<RequestParameter>
                    {
                    new RequestParameter
                    {
                        ParameterName = "USER_ID",
                        ParameterValue = "RJERGUIZA",
                        ParameterType = "ST"
                    }

                    };
            List<TokenResult> tokenResults = GetTokenAsync().GetAwaiter().GetResult();
            HttpClient client = new HttpClient();
            var url = _Ipsettings.ipaddress + "/api/ChequeType/GetList";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new Token().generateJWT());
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync(url, content))
            {
                res = await response.Content.ReadAsStringAsync();
                var baseResult = JsonConvert.DeserializeObject<BaseResult>(res);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(res);
                var chequeList = JsonConvert.DeserializeObject<List<ChequeModel>>(apiResponse.Data);
                if (!string.IsNullOrEmpty(apiResponse.Data))
                {

                    // Output to verify
                    foreach (var item in chequeList)
                    {
                        //Console.WriteLine($"Transaction ID: {item.MV_DETAIL.TXN_ID}");
                        var items = new ChequeModel();
                        items.CHEQUE_TYPE_CODE =item.CHEQUE_TYPE_CODE ;
                        items.CHEQUE_TYPE = item.BANK_MNEMONIC + " - " + item.CHEQUE_TYPE+ " - "+ item.BANK_NAME;
                        items.BANK_CODE = item.BANK_CODE;
                        items.BANK_NAME = item.BANK_NAME;
                        items.BANK_CLASSIFICATION = item.BANK_CLASSIFICATION;
                        items.BANK_MNEMONIC = item.BANK_MNEMONIC;
                        response_result.Add(items);
                    }
                }

            }

            return response_result;
        }
        public async Task<List<OrDetailsVM>> getOR(string orid)
        {
            var transcations = (dynamic)null;
            List<OrDetailsVM> response_result = new List<OrDetailsVM>();
            string res = "";

            var data = new List<RequestParameter>
            {
                new RequestParameter
                {
                    ParameterName = "USER_ID",
                    ParameterValue = "puertorm",
                    ParameterType = "ST"
                },
                new RequestParameter
                {
                    ParameterName = "OR_NO",
                    ParameterValue = orid,
                    ParameterType = "ST"
                }
            };

            List<TokenResult> tokenResults = GetTokenAsync().GetAwaiter().GetResult();
            HttpClient client = new HttpClient();
            var url = _Ipsettings.ipaddress + "/api/OR/GetDetails";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);

            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            using (var response = await client.PostAsync(url, content))
            {
                res = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(res);

                if (!string.IsNullOrEmpty(apiResponse.Data))
                {
                    var json = apiResponse.Data;
                    var modelList = JsonConvert.DeserializeObject<OrDetailsVM>(json);
                    var modelListCollection = new List<OrDetailsVM> { modelList };

                    foreach (var item in modelListCollection)
                    {
                        var newItem = new OrDetailsVM
                        {
                            TXN_ID = item.TXN_ID+" - "+ item.SEQ_INDICATOR,
                            OR_NO = item.OR_NO,
                            OR_DATE = item.OR_DATE,
                            OFFICE_CODE = item.OFFICE_CODE,
                            OFFICE_NAME = item.OFFICE_NAME,
                            CUSTOMER_NAME = item.CUSTOMER_NAME,
                            CUSTOMER_ADDRESS = item.CUSTOMER_ADDRESS,
                            TIN = item.TIN,
                            SEQ_INDICATOR = item.SEQ_INDICATOR,
                            OR_AMT = item.OR_AMT,
                            AMT_IN_WORDS = item.AMT_IN_WORDS,
                            CASHIER_ID = item.CASHIER_ID,
                            DO_CHIEF_ID = item.DO_CHIEF_ID,
                            EOR_PRINT_COPIES = item.EOR_PRINT_COPIES,
                            ORPaymentModes = new List<ORPaymentMode>(),
                            TransactionTxnCodes = new List<TransactionTxnCode>(),
                            PaidTransactionFees = new List<PaidTransactionFee>(),
                            MVRSPaymentDetails = new MVRSPaymentDetails()
                        };

                        // Add ORPaymentModes
                        foreach (var fee in item.ORPaymentModes)
                        {
                            newItem.ORPaymentModes.Add(new ORPaymentMode
                            {
                                PAYMENT_AMOUNT = fee.PAYMENT_AMOUNT,
                                PAYMENT_MODE_DESC = fee.PAYMENT_MODE_DESC,
                                REFERENCE_NO = fee.REFERENCE_NO
                            });
                        }

                        // Add TransactionTxnCodes
                        foreach (var txnCode in item.TransactionTxnCodes)
                        {
                            newItem.TransactionTxnCodes.Add(new TransactionTxnCode
                            {
                                TXN_CODE = txnCode.TXN_CODE 
                            });
                        }

                        // Add PaidTransactionFees
                        foreach (var fee in item.PaidTransactionFees)
                        {
                            newItem.PaidTransactionFees.Add(new PaidTransactionFee
                            {
                                FEE_CODE = fee.FEE_CODE,
                                FEE_DESC = fee.FEE_DESC,
                                FEE_AMOUNT = fee.FEE_AMOUNT,
                                FEE_TYPE = fee.FEE_TYPE,
                                SEQ_INDICATOR = fee.SEQ_INDICATOR
                            });
                        }

                        // Add MVRSPaymentDetails
                        newItem.MVRSPaymentDetails = new MVRSPaymentDetails
                        {
                            MV_FILE_NO = item.MVRSPaymentDetails.MV_FILE_NO,
                            MV_DESCRIPTION = item.MVRSPaymentDetails.MV_DESCRIPTION,
                            PLATE_NO = item.MVRSPaymentDetails.PLATE_NO,
                            GROSS_WT = item.MVRSPaymentDetails.GROSS_WT,
                            OR_REMARKS = item.MVRSPaymentDetails.OR_REMARKS,
                            AIRCON_REF_DETAILS = item.MVRSPaymentDetails.AIRCON_REF_DETAILS,
                            REGISTRATION_VALIDITY = item.MVRSPaymentDetails.REGISTRATION_VALIDITY
                        };

                        response_result.Add(newItem);
                    }
                }
            }

            return response_result;

        }
        public async Task<List<Fees_Details>> getFeespayment(string transid)
        {

            var transcations = (dynamic)null;
            //var result = (dynamic)null;
            List<Fees_Details> response_result = new List<Fees_Details>();
            string res = "";
            var data = new List<RequestParameter>
                    {
                        new RequestParameter
                        {
                            ParameterName = "USER_ID",
                            ParameterValue = "msantonio",
                            ParameterType = "ST"
                        },
                          new RequestParameter
                        {
                            ParameterName = "TXN_ID",
                            ParameterValue =transid,
                            ParameterType = "ST"
                        },
                        new RequestParameter
                        {
                            ParameterName = "OFFICE_CODE",
                            ParameterValue = "0104",
                            ParameterType = "ST"
                        },
                          new RequestParameter
                        {
                            ParameterName = "TXN_CODE",
                            ParameterValue = "LCLC01",
                            ParameterType = "ST"
                        }

                    };
            List<TokenResult> tokenResults = GetTokenAsync().GetAwaiter().GetResult();
            HttpClient client = new HttpClient();
            var url = _Ipsettings.ipaddress + "/api/Transaction/GetDetails";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new Token().generateJWT());
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync(url, content))
            {
                res = await response.Content.ReadAsStringAsync();
                var baseResult = JsonConvert.DeserializeObject<BaseResult>(res);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(res);
                if (!string.IsNullOrEmpty(apiResponse.Data))
                {
                    Unlock(transid);
                    var modelList = JsonConvert.DeserializeObject<FeesModelList>(apiResponse.Data);
                    var modelListCollection = new List<FeesModelList> { modelList };
                    var fees = new List<TXN_CHARGES>();
                    // Output to verify
                    foreach (var item in modelListCollection)
                    {
                        //Console.WriteLine($"Transaction ID: {item.MV_DETAIL.TXN_ID}");
                        var items = new Fees_Details();
                        items.TXN_ID = item.TXN_HEADER.TXN_ID;
                        items.TXN_CODE = item.TXN_HEADER.TXN_CODE;
                        items.CUST_NAME = item.TXN_HEADER.CUST_NAME;
                        items.RO_CODE = item.TXN_HEADER.RO_CODE;
                        items.DO_CODE = item.TXN_HEADER.DO_CODE;
                        items.STATUS = item.TXN_HEADER.STATUS;
                        items.OFFICE_CODE = item.TXN_HEADER.OFFICE_CODE;
                        items.SETTLEMENT_TYPE = item.TXN_HEADER.SETTLEMENT_TYPE;
                        items.TXN_DATE = item.TXN_HEADER.TXN_DATE;
                        items.CUSTOMER_ID = item.TXN_HEADER.CUSTOMER_ID;
                        items.TOTAL_DUE = item.TXN_HEADER.TOTAL_DUE;


                        foreach (var fee in item.TXN_CHARGES)
                        {
                            var r_item = new TXN_CHARGES();
                            r_item.TXN_ID = fee.TXN_ID;
                            r_item.FEE_CODE = fee.FEE_CODE;
                            r_item.FEE_DESC = fee.FEE_DESC;
                            r_item.SEQ_INDICATOR = fee.SEQ_INDICATOR;
                            r_item.BATCH_NO = fee.BATCH_NO;
                            r_item.BATCH_FLAG = fee.BATCH_FLAG;
                            r_item.CHARGED_AMT = fee.CHARGED_AMT;
                            r_item.CURR_CODE = fee.CURR_CODE;
                            r_item.WAIVED_INDICATOR = fee.WAIVED_INDICATOR == "0"? "NO" : "YES";
                            r_item.OR_NO = fee.OR_NO;
                            r_item.OR_DATE = fee.OR_DATE;
                            r_item.CASHIER_ID = fee.CASHIER_ID;
                            r_item.CASHIER_OFFICE = fee.CASHIER_OFFICE;
                            r_item.TOTAL_ID = fee.TOTAL_ID;
                            r_item.PROC_FEE_IND = fee.PROC_FEE_IND;
                            r_item.RO_CODE = fee.RO_CODE;
                            r_item.DO_CODE = fee.DO_CODE;
                            r_item.OPMODE = fee.OPMODE;
                            r_item.GL_PRODUCT_CODE = fee.GL_PRODUCT_CODE;
                       

                            fees.Add(r_item);

                            //Console.WriteLine($"Fee Description: {fee.FEE_DESC}, Amount: {fee.CHARGED_AMT}");
                        }
                        //items.Total = item.FEES.Sum(item => item.CHARGED_AMT).ToString();
                        items.TXN_CHARGES = fees;
                        response_result.Add(items);
                    }
                }

            }

            return response_result;
        }
        public async Task<List<MV_Details>> getMV(string transid)
        {

            var transcations = (dynamic)null;
            //var result = (dynamic)null;
            List<MV_Details> response_result = new List<MV_Details>();
            string res = "";
            var data = new List<RequestParameter>
                    {
                    new RequestParameter
                    {
                        ParameterName = "USER_ID",
                        ParameterValue = "msantonio",
                        ParameterType = "ST"
                    },
                      new RequestParameter
                    {
                        ParameterName = "TXN_ID",
                        ParameterValue =transid,
                        ParameterType = "ST"
                    },
                    new RequestParameter
                    {
                        ParameterName = "OFFICE_CODE",
                        ParameterValue = "0104",
                        ParameterType = "ST"
                    }

                    };
            List<TokenResult> tokenResults = GetTokenAsync().GetAwaiter().GetResult();
            HttpClient client = new HttpClient();
            var url = _Ipsettings.ipaddress + "/api/TxnDetails/GetMVTxnDetail";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new Token().generateJWT());
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync(url, content))
            {
                res = await response.Content.ReadAsStringAsync();
                var baseResult = JsonConvert.DeserializeObject<BaseResult>(res);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(res);
                if (!string.IsNullOrEmpty(apiResponse.Data))
                {
                   
                    var modelList = JsonConvert.DeserializeObject<ModelList>(apiResponse.Data);
                    var modelListCollection = new List<ModelList> { modelList };
                    var fees = new List<Fees>();
                    // Output to verify
                    foreach (var item in modelListCollection)
                    {
                        //Console.WriteLine($"Transaction ID: {item.MV_DETAIL.TXN_ID}");
                        var items = new MV_Details();
                        items.TXN_ID = item.MV_DETAIL.TXN_ID;
                        items.MV_FILE_NO = item.MV_DETAIL.MV_FILE_NO;
                        items.PLATE_NO = item.MV_DETAIL.PLATE_NO;
                        items.ENGINE_NO = item.MV_DETAIL.ENGINE_NO;
                        items.CHASIS_NO = item.MV_DETAIL.CHASSIS_NO;
                        items.MAKE = item.MV_DETAIL.MAKE;
                        items.SERIES = item.MV_DETAIL.SERIES;
                        items.CUSTOMER_NAME = item.MV_DETAIL.CUSTOMER_NAME;
                        items.CUSTOMER_ID = item.MV_DETAIL.CUSTOMER_ID;


                        foreach (var fee in item.FEES)
                        {
                            var r_item = new Fees();
                            r_item.FEE_DESC = fee.FEE_DESC;
                            r_item.CHARGED_AMT = fee.CHARGED_AMT.ToString();


                            fees.Add(r_item);

                            //Console.WriteLine($"Fee Description: {fee.FEE_DESC}, Amount: {fee.CHARGED_AMT}");
                        }
                        items.Total = item.FEES.Sum(item => item.CHARGED_AMT).ToString();
                        items.Fees = fees;
                        response_result.Add(items);
                    }
                }   
                
            }

            return response_result;
        }

        public async Task<bool> Unlock(string transid)
        {
            bool result=false;

            try
            {
                var data = new List<RequestParameter>
                    {
                    new RequestParameter
                    {
                        ParameterName = "USER_ID",
                        ParameterValue = "puertorm",
                        ParameterType = "ST"
                    },
                      new RequestParameter
                    {
                        ParameterName = "TXN_ID",
                        ParameterValue =transid,
                        ParameterType = "ST"
                    }

                    };
                List<TokenResult> tokenResults = GetTokenAsync().GetAwaiter().GetResult();
                HttpClient client = new HttpClient();
                var url = _Ipsettings.ipaddress + "/api/Transaction/Unlock";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);
                // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new Token().generateJWT());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                   

                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and log them as needed
                Console.WriteLine($"Exception: {ex.GetBaseException()}");
            }


            return result;
        }
        public async Task<List<TokenResult>> GetTokenAsync()
        {
            List<TokenResult> resultList = new List<TokenResult>();
            string res = "";

            try
            {
                var data = new jwtparam
                {
                    Username = "Kieth",
                    AppCode = "CACAMS",
                    Roles = ""
                };
                using (HttpClient client = new HttpClient())
                {
                    // Uncomment and set the Authorization header if needed
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-token");

                    var url = _Ipsettings.jwt +"api/TokenGeneration/ManualGenerateToken";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                    using (var response = await client.PostAsync(url, content))
                    {
                        res = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            TokenResult result = JsonConvert.DeserializeObject<TokenResult>(res);

                            // Wrap the single TokenResult object into a List
                            resultList.Add(result);
                        }
                        else
                        {
                            // Handle error response if needed
                            throw new Exception($"Error: {response.StatusCode}, {res}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and log them as needed
                Console.WriteLine($"Exception: {ex.GetBaseException()}");
            }

            return resultList;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
        public async Task<List<Transaction>>  getDetails()
        {

            var transcations = (dynamic)null;
            var result = (dynamic)null;
            List<Transaction> response_result = new List<Transaction>();
            string res = "";

            try
            {
                var data = new List<RequestParameter>
                    {
                    new RequestParameter
                    {
                        ParameterName = "USER_ID",
                        ParameterValue = "cashier_user",
                        ParameterType = "ST"
                    },
                      new RequestParameter
                    {
                        ParameterName = "TXN_ID",
                        ParameterValue = "010104103120230261",
                        ParameterType = "ST"
                    },
                    new RequestParameter
                    {
                        ParameterName = "OFFICE_CODE",
                        ParameterValue = "0104",
                        ParameterType = "ST"
                    }
                    };
                List<TokenResult> tokenResults = GetTokenAsync().GetAwaiter().GetResult();
                HttpClient client = new HttpClient();
                var url = _Ipsettings.ipaddress + "/api/PendingPayments/GetList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);
               // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new Token().generateJWT());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    res = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<ApiResponse>(res);
                    transcations = JsonConvert.DeserializeObject<List<Transaction>>(result.Data);
                    List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(result.Data);


                    for (int x = 0; x < transactions.Count; x++)
                    {
                        string dateTimeString = transcations[x].TXN_DATE;
                        DateTime dateTime = DateTime.Parse(dateTimeString);
                        string formattedDate = dateTime.ToString("MM-dd-yyyy");
                        var item = new Transaction();
                        item.OFFICE_CODE = transcations[x].OFFICE_CODE;
                        item.TXN_DATE = formattedDate;
                        item.TXN_ID = transcations[x].TXN_ID;
                        item.TXN_CODE = transcations[x].TXN_CODE;
                        item.CUSTOMER_NAME = transcations[x].CUSTOMER_NAME;
                        item.SETTLEMENT_TYPE = transcations[x].SETTLEMENT_TYPE;
                        item.SETTLEMENT_DESCRIPTION = transcations[x].SETTLEMENT_DESCRIPTION;
                        item.STATUS = transcations[x].STATUS;
                        item.STATUS_DESCRIPTION = transcations[x].STATUS_DESCRIPTION;
                        item.CUSTOMER_ID = transcations[x].CUSTOMER_ID;
                        response_result.Add(item);
                    }


                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return response_result;
        }

       

        [HttpPost]
        public async Task<IActionResult> GetOP(datefilter data)
        {
            List<Transaction> list = new List<Transaction>();
            
             list = getDetails().GetAwaiter().GetResult().Where(a => a.SETTLEMENT_TYPE == "OP" && Convert.ToDateTime(a.TXN_DATE) >= Convert.ToDateTime(data.datefrom) && Convert.ToDateTime(a.TXN_DATE) <= Convert.ToDateTime(data.dateto)).OrderByDescending(a => a.TXN_DATE).ThenByDescending(a => a.TXN_ID).ToList();


            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        } 
        [HttpPost]
        public async Task<IActionResult> checquelist()
        {
            List<ChequeModel> list = new List<ChequeModel>();

             list = GetChequeList().GetAwaiter().GetResult().Take(10).ToList();

            return Json(list);
        }
        [HttpPost]
        public async Task<IActionResult> GetWI(datefilter data)
        {
            List<Transaction> list = new List<Transaction>();
          
            list = getDetails().GetAwaiter().GetResult().Where(a => a.SETTLEMENT_TYPE == "WI" && Convert.ToDateTime(a.TXN_DATE) >= Convert.ToDateTime(data.datefrom) && Convert.ToDateTime(a.TXN_DATE) <= Convert.ToDateTime(data.dateto)).OrderByDescending(a=>a.TXN_DATE).ThenByDescending(a=>a.TXN_ID).ToList();
            
            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        public class processpayment
        {
            public string? TXN_ID { get; set; }
            public string? TXN_CODE { get; set; }
            public string? CUSTOMER_ID { get; set; }
            public string? CUSTOMER_NAME { get; set; }
            public string? AMOUNT { get; set; }
            public string? Date { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> SavePayment(processpayment data)
        {
            string status = "";
            double compfee= double.Parse(HttpContext.Session.GetString("ChargeAmount"));
            double total = Convert.ToDouble(data.AMOUNT) - compfee;
            try
            {
                var datas = new List<RequestParameter>
        {
            new RequestParameter { ParameterName = "USER_ID", ParameterValue = "DO0104", ParameterType = "ST" },
            new RequestParameter { ParameterName = "OFFICE_CODE", ParameterValue = "0104", ParameterType = "ST" },
            new RequestParameter { ParameterName = "TXN_ID", ParameterValue = data.TXN_ID, ParameterType = "ST" },
            new RequestParameter { ParameterName = "TXN_CODE", ParameterValue = data.TXN_CODE, ParameterType = "ST" },
            new RequestParameter { ParameterName = "TXN_DATE", ParameterValue = data.Date, ParameterType = "DT" },
            new RequestParameter { ParameterName = "POST_DATE", ParameterValue = DateTime.Now.ToString("yyyy-MM-dd"), ParameterType = "DT" },
            new RequestParameter { ParameterName = "AGENCY_CODE", ParameterValue = "02", ParameterType = "ST" },
            new RequestParameter { ParameterName = "CUSTOMER_ID", ParameterValue = data.CUSTOMER_ID, ParameterType = "ST" },
            new RequestParameter { ParameterName = "CUSTOMER_NAME", ParameterValue = data.CUSTOMER_NAME, ParameterType = "ST" },
            new RequestParameter { ParameterName = "SETTLEMENT_TYPE", ParameterValue = "OP", ParameterType = "ST" },
            new RequestParameter
            {
                ParameterName = "PAYMENT_RCVD",
                ParameterValue = "[{\"PAYMENT_MODE_CODE\":\"CA\",\"CURR_CODE\":\"PHP\",\"AMOUNT\":"+ Math.Round(total, 2)+",\"PROC_FEE_IND\":\"0\"},{\"PAYMENT_MODE_CODE\":\"CA\",\"CURR_CODE\":\"PHP\",\"AMOUNT\":"+Math.Round(compfee,2)+",\"PROC_FEE_IND\":\"1\"}]",
                ParameterType = "ST"
            }
        };
                List<TokenResult> tokenResults = GetTokenAsync().GetAwaiter().GetResult();
                HttpClient client = new HttpClient();
                var url = _Ipsettings.ipaddress + "/api/Payment/Process";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);

                StringContent content = new StringContent(JsonConvert.SerializeObject(datas), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    status = await response.Content.ReadAsStringAsync();
                    var baseResult = JsonConvert.DeserializeObject<BaseResult>(status);
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(status);
                    var chequeList = JsonConvert.DeserializeObject<List<PaymentResult>>(apiResponse.Data);
                    if (!string.IsNullOrEmpty(apiResponse.Data))
                    {
                        foreach (var item in chequeList)
                        {
                            status = item.OR_NO;
                        }
                         
                    }


                }
            }
            catch (Exception ex) 
            {
                 status = ex.GetBaseException().Message;
            }
            return Json(new { stats = status });
        }
        public class printedstatus
        {
            public string? ornum { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> SetOrPrintedSttus(printedstatus data)
        {
            string status = "";
            try
            {
                var datas = new List<RequestParameter>
        {
            new RequestParameter { ParameterName = "USER_ID", ParameterValue = "DO0104", ParameterType = "ST" },
            new RequestParameter { ParameterName = "OR_NO", ParameterValue = data.ornum, ParameterType = "ST" }
        };
                List<TokenResult> tokenResults = GetTokenAsync().GetAwaiter().GetResult();
                HttpClient client = new HttpClient();
                var url = _Ipsettings.ipaddress + "/api/OR/SetORPrintedStatus";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);

                StringContent content = new StringContent(JsonConvert.SerializeObject(datas), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    status = await response.Content.ReadAsStringAsync();
                    var baseResult = JsonConvert.DeserializeObject<BaseResult>(status);
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(status);
                    status = apiResponse.BaseResult.Status;

                }
            }
            catch (Exception ex) 
            {
                 status = ex.GetBaseException().Message;
            }
            return Json(new { stats = status });
        }
        public static bool IsValidDate(DateTime date)
        {
            return date >= new DateTime(1900, 1, 1);
        }
        private bool TryParseDate(string dateString, out DateTime date)
        {
            // Try parsing the date
            bool parsed = DateTime.TryParse(dateString, out date);
            return parsed && IsValidDate(date);
        }
    }

}
