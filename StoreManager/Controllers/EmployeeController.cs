using Microsoft.AspNetCore.Mvc;
using StoreManager.ApiServices.DummyRest;
using StoreManager.ViewModels.Employee;

namespace StoreManager.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IDummyRestApiService _employeeService;
        public EmployeeController(ILogger<EmployeeController> logger, IDummyRestApiService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetEmployees()
        {
            var employees = await _employeeService.GetEmployees();
            if (employees == null)
            {
                return Json(new { data = new List<EmployeeVM>(), error = true });
            }
            return Json(new
            {
                data = employees.Select(e => new EmployeeVM()
                {
                    EmployeeAge = e.employee_age,
                    EmployeeName = e.employee_name,
                    EmployeeSalary = e.employee_salary,
                    ProfileImage = e.profile_image,
                    Id = e.id
                }),
                error = false
            });

        }
    }
}
