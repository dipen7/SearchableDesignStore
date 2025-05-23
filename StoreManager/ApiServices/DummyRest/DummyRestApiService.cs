
using StoreManager.ApiServices.DummyRest.Model.Api.ResponseBody;
using System.Text.Json;

namespace StoreManager.ApiServices.DummyRest
{
    public class DummyRestApiService : IDummyRestApiService
    {

        private readonly ILogger<IDummyRestApiService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;
        public DummyRestApiService(ILogger<IDummyRestApiService> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _apiUrl = configuration.GetValue<string>("ApiUrl");
            
        }
        public async Task<List<Employee>> GetEmployees()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                string retriveEmployeeListUrl = _apiUrl;
                HttpResponseMessage returnedEmployeeData;

                using (var request = new HttpRequestMessage(new HttpMethod("GET"), retriveEmployeeListUrl))
                {
                    _logger.LogInformation($"Calling Dummy EMployee List api.");
                    returnedEmployeeData = await client.SendAsync(request);

                }
                if (returnedEmployeeData.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    GetEmployeesResponse getEmployeesResponse = new GetEmployeesResponse();
                    try
                    {
                        string jsonResponse = await returnedEmployeeData.Content.ReadAsStringAsync();
                        getEmployeesResponse = JsonSerializer.Deserialize<GetEmployeesResponse>(jsonResponse);
                        _logger.LogInformation($"Dummy EMployee List api call runned successfully.");
                        if (getEmployeesResponse.status == "success")
                        {
                            _logger.LogInformation($"Dummy EMployee List api call runned successfully.");
                            return getEmployeesResponse.data;

                        }
                        else
                        {
                            _logger.LogInformation($"Dummy EMployee List api call runned successfully but with status other then success.");

                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Response from Dummy EMployee List api is not readable.:{ex.Message}");
                    
                    }
                }
                else
                {
                    _logger.LogWarning($"Dummy EMployee List api returned with status code {returnedEmployeeData.StatusCode} other then ok.");
                
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while calling Dummy EMployee List api.: {ex.Message}");
                _logger.LogInformation($"returning empty employee list due to api call error");

            }
            return null;

        }
    }
}
