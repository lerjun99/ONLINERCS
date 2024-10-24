using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RCS.Models;
using System.Diagnostics;
using static RCS.ApplicationEntityModels.EntityModels;
using System.Text;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace RCS.Controllers
{
    public class MiscellaneousController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly Connection _Ipsettings;
        public MiscellaneousController(ILogger<DashboardController> logger ,IOptions<Connection> ipsettings)
        {
            _logger = logger;
            _Ipsettings = ipsettings.Value;
        }

        public IActionResult CashBalancing()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Reasonlist()
        {
            List<ReasonVM> list = new List<ReasonVM>();

            list = getReasonlist().GetAwaiter().GetResult().Take(10).ToList();

            return Json(list);
        }
        public IActionResult CancelOR()
        {
            return PartialView("CancelOR");
        }
        public class cancelorvm
        {
            public string? REASON_CODE { get; set; }
            public string? OR_NO { get; set; }
            public string? REMARKS { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> ORCANCEL(cancelorvm data)
        {
            string status = "";
            try
            {
                var datas = new List<RequestParameter>
        {
            new RequestParameter { ParameterName = "USER_ID", ParameterValue = "DO0104", ParameterType = "ST" },
            new RequestParameter { ParameterName = "OR_NO", ParameterValue = data.OR_NO, ParameterType = "ST" },
            new RequestParameter { ParameterName = "TXN_ID", ParameterValue = "", ParameterType = "ST" },
            new RequestParameter { ParameterName = "SEQ_IND", ParameterValue = "", ParameterType = "ST" },
            new RequestParameter { ParameterName = "REASON_CODE", ParameterValue = data.REASON_CODE, ParameterType = "ST" },
            new RequestParameter { ParameterName = "REMARKS", ParameterValue = data.REMARKS, ParameterType = "ST" },
            new RequestParameter { ParameterName = "SUPERVISOR_ID", ParameterValue = "DO0104", ParameterType = "ST" },
            new RequestParameter { ParameterName = "OFFICE_CODE", ParameterValue = "0104", ParameterType = "ST" },
            new RequestParameter { ParameterName = "AGENCY_CODE", ParameterValue = "02", ParameterType = "ST" },
            new RequestParameter { ParameterName = "POST_DATE", ParameterValue = DateTime.Now.ToString("MM-dd-yyyy"), ParameterType = "DT" }
        };
                List<TokenResult> tokenResults = GetTokenAsync().GetAwaiter().GetResult();
                HttpClient client = new HttpClient();
                var url = _Ipsettings.ipaddress + "/api/ORCancellation/Process";
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
        public async Task<List<ReasonVM>> getReasonlist()
        {

            var transcations = (dynamic)null;
            List<ReasonVM> response_result = new List<ReasonVM>();
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
            var url = _Ipsettings.ipaddress + "/api/ORCancelReason/GetList";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResults[0].Token);
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new Token().generateJWT());
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync(url, content))
            {
                res = await response.Content.ReadAsStringAsync();
                var baseResult = JsonConvert.DeserializeObject<BaseResult>(res);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(res);
                var chequeList = JsonConvert.DeserializeObject<List<ReasonVM>>(apiResponse.Data);
                if (!string.IsNullOrEmpty(apiResponse.Data))
                {

                    // Output to verify
                    foreach (var item in chequeList)
                    {
                        //Console.WriteLine($"Transaction ID: {item.MV_DETAIL.TXN_ID}");
                        var items = new ReasonVM();
                        items.REASON_DESC=  item.REASON_DESC;
                        items.REASON_CODE=  item.REASON_CODE;
                        response_result.Add(items);
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
    }
}
