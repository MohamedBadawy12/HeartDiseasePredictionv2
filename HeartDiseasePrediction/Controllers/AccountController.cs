using Database.Entities;
using HeartDiseasePrediction.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NToastNotify;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HeartDiseasePrediction.Controllers
{
    public class AccountController : Controller
    {
        private readonly IToastNotification _toastNotification;
        Uri baseAddress = new Uri("https://localhost:44304/api");
        HttpClient _client;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(IToastNotification toastNotification, UserManager<ApplicationUser> userManger,
            SignInManager<ApplicationUser> signInManager)
        {
            _toastNotification = toastNotification;
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
            _userManager = userManger;
            _signInManager = signInManager;
        }
        //Login
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginViewModel)
        {
            try
            {
                //string data = JsonConvert.SerializeObject(model);
                //StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                //HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress +
                //    "/Account/Login", content);
                //if (response.IsSuccessStatusCode)
                //{
                //    //LoginResponseVM models = JsonConvert.DeserializeObject<LoginResponseVM>(Convert.ToString(response));
                //    //var handler = new JwtSecurityTokenHandler();
                //    //var jwt = handler.ReadJwtToken(models.Token);
                //    //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                //    //identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                //    //identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
                //    //var principal = new ClaimsPrincipal(identity);
                //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                //    //HttpContext.Session.SetString("JWToken", models.Token);
                //    string token = await response.Content.ReadAsStringAsync();
                //    HttpContext.Session.SetString("JWToken", token);

                //    //ViewData["UserName"]=User.Identity.Name;
                //    //ViewData["UserName"] = model.Email;
                //    ViewBag.UserName = model.Email.ToString();
                //    TempData["successMesssage"] = "Login Success.";
                //    _toastNotification.AddSuccessToastMessage("Login Success");
                //    return RedirectToAction("Index", "Home");
                //}
                if (ModelState.IsValid == true)
                {
                    var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                    if (user != null)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        TempData["Error"] = "Wrong credentials. please,try again!";
                        return View(loginViewModel);
                    }
                    TempData["Error"] = "Wrong credentials. please,try again!";
                }
                return View(loginViewModel);
            }
            catch (Exception ex)
            {
                TempData["errorMesssage"] = ex.Message;
                _toastNotification.AddErrorToastMessage("Email or Password is Incorrect");
                return View(loginViewModel);
            }
            //_toastNotification.AddErrorToastMessage("Email or Password is Incorrect");
            //return View();
        }
        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            string data = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress +
                "/Account/ForgetPassword", content);
            if (response.IsSuccessStatusCode)
            {
                //TempData["successMesssage"] = "Password changed successfully.";
                //_toastNotification.AddSuccessToastMessage("Password changed successfully.");
                return RedirectToAction(nameof(Login));
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Error, please Check the Email");
                return View();
            }
        }

        public async Task<IActionResult> Resetpassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resetpassword(ResetPasswordViewModel model)
        {
            string data = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            //var content = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string, string>("email", model.Email),
            //    new KeyValuePair<string, string>("password", model.Password),
            //    new KeyValuePair<string, string>("confirmPassword", model.ConfirmPassword),
            //});
            HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress +
                "/Account/Resetpassword", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["successMesssage"] = "Password changed successfully.";
                _toastNotification.AddSuccessToastMessage("Password changed successfully.");
                return RedirectToAction(nameof(Login));
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Error in changing password");
                return View();
            }

        }

        //Logout 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString("JWToken", "");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> RegisterOfUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterOfUser(RegisterUserVM model)
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress +
                    "/Account/registerPatient", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMesssage"] = "User Created successfully.";
                    _toastNotification.AddSuccessToastMessage("Register User successfully.");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMesssage"] = ex.Message;
                _toastNotification.AddErrorToastMessage("Register User Failed");
                return View();
            }
            _toastNotification.AddErrorToastMessage("Register User Failed");
            return View();
        }
        public async Task<IActionResult> RegisterOfDoctor()
        {
            //HttpResponseMessage response = _client.GetAsync(_client.BaseAddress +
            //	"/Doctor/GetSpecailization").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //	string data = await response.Content.ReadAsStringAsync();
            //	JsonConvert.DeserializeObject<Specialization>(data);
            //}
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterOfDoctor(RegisterDoctorVM model)
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress +
                    "/Account/registerDoctor", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMesssage"] = "Doctor Created successfully.";
                    _toastNotification.AddSuccessToastMessage("Doctor Created successfully.");
                    return RedirectToAction("Index", "Doctor");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMesssage"] = ex.Message;
                _toastNotification.AddErrorToastMessage("Register Doctor Failed");
                return View();
            }
            _toastNotification.AddErrorToastMessage("Register Doctor Failed");
            return View();
        }
        public async Task<IActionResult> RegisterOfMedicalAnalyst()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterOfMedicalAnalyst(RegisterMedicalAnalystVM model)
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress +
                    "/Account/registerMedicalAnalyst", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMesssage"] = "Medical Analyst Created successfully.";
                    _toastNotification.AddSuccessToastMessage("Medical Analyst Created successfully.");
                    return RedirectToAction("Index", "MedicalAnalyst");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMesssage"] = ex.Message;
                _toastNotification.AddErrorToastMessage("Register Medical Analyst Failed");
                return View();
            }
            _toastNotification.AddErrorToastMessage("Register Medical Analyst Failed");
            return View();
        }
        public async Task<IActionResult> RegisterOfReciptionist()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterOfReciptionist(RegisterReciptionistVM model)
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress +
                    "/Account/registerReceptionist", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMesssage"] = "Reciptionist Created successfully.";
                    _toastNotification.AddSuccessToastMessage("Reciptionist Created successfully.");
                    return RedirectToAction("Index", "Reciptionist");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMesssage"] = ex.Message;
                _toastNotification.AddErrorToastMessage("Register Reciptionist Failed");
                return View();
            }
            _toastNotification.AddErrorToastMessage("Register Reciptionist Failed");
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
