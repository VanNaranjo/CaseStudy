using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HelpdeskViewModels;


namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    public class CallController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                CallViewModel viewModel = new();
                List<CallViewModel> allCalls = await viewModel.GetAll();
                return Ok(allCalls);
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

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] CallViewModel viewModel)
        {
            try
            {
                int retVal = await viewModel.Update();
                return retVal switch
                {
                    1 => Ok(new { msg = "Employee " + viewModel.EmployeeName + " updated!" }),
                    -1 => Ok(new { msg = "Employee " + viewModel.EmployeeName + " not updated!" }),
                    -2 => Ok(new { msg = "Data is stale for " + viewModel.EmployeeName + ", Employee not updated!" }),
                    _ => Ok(new { msg = "Employee " + viewModel.EmployeeName + " not updated!" }),
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CallViewModel viewModel)
        {
            try
            {
                await viewModel.Add();
                return viewModel.Id > 1
                ? Ok(new { msg = "Employee " + viewModel.EmployeeName + " added!" })
                : Ok(new { msg = "Employee " + viewModel.EmployeeName + " not added!" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                CallViewModel viewModel = new() { Id = id };
                return await viewModel.Delete() == 1
                ? Ok(new { msg = "Employee " + id + " deleted!" })
               : Ok(new { msg = "Employee " + id + " not deleted!" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }



    }
}
