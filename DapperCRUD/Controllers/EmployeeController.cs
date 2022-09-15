using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Employee> employees = await SelectAllEmployees(connection);
            return Ok(employees);
        }

        [HttpPut]
        public async Task<ActionResult<List<Employee>>> UpdateEmployee(Employee employee)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update empl set name = @Name, firstname = @FirstName, lastname = @LastName, place = @Place where id = @Id",employee);
            return Ok(await SelectAllEmployees(connection));
        }

        [HttpDelete("{employeeId}")]
        public async Task<ActionResult<List<Employee>>> DeleteEmployee(int employeeId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from empl where id = @Id", new { Id = employeeId });
            return Ok(await SelectAllEmployees(connection));
        }

        [HttpPost]
        public async Task<ActionResult<List<Employee>>> CreateEmployee(Employee employee)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into empl (name, firstname, lastname, place) values (@Name, @FirstName, @LastName, @Place)", employee);
            return Ok(await SelectAllEmployees(connection));
        }
        private static async Task<IEnumerable<Employee>> SelectAllEmployees(SqlConnection connection)
        {
            return await connection.QueryAsync<Employee>("select * from empl");
        }

        [HttpGet("{employeeId}")]
        public async Task<ActionResult<Employee>> GetEmployee(int employeeId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var employee = await connection.QueryFirstAsync<Employee>("select * from empl where id = @Id", new {Id=employeeId});
            return Ok(employee);
        }

    }
}
