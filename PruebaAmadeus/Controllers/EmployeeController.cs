using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaAmadeus.Models;
using PruebaAmadeus.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaAmadeus.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DBAmadeusContext _dBAmadeusContext;

        public EmployeeController(DBAmadeusContext dBAmadeusContext)
        {
            _dBAmadeusContext = dBAmadeusContext;
        }

        [HttpPost]
        [Route("RegisterEmployee")]
        public async Task<ActionResult> RegisterEmployee(EmployeeDTO employeeDTO)
        {
            var employeeModel = new Employee
            {
                Name = employeeDTO.Name,
                Email = employeeDTO.Email,
                PhoneNumber = employeeDTO.PhoneNumber,
                IsActive = true,
                DateCreated = DateTime.Now
            };

            await _dBAmadeusContext.Employees.AddAsync(employeeModel);
            await _dBAmadeusContext.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(employeeModel.Name.Trim()))
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
            }
        }

        [HttpGet]
        [Route("GetEmployees")]
        public async Task<ActionResult> GetEmployees()
        {
            var employees = await _dBAmadeusContext.Employees.Where(s=> s.IsActive == true).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = employees });
        }

        [HttpGet]
        [Route("GetEmployeeById")]
        public async Task<ActionResult> GetEmployeeById(int id)
        {
            var employee = await _dBAmadeusContext.Employees.FindAsync(id);
            if (employee != null)
            {
                return StatusCode(StatusCodes.Status200OK, new { value = employee });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut]
        [Route("EditEmployee")]
        public async Task<ActionResult> EditEmployee(EmployeeDTO employeeDTO)
        {
            var employee = await _dBAmadeusContext.Employees.Where(u => u.Id == employeeDTO.Id).FirstOrDefaultAsync();

            employee.Name = employeeDTO.Name;
            employee.Email = employeeDTO.Email;
            employee.PhoneNumber = employeeDTO.PhoneNumber;
            //employee.IsActive = employeeDTO.IsActive;
            //employee.DateCreated = employeeDTO.DateCreated;

            _dBAmadeusContext.Employees.Update(employee);
             _dBAmadeusContext.SaveChanges();

            if (!string.IsNullOrWhiteSpace(employee.Name.Trim()))
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
            }
        }

        [HttpPut]
        [Route("DeleteEmployeeById")]
        public async Task<ActionResult> DeleteEmployeeById([FromBody]int id)
        {
            var employee = await _dBAmadeusContext.Employees.FindAsync(id);

            employee.IsActive = false;

            _dBAmadeusContext.Employees.Update(employee);
            await _dBAmadeusContext.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(employee.Name.Trim()))
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
            }
        }
    }
}
