using Microsoft.AspNetCore.Mvc;
using Protobuf.Server.DTO;
using Google.Protobuf;

namespace Protobuf.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEmployees()
        {
            var employees = new Employee[] {
                new Employee { Id = 1, Name = "Oleksandr", Salary = 1000 },
                new Employee { Id = 2, Name = "Dmytro", Salary = 2000 },
                new Employee { Id = 3, Name = "Andriy", Salary = 3000 },
            };

            return Ok(employees);
        }

        [HttpGet("proto")]
        public IActionResult GetEmployeesProto()
        {
            var employees = new DistributedSystems.Proto.Employees
            {
                Employees_ = {
                    new DistributedSystems.Proto.Employee { Id = 4, Name = "Denys", Salary = 4000 },
                    new DistributedSystems.Proto.Employee { Id = 5, Name = "Mykhailo", Salary = 5000 },
                    new DistributedSystems.Proto.Employee { Id = 6, Name = "Ostap", Salary = 6000 }
                }
            };

            using (var output = new MemoryStream())
            {
                employees.WriteTo(output);

                return Ok(output.ToArray());
            }
        }
    }
}
