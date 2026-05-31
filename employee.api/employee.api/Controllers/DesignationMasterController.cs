
using employee.api.Modal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationMasterController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public DesignationMasterController(EmployeeDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAllDesignations()
        {
            try
            {
                var data = await _context.Designations.ToListAsync();

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
        [HttpGet("Designation/{id}")]
        public async Task<IActionResult> GetDesignationById(int id)
        {
            try
            {
                var designation = await _context.Designations
                    .FirstOrDefaultAsync(x => x.designationId == id);

                if (designation == null)
                {
                    return NotFound("Designation not found");
                }

                return Ok(designation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================
        // FILTER API
        // GET BY DEPARTMENT ID
        // =========================
        [HttpGet("GetByDepartment/{departmentId}")]
        public async Task<IActionResult> GetByDepartment(int departmentId)
        {
            try
            {
                var data = await _context.Designations
                    .Where(x => x.departmentId == departmentId)
                    .ToListAsync();

                if (data.Count == 0)
                {
                    return NotFound("No designation found");
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
        [HttpPost("AddDesignation")]
        public async Task<IActionResult> AddDesignation([FromBody] Designation designation)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check duplicate designation name
                bool exists = await _context.Designations
                    .AnyAsync(x => x.designationName == designation.designationName);

                if (exists)
                {
                    return BadRequest("Designation already exists");
                }

                await _context.Designations.AddAsync(designation);
                await _context.SaveChangesAsync();

                return Ok("Designation added successfully");
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
        public async Task<IActionResult> UpdateDesignation(int id, [FromBody] Designation designation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingDesignation = await _context.Designations
                    .FirstOrDefaultAsync(x => x.designationId == id);

                if (existingDesignation == null)
                {
                    return NotFound("Designation not found");
                }

                existingDesignation.departmentId = designation.departmentId;
                existingDesignation.designationName = designation.designationName;

                _context.Designations.Update(existingDesignation);
                await _context.SaveChangesAsync();

                return Ok("Designation updated successfully");
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
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            try
            {
                var designation = await _context.Designations
                    .FirstOrDefaultAsync(x => x.designationId == id);

                if (designation == null)
                {
                    return NotFound("Designation not found");
                }

                _context.Designations.Remove(designation);
                await _context.SaveChangesAsync();

                return Ok("Designation deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}