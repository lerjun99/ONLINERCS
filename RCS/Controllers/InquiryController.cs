using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RCS.Models;
using System.Diagnostics;
using static RCS.ApplicationEntityModels.EntityModels;
using System.Text;
using System.Runtime;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

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
           
            return View();
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
        public async Task<IActionResult> getinquiryList()
        {
            List<PaidORModel> list = new List<PaidORModel>();

             list = GetPaidORList().GetAwaiter().GetResult().ToList();
            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        public async Task<List<PaidORModel>> GetPaidORList()
        {

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
                            ParameterValue = "jydomingo",
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
                            ParameterValue = "",
                            ParameterType = "DT"
                        },
                         new RequestParameter
                        {
                            ParameterName = "OR_DATE_TO",
                            ParameterValue = "",
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
                            ParameterValue = "org",
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
                        items.OR_DATE = item.OR_DATE;
                        items.OR_AMT = item.OR_AMT;
                        items.OR_STATUS = item.OR_STATUS;
                        items.OR_STATUS_DESC = item.OR_STATUS_DESC;
                        response_result.Add(items);
                    }
                }

            }

            return response_result;
        }

    }
}
