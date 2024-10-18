using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using RCS.Models;
using static RCS.ApplicationEntityModels.EntityModels;
using System.Net;

namespace RCS.ApiHelper
{
    public class HelperApi
    {
        public static IConfiguration Configuration { get; set; }
        public static string GetIpAddress()
        {
            return Configuration?.GetSection("IP")["ipaddress"] ?? string.Empty;
        }

        public static string GetJwtUrl()
        {
            return Configuration?.GetSection("IP")["jwt"] ?? string.Empty;
        }
       
        public async Task<List<TokenResult>> GetTokenAsync()
        {
            List<TokenResult> resultList = new List<TokenResult>();
            string res = "";

            try
            {
                var data = new jwtparam
                {
                    Username = "Puertorm",
                    AppCode = "RCSAPI",
                    Roles = ""
                };

                using (HttpClient client = new HttpClient())
                {
                    // Uncomment and set the Authorization header if needed
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-token");

                    var url = GetIpAddress() + "api/TokenGeneration/ManualGenerateToken";
                    StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                    // Await the PostAsync call
                    using (var response = await client.PostAsync(url, content))
                    {
                        // Read the response content as a string
                        res = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            // Deserialize the response directly from the content
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
            var url = GetIpAddress() + "/api/TxnDetails/GetMVTxnDetail";
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
        public async Task<List<Transaction>> getDetails()
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
                var url = GetIpAddress() + "/api/PendingPayments/GetList";
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
            var url = GetIpAddress() + "/api/Transaction/GetDetails";
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
                var url = GetIpAddress() + "/api/Transaction/Unlock";
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
    }
}
