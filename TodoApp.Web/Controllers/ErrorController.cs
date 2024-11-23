using Microsoft.AspNetCore.Mvc;

namespace TodoApp.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                    _logger.LogWarning("404 error occurred. Path = {Path}", statusCodeResult?.OriginalPath);
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Sorry, something went wrong on the server";
                    _logger.LogError("500 error occurred. Path = {Path}", statusCodeResult?.OriginalPath);
                    break;
                default:
                    ViewBag.ErrorMessage = "Sorry, an error occurred";
                    _logger.LogError("Error occurred. Status code = {StatusCode}, Path = {Path}", 
                        statusCode, statusCodeResult?.OriginalPath);
                    break;
            }

            return View("Error");
        }

        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            
            _logger.LogError(exceptionDetails?.Error, 
                "Error occurred. Path = {Path}", exceptionDetails?.Path);

            ViewBag.ErrorMessage = "Sorry, an error occurred while processing your request";
            ViewBag.ErrorDetails = exceptionDetails?.Error.Message;

            return View();
        }
    }
}
