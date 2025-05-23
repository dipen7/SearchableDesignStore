using StoreManager.ApiServices.DummyRest.Model.Api.ResponseBody;

namespace StoreManager.ApiServices.DummyRest
{
    public interface IDummyRestApiService
    {
        Task<List<Employee>> GetEmployees();
    }
}
