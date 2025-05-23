namespace StoreManager.ApiServices.DummyRest.Model.Api.ResponseBody
{
    public class Employee
    {
        public int id { get; set; }
        public string employee_name { get; set; }
        public int employee_salary { get; set; }
        public int employee_age { get; set; }
        public string profile_image { get; set; }
    }

    public class GetEmployeesResponse
    {
        public string status { get; set; }
        public List<Employee> data { get; set; }
        public string message { get; set; }
    }
}