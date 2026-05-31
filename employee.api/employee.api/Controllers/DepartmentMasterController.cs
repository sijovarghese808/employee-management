using employee.api.DTOs;
using employee.api.Modal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentMasterController : ControllerBase
    {
        private readonly EmployeeDbContext _context;
        public DepartmentMasterController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllpartments")]
        public IActionResult GetDepartment()
        {
            var deptList = _context.Departments.ToList();

            return Ok(deptList);
        }

        [HttpPost("AddDepartment")]
        public IActionResult AddDepartment([FromBody] Department dept)
        {
            // Check if department name already exists (case-insensitive)
            bool exists = _context.Departments
                .Any(d => d.departmentName.ToLower() == dept.departmentName.ToLower());

            if (exists)
            {
                return Conflict("Department name must be unique.");
            }
            var checkifExists = _context.Departments.Any(d => d.departmentName.ToLower() == dept.departmentName.ToLower());

            _context.Departments.Add(dept);
            _context.SaveChanges();
            return Ok(new DesignationDTO<DesignationSuccessDto>
            {
                Success = true,
                Message = "New designation added succesfully",
                Data = new DesignationSuccessDto
                {
                    designationId :  0,
                    departmentId: 0,
                    designationName: 'Any'


                }
            }
    
        }

        [HttpPost("UpdateDepartment")]

        public IActionResult UpdateDepartment([FromBody] Department dept)
        {
            var existingDepartment = _context.Departments.Find(dept.departmentId);
            if(existingDepartment == null)
            {
                return Ok("No recods Found");
            }
            else
            {
                existingDepartment.departmentName = (dept.departmentName);
                existingDepartment.isActive = (dept.isActive);
                _context.SaveChanges();
                return Ok(" Department  updated successfully");


            }
        }

        [HttpPost("DeleteDepartment/{id}")]

        public IActionResult DeleteDepartment(int id)
        {
            var dept = _context.Departments.Find(id);

            if (dept == null)
            {
                return Ok("No recods Found");
            }
            else
            {
                _context.Departments.Remove(dept);
                _context.SaveChanges();
                return Ok("Department deleted Successfully");

            }
        }
    }
} 
