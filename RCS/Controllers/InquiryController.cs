using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RCS.Models;
using System.Diagnostics;
using static RCS.ApplicationEntityModels.EntityModels;
using System.Text;
using System.Runtime;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System;

namespace RCS.Controllers
{
    public class InquiryController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly Connection _Ipsettings;
        public InquiryController(ILogger<DashboardController> logger, IOptions<Connection> ipsettings)
        {
            _logger = logger;
            _Ipsettings = ipsettings.Value;
        }

        public IActionResult Index()
        {
            var result = GetOrStatus().GetAwaiter().GetResult().ToList();

            if (result != null)
            {
                var model = new ORStatusModel
                {
                    Statuslist = result
                };
                return View(model);
            }
            else
            {
                ViewBag.ErrorMessage = "No Data Found";
                return View("Index");
            }

        }
        public async Task<bool> Unlock(string transid)
        {
            bool result = false;

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
                            r_item.WAIVED_INDICATOR = fee.WAIVED_INDICATOR == "0" ? "NO" : "YES";
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
   
        public IActionResult FeesPayment(string ornum)
        {
            var result = getOR(ornum).GetAwaiter().GetResult().FirstOrDefault();
            if (result != null)
            {
                var model = new OrDetailsVM
                {
                    OR_NO = ornum,
                    OR_DATE =Convert.ToDateTime( result.OR_DATE).ToString("MM-dd-yyyy"),
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
                var models = (dynamic)null;
                var result1 = getFeespayment(result.TXN_ID).GetAwaiter().GetResult().FirstOrDefault();
                if (result1 != null)
                {
                     models = new Fees_Details
                    {
                        TXN_ID = result.TXN_ID,
                        TXN_CODE = result1.TXN_CODE,
                        CUST_NAME = result1.CUST_NAME,
                        RO_CODE = result1.RO_CODE,
                        DO_CODE = result1.DO_CODE,
                        STATUS = result1.STATUS,
                        OFFICE_CODE = result1.OFFICE_CODE,
                        SETTLEMENT_TYPE = result1.SETTLEMENT_TYPE,
                        TXN_DATE = result1.TXN_DATE,
                        CUSTOMER_ID = result1.CUSTOMER_ID,
                        TOTAL_DUE = result1.TOTAL_DUE,

                        TXN_CHARGES = result1.TXN_CHARGES.ToList()

                    };
                }
       
                MyViewModel viewModel = new MyViewModel { Model1 = model, Model2 = models };
                return PartialView("FeesPayment", viewModel);
            }
            else
            {
                MyViewModel viewModel = new MyViewModel { Model1 = null, Model2 = null };
                ViewBag.ErrorMessage = "No Data Found";
                return PartialView("FeesPayment", viewModel);
            }

         

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
                            TXN_ID = item.TXN_ID ,
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

                    var url = _Ipsettings.jwt + "api/TokenGeneration/ManualGenerateToken";
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
        [HttpPost]
        public async Task<IActionResult> getinquiryList(inquiryfilter filter)
        {
            List<PaidORModel> list = new List<PaidORModel>();
            var fullname = filter.lname + ", " + filter.fname + " " + filter.mname;
            var datefrom = filter.datefrom == null ? DateTime.Now.ToString("yyyy-MM-dd") : filter.datefrom;
            var dateto = filter.dateto == null ? DateTime.Now.ToString("yyyy-MM-dd") : filter.dateto;
            if (filter.status != null )
            {
                list = GetPaidORList(datefrom, dateto).GetAwaiter().GetResult().Where(a => a.OR_STATUS == filter.status).ToList();

            }
            else if (filter.orgname != null && filter.orgname != "")
            {
                list = GetPaidORList(datefrom, dateto).GetAwaiter().GetResult().Where(a=> a.CUSTOMER_NAME.ToUpper().Trim().Contains(filter.orgname.ToUpper().Trim())).ToList();
            }
            else if (filter.ornum != null && filter.ornum != "")
            {
                list = GetPaidORList(datefrom, dateto).GetAwaiter().GetResult().Where(a => a.OR_NO.Trim() == filter.ornum.Trim()).ToList();
            }
            else if( filter.fname != null &&  filter.mname != null && filter.lname != null)
            {
             
                list = GetPaidORList(datefrom, dateto).GetAwaiter().GetResult().Where(a=>  a.CUSTOMER_NAME.ToUpper().Trim().Contains(fullname.ToUpper().Trim()) ).ToList();
            }
            else
            {
                list = GetPaidORList(datefrom, dateto).GetAwaiter().GetResult().ToList();
            }
            
            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        public async Task<List<PaidORModel>> GetPaidORList(string datefrom, string dateto)
        {
            //var orgname = filter.orgname == null ? "" : filter.orgname;
            //var ornum = filter.ornum == null ? "" : filter.ornum;
            //var datefrom = filter.datefrom == null ? DateTime.Now.ToString("yyyy-MM-dd") : filter.datefrom;
            //var dateto = filter.dateto == null ? DateTime.Now.ToString("yyyy-MM-dd") : filter.dateto;
            //var lname = filter.lname == null ? "" : filter.lname;
            //var mname = filter.mname == null ? "" : filter.mname;
            //var fname = filter.fname == null ? "" : filter.fname;
            var transcations = (dynamic)null;
            List<PaidORModel> response_result = new List<PaidORModel>();
            string res = "";
            var data = new List<RequestParameter>
            {
                        new RequestParameter
                        {
                            ParameterName = "OFFICE_CODE",
                            ParameterValue = "0104",
                            ParameterType = "ST"
                        },
                         new RequestParameter
                        {
                            ParameterName = "USER_ID",
                            ParameterValue = "DO0104",
                            ParameterType = "ST"
                        },
                         new RequestParameter
                        {
                            ParameterName = "OR_NO",
                            ParameterValue = "",
                            ParameterType = "ST"
                        },
                         new RequestParameter
                        {
                            ParameterName = "OR_DATE_FROM",
                            ParameterValue =  datefrom,
                            ParameterType = "DT"
                        },
                         new RequestParameter
                        {
                            ParameterName = "OR_DATE_TO",
                            ParameterValue =  dateto,
                            ParameterType = "DT"
                        },
                         new RequestParameter
                        {
                            ParameterName = "LAST_NAME",
                            ParameterValue = "",
                            ParameterType = "ST"
                        },
                         new RequestParameter
                        {
                            ParameterName = "FIRST_NAME",
                            ParameterValue = "",
                            ParameterType = "ST"
                        },
                         new RequestParameter
                        {
                            ParameterName = "MIDDLE_NAME",
                            ParameterValue = "",
                            ParameterType = "ST"
                        },
                         new RequestParameter
                        {
                            ParameterName = "ORG_NAME",
                            ParameterValue = "" ,
                            ParameterType = "ST"
                        }

            };
            List<TokenResult> tokenResults = GetTokenAsync().GetAwaiter().GetResult();
            HttpClient client = new HttpClient();
            var url = _Ipsettings.ipaddress + "/api/PaidOR/GetList";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new Token().generateJWT());
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync(url, content))
            {
                res = await response.Content.ReadAsStringAsync();
                var baseResult = JsonConvert.DeserializeObject<BaseResult>(res);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(res);
                var chequeList = JsonConvert.DeserializeObject<List<PaidORModel>>(apiResponse.Data);
                if (!string.IsNullOrEmpty(apiResponse.Data))
                {

                    // Output to verify
                    foreach (var item in chequeList)
                    {
                        //Console.WriteLine($"Transaction ID: {item.MV_DETAIL.TXN_ID}");
                        var items = new PaidORModel();
                        items.CUSTOMER_NAME = item.CUSTOMER_NAME;
                        items.OR_NO = item.OR_NO;
                        items.OR_DATE = Convert.ToDateTime(item.OR_DATE).ToString("yyyy-MM-dd");
                        items.OR_AMT = item.OR_AMT;
                        items.OR_STATUS = item.OR_STATUS;
                        items.OR_STATUS_DESC = item.OR_STATUS_DESC;
                        response_result.Add(items);
                    }
                }

            }

            return response_result;
        }

        public async Task<List<ORStatusModel>> GetOrStatus()
        {

            var transcations = (dynamic)null;
            List<ORStatusModel> response_result = new List<ORStatusModel>();
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
            var url = _Ipsettings.ipaddress + "/api/ORStatus/GetList";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new Token().generateJWT());
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync(url, content))
            {
                res = await response.Content.ReadAsStringAsync();
                var baseResult = JsonConvert.DeserializeObject<BaseResult>(res);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(res);
                var chequeList = JsonConvert.DeserializeObject<List<ORStatusModel>>(apiResponse.Data);
                if (!string.IsNullOrEmpty(apiResponse.Data))
                {

                    // Output to verify
                    foreach (var item in chequeList)
                    {
                        //Console.WriteLine($"Transaction ID: {item.MV_DETAIL.TXN_ID}");
                        var items = new ORStatusModel();
                        items.CODE = item.CODE;
                        items.DESCRIPTION = item.DESCRIPTION;
                        response_result.Add(items);
                    }
                }

            }

            return response_result;
        }

    }
}
