using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HelpdeskViewModels;

namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    public class ProblemController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                ProblemViewModel viewModel = new();
                List<ProblemViewModel> allProblems = await viewModel.GetAll();
                return Ok(allProblems);
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
