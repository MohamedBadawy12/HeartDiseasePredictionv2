using Database.Entities;
using HearPrediction.Api.Data.Services;
using HearPrediction.Api.DTO;
using HearPrediction.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HearPrediction.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JWT _jwt;
        private readonly IMailServices _mailServices;

        public AccountController(IAuthService authService, SignInManager<ApplicationUser> signInManager,
             IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMailServices mailServices, JWT jwt)
        {
            _authService = authService;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mailServices = mailServices;
            _jwt = jwt;
        }

        //Register of patient
        [HttpPost("registerPatient")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterUserAsync(model);
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var confirmationLink = Url.Action(nameof(ConfirmationOfEmail),"Authentication",new ConfirmEmailDto { Token = token,Email = user.Email});
            //var message = new MailRequestDto(user.Email, "Confirmation Email Link",confirmationLink);
            //_mailServices.SendEmail(message);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        //Register of doctor
        [HttpPost("registerDoctor")]
        //[AllowAnonymous]
        public async Task<IActionResult> RegisterDoctorAsync([FromBody] RegisterDoctorDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterDoctorAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        //Register of medicalAnalyst
        [HttpPost("registerMedicalAnalyst")]
        //[AllowAnonymous]
        public async Task<IActionResult> RegisterMedicalAnalystAsync([FromBody] RegisterMedicalAnalystDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterMedicalAnalystAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        //Register of receptionist
        [HttpPost("registerReceptionist")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterReceptionistAsync([FromBody] RegisterReciptionistDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterReceptionistAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        //Login  
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }

        [HttpGet("TestEmail")]
        [AllowAnonymous]
        public IActionResult TestEmail()
        {
            var message = new MailRequestDto("m.badawy2442002@gmail.com"
            , "Test", "<h1>Welcome to our Website</h1>");
            _mailServices.SendEmail(message);
            return StatusCode(StatusCodes.Status200OK,
                new Responses { Status = "Success", Message = "Email sent successfuly" });

        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirm)
        {
            var user = await _userManager.FindByNameAsync(confirm.Email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, confirm.Token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new Responses { Status = "Success", Message = "Email Verfied Successfully" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                        new Responses { Status = "Error", Message = "This User Doesn't exist!" });
        }

        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(ForgetPassword model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgetPasswordLink = Url.Action(nameof(Resetpassword), "Authentication", new { token, email = user.Email }
                , Request.Scheme);
                var message = new MailRequestDto(user.Email, "Forget Password Link", forgetPasswordLink);
                _mailServices.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                    new Responses
                    {
                        Status = "Success",
                        Message = $"Password changed is sent on Email " +
                    $"{user.Email}.please open your Email & click the link"
                    });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                    new Responses
                    {
                        Status = "Error",
                        Message = $"Couldn't sent link to email, Please try again."
                    });
        }

        [HttpGet("Resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> Resetpassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };
            return Ok(new { model });
        }

        [HttpPost("Resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> Resetpassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByNameAsync(resetPassword.Email);
            if (user != null)
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }
                return StatusCode(StatusCodes.Status200OK,
                    new Responses
                    {
                        Status = "Success",
                        Message = $"Password changed is sent on Email "
                    });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                    new Responses
                    {
                        Status = "Error",
                        Message = $"Couldn't sent link to email, Please try again."
                    });
        }

        //[HttpPost("Logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //    var result = await _authService.RevokeTokenAsync(token);
        //    await _signInManager.SignOutAsync();
        //    if (result)
        //    {
        //        return Ok(new { message = "Logout successful" });
        //    }
        //    else
        //    {
        //        return BadRequest(new { message = "Logout failed" });
        //    }
        //}

        [HttpGet("refreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("revokeToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await _authService.RevokeTokenAsync(token);

            if (!result)
                return BadRequest("Token is invalid!");

            return Ok();
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            if (refreshToken == null)
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }
            //var escapedRefreshToken = Uri.EscapeDataString(refreshToken);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
        [HttpGet("GetUserProfileByRole")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserProfile()
        {
            string userId = User.Claims.First(x => x.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return Ok(new
            {
                user.Email,
                user.FirstName,
                user.LastName,
                user.Age,
                user.Gender,
                user.PhoneNumber,
                user.ImageFile,
                user.BirthDate,
            });
        }

        [HttpGet("GetProfileByRole")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfile(string Email)
        {
            var userId = _userManager.GetUserName(HttpContext.User);
            if (!string.IsNullOrEmpty(Email))
            {
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
                return Ok(new DoctorFormDTO
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BirthDate = user.BirthDate,
                    Gender = (Enums.Gender)user.Gender,
                });
            }
            else
            {
                return BadRequest(ModelState);
            }
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
            //var userId =_userManager.GetUserId(HttpContext.User); 
            //var userRole = User.FindFirst(ClaimTypes.Role)?.Value; 

            //if (userId == null /*|| userRole == null*/)
            //{
            //    return BadRequest("User ID or Role not found in claims.");
            //}
            //ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;
            //return Ok(applicationUser);

            //switch (userRole.ToLower())
            //{
            //    case "doctor":
            //        var doctorProfile = await _unitOfWork.Doctors.GetProfile(userId);
            //        return Ok(doctorProfile); 

            //    case "medicalanalyst":
            //        var medicalAnalystProfile = await _unitOfWork.medicalAnalyst.GetProfile(userId);
            //        return Ok(medicalAnalystProfile); 

            //    case "receptionist":
            //        var receptionistProfile = await _unitOfWork.reciptionist.GetProfile(userId);
            //        return Ok(receptionistProfile); 

            //    case "user":
            //        var userProfile = await _unitOfWork.Patients.GetProfile(userId);
            //        return Ok(userProfile); 

            //    default:
            //        return BadRequest("Invalid user role.");
            //}
        }
        [HttpPost("UpadteProfile")]
        public async Task<IActionResult> UpadteProfile()
        {
            return Ok();
        }
    }
}
