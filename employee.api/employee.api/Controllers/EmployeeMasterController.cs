using employee.api.Modal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeMasterController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeeMasterController(EmployeeDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var data = await _context.Employees.ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(x => x.employeeId == id);

                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================
        // FILTER API
        // GET BY DESIGNATION ID
        // =========================
        [HttpGet("GetByDesignation/{designationId}")]
        public async Task<IActionResult> GetByDesignation(int designationId)
        {
            try
            {
                var data = await _context.Employees
                    .Where(x => x.designationId == designationId)
                    .ToListAsync();

                if (data.Count == 0)
                {
                    return NotFound("No employees found");
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================
        // ADD
        // =========================
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Email unique validation
                bool emailExists = await _context.Employees
                    .AnyAsync(x => x.email == employee.email);

                if (emailExists)
                {
                    return BadRequest("Email already exists");
                }

                // Contact number unique validation
                bool contactExists = await _context.Employees
                    .AnyAsync(x => x.contactNo == employee.contactNo);

                if (contactExists)
                {
                    return BadRequest("Contact number already exists");
                }

                employee.createdDate = DateTime.Now;
                employee.modifiedDate = DateTime.Now;

                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                return Ok("Employee added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(x => x.employeeId == id);

                if (existingEmployee == null)
                {
                    return NotFound("Employee not found");
                }

                // Email unique validation
                bool emailExists = await _context.Employees
                    .AnyAsync(x => x.email == employee.email &&
                                   x.employeeId != id);

                if (emailExists)
                {
                    return BadRequest("Email already exists");
                }

                // Contact number unique validation
                bool contactExists = await _context.Employees
                    .AnyAsync(x => x.contactNo == employee.contactNo &&
                                   x.employeeId != id);

                if (contactExists)
                {
                    return BadRequest("Contact number already exists");
                }

                existingEmployee.name = employee.name;
                existingEmployee.contactNo = employee.contactNo;
                existingEmployee.email = employee.email;
                existingEmployee.city = employee.city;
                existingEmployee.state = employee.state;
                existingEmployee.pincode = employee.pincode;
                existingEmployee.altContactNo = employee.altContactNo;
                existingEmployee.address = employee.address;
                existingEmployee.designationId = employee.designationId;
                existingEmployee.modifiedDate = DateTime.Now;

                _context.Employees.Update(existingEmployee);
                await _context.SaveChangesAsync();

                return Ok("Employee updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(x => x.employeeId == id);

                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return Ok("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("SortEmployees")]
        public async Task<IActionResult> SortEmployees(
    string sortBy = "employeeId",
    string sortOrder = "asc")
        {
            try
            {
                var query = _context.Employees.AsQueryable();

                switch (sortBy.ToLower())
                {
                    case "name":
                        query = sortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.name)
                            : query.OrderBy(x => x.name);
                        break;

                    case "email":
                        query = sortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.email)
                            : query.OrderBy(x => x.email);
                        break;

                    case "contactno":
                        query = sortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.contactNo)
                            : query.OrderBy(x => x.contactNo);
                        break;

                    case "createddate":
                        query = sortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.createdDate)
                            : query.OrderBy(x => x.createdDate);
                        break;

                    default:
                        query = sortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.employeeId)
                            : query.OrderBy(x => x.employeeId);
                        break;
                }

                var data = await query.ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // =========================
        // PAGINATION API
        // =========================

        [HttpGet("Pagination")]
        public async Task<IActionResult> Pagination(
            int pageNumber = 1,
            int pageSize = 5)
        {
            try
            {
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return BadRequest("Page number and page size must be greater than 0");
                }

                var totalRecords = await _context.Employees.CountAsync();

                var employees = await _context.Employees
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    Data = employees
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================
        // LOGIN API
        // =========================

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                // Model Validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check User Credentials 
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(x =>
                        x.email == login.email &&
                        x.contactNo == login.contactNo);

                if (employee == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                return Ok(new
                {
                    Message = "Login successful",
                    EmployeeId = employee.employeeId,
                    
                    data = new
                    {
                        employee.name,
                        employee.email,
                        employee.designationId,
                        employee.employeeId,
                        employee.contactNo,
                        employee.role
                         

                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}