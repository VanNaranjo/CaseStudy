using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HelpdeskViewModels;

namespace HelpdeskWebsite.Controllers
{
    // Specifies the route for this controller as "api/Department"
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        // HTTP GET method to retrieve all departments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                DepartmentViewModel viewmodel = new(); // Initialize a new instance of the Department view model
                List<DepartmentViewModel> allDepartments = await viewmodel.GetAll(); // Asynchronously get all departments
                return Ok(allDepartments); // Return the list of departments with HTTP 200 status
            }
            catch (Exception ex)    
            {
                // Log any exceptions that occur, specifying the class and method where it happened
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);

                // Return a 500 Internal Server Error if something goes wrong
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
