using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaAmadeus.Custom;
using PruebaAmadeus.Models;
using PruebaAmadeus.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaAmadeus.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly DBAmadeusContext _dBAmadeusContext;
        private readonly Utilities _utilities;

        public AccessController(DBAmadeusContext dBAmadeusContext, Utilities utilities)
        {
            _dBAmadeusContext = dBAmadeusContext;
            _utilities = utilities;
        }

        [HttpPost]
        [Route("RegisterUser")]
        public async Task<ActionResult> RegisterUser(UserDTO userDTO)
        {

            var dataValid = _dBAmadeusContext.Users.Where(w => w.Email == userDTO.Email).ToList();
            if (dataValid.Count > 0)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, mensaje =  $"El usuario {userDTO.Email} ya existe"});
            }
            var userModel = new User
            {
                Email = userDTO.Email,
                Password = _utilities.EncryptSHA256(userDTO.Password)
            };

            await _dBAmadeusContext.Users.AddAsync(userModel);
            await _dBAmadeusContext.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(userModel.Email.Trim()))
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(UserDTO userDTO)
        {
            var user = await _dBAmadeusContext.Users.Where(u =>
            u.Email == userDTO.Email && u.Password == _utilities.EncryptSHA256(userDTO.Password)
            ).FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utilities.GenerateJWT(user) });
            }
        }
    }
}
