using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace StoreManager.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger) 
        { 
            _logger = logger;
        }
        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            var ExceptoionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>() ;
            _logger.LogError($"Exception path:{ExceptoionDetails.Path}\nException message:{ExceptoionDetails.Error.Message}");
            return View();

        }

    }
}
