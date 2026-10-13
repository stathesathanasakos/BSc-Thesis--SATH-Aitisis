using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.Web.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace SATH.Aitisis.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        /*
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }*/


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                _logger.LogError(exceptionFeature.Error, "Unhandled exception occurred while processing {Path}.", exceptionFeature.Path);
            }

            Response.StatusCode = StatusCodes.Status500InternalServerError;

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }


        
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult StatusCodePage(int code)
        {
            Response.StatusCode = code;
            ViewData["StatusCode"] = code;

            ViewData["Message"] = code switch
            {
                StatusCodes.Status400BadRequest => 
                    "The request could not be processed because it contains invalid information.",

                StatusCodes.Status401Unauthorized =>
                    "Authentication is required to access this resource.",

                StatusCodes.Status403Forbidden =>
                    "You do not have permission to access this resource.",

                StatusCodes.Status404NotFound =>
                    "The requested resource could not be found.",

                _ =>
                    "The request could not be completed."
            };

            return View();
        }
    }
}
