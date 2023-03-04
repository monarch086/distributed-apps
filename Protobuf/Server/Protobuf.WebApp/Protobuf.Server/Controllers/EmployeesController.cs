using Microsoft.AspNetCore.Mvc;
using Protobuf.Server.DTO;

namespace Protobuf.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    //application/x-protobuf
    public class EmployeesController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> ListRequirementLinksByRequirementId()
        {
            var employees = new Employee[] { 
                new Employee { Id = 1, Name = "Oleksandr", Salary = 1000 },
                new Employee { Id = 2, Name = "Dmytro", Salary = 2000 },
                new Employee { Id = 2, Name = "Andriy", Salary = 3000 },
            };

            return Ok(employees);
        }
    }
}
