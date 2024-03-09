using Database.Entities;
using HeartDiseasePrediction.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace HeartDiseasePrediction.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DoctorController : Controller
    {
        private readonly IToastNotification _toastNotification;
        private readonly IDoctorService _doctorService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DoctorController(IToastNotification toastNotification, IDoctorService doctorService, AppDbContext context
            , IWebHostEnvironment webHostEnvironment)
        {
            _toastNotification = toastNotification;
            _doctorService = doctorService;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //get all doctors in list
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorService.GetDoctors();
            return View(doctors);
        }

        //Search 
        [HttpPost]
        public async Task<IActionResult> Index(string search)
        {
            var doctorViewModel = await _doctorService.FilterDoctors(search);
            return View(doctorViewModel);
        }

        //get doctor details
        public async Task<IActionResult> DoctorDetails(int id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctor(id);
                if (doctor == null)
                    return View("NotFound");

                var DoctorDetail = new DoctorVM
                {
                    FirstName = doctor.User.FirstName,
                    LastName = doctor.User.LastName,
                    BirthDate = doctor.User.BirthDate,
                    Email = doctor.User.Email,
                    Gender = doctor.User.Gender,
                    PhoneNumber = doctor.User.PhoneNumber,
                    ProfileImg = doctor.User.ProfileImg,
                };
                return View(DoctorDetail);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> DoctorProfile()
        {
            return View();
        }

        //Edit Doctor
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctor(id);
                if (doctor == null)
                    return View("NotFound");

                var DoctorDetail = new DoctorVM
                {
                    FirstName = doctor.User.FirstName,
                    LastName = doctor.User.LastName,
                    BirthDate = doctor.User.BirthDate,
                    Email = doctor.User.Email,
                    Gender = doctor.User.Gender,
                    PhoneNumber = doctor.User.PhoneNumber,
                    //ProfileImg = doctor.User.ProfileImg,
                };
                return View(DoctorDetail);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DoctorVM model)
        {
            try
            {
                var doctor = await _doctorService.GetDoctor(id);
                if (doctor == null)
                    return View("NotFound");

                //string wwwRootPath = _webHostEnvironment.WebRootPath;
                //string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                //string extension = Path.GetExtension(model.ImageFile.FileName);
                //model.ProfileImg = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                //string path = Path.Combine(wwwRootPath + "/Upload", fileName);

                //using (var fileStream = new FileStream(path, FileMode.Create))
                //{
                //    await model.ImageFile.CopyToAsync(fileStream);
                //}
                doctor.User.PhoneNumber = model.PhoneNumber;
                doctor.User.Email = model.Email;
                doctor.User.FirstName = model.FirstName;
                doctor.User.LastName = model.LastName;
                doctor.User.BirthDate = model.BirthDate;
                doctor.User.Gender = model.Gender;
                //doctor.User.ProfileImg = model.ProfileImg;

                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Doctor Updated successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                _toastNotification.AddErrorToastMessage("Doctor Updated Failed");
                return View();
            }
        }
        //Delete docotor 
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = _doctorService.Get_Doctor(id);
            if (doctor == null)
                return View("NotFound");
            //var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Upload", doctor.User.ProfileImg);
            try
            {
                //if (System.IO.File.Exists(imagePath))
                //    System.IO.File.Delete(imagePath);
                _doctorService.Delete(doctor);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage($"Doctor with ID {id} removed successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
    }
}
