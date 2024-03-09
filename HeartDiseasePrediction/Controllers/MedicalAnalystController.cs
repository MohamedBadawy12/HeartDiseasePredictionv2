﻿using Database.Entities;
using HeartDiseasePrediction.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace HeartDiseasePrediction.Controllers
{
    public class MedicalAnalystController : Controller
    {
        private readonly IToastNotification _toastNotification;
        private readonly IMedicalAnalystService _medicalAnalystService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MedicalAnalystController(IToastNotification toastNotification, IMedicalAnalystService medicalAnalystService
            , AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _toastNotification = toastNotification;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _medicalAnalystService = medicalAnalystService;
        }
        //get all Medical Analysts in list
        public async Task<IActionResult> Index()
        {
            var medicalAnalyst = await _medicalAnalystService.GetMedicalAnalysts();
            return View(medicalAnalyst);
        }
        //search
        [HttpPost]
        public async Task<IActionResult> Index(string search)
        {
            var medicalAnalyst = await _medicalAnalystService.FilterMedicalAnalyst(search);
            return View(medicalAnalyst);
        }
        //Medical Analyst Details
        public async Task<IActionResult> MedicalAnalystDetails(int id)
        {
            try
            {
                var medicalAnalyst = await _medicalAnalystService.GetMedicalAnalyst(id);
                if (medicalAnalyst == null)
                    return View("NotFound");

                var medicalAnalystVM = new MedicalAnalystVM
                {
                    FirstName = medicalAnalyst.User.FirstName,
                    LastName = medicalAnalyst.User.LastName,
                    BirthDate = medicalAnalyst.User.BirthDate,
                    Email = medicalAnalyst.User.Email,
                    Gender = medicalAnalyst.User.Gender,
                    PhoneNumber = medicalAnalyst.User.PhoneNumber,
                    ProfileImg = medicalAnalyst.User.ProfileImg,
                };
                return View(medicalAnalystVM);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        //Edit details of Medical Analyst
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var medicalAnalyst = await _medicalAnalystService.GetMedicalAnalyst(id);
                if (medicalAnalyst == null)
                    return View("NotFound");

                var medicalAnalystVM = new MedicalAnalystVM
                {
                    FirstName = medicalAnalyst.User.FirstName,
                    LastName = medicalAnalyst.User.LastName,
                    BirthDate = medicalAnalyst.User.BirthDate,
                    Email = medicalAnalyst.User.Email,
                    Gender = medicalAnalyst.User.Gender,
                    PhoneNumber = medicalAnalyst.User.PhoneNumber,
                    ProfileImg = medicalAnalyst.User.ProfileImg,
                    LabId = medicalAnalyst.LabId,
                };
                return View(medicalAnalystVM);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, MedicalAnalystVM model)
        {
            try
            {
                var medicalAnalyst = await _medicalAnalystService.GetMedicalAnalyst(id);
                if (medicalAnalyst == null)
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
                medicalAnalyst.User.PhoneNumber = model.PhoneNumber;
                medicalAnalyst.User.Email = model.Email;
                medicalAnalyst.User.FirstName = model.FirstName;
                medicalAnalyst.User.LastName = model.LastName;
                medicalAnalyst.User.BirthDate = model.BirthDate;
                medicalAnalyst.User.Gender = model.Gender;
                medicalAnalyst.LabId = model.LabId;
                //doctor.User.ProfileImg = model.ProfileImg;

                _context.MedicalAnalysts.Update(medicalAnalyst);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("MedicalAnalyst Updated successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                _toastNotification.AddErrorToastMessage("MedicalAnalyst Updated Failed");
                return View();
            }
        }

        //Delete Medical Analyst 
        public async Task<IActionResult> DeleteMedicalAnalyst(int id)
        {
            var medicalAnalyst = _medicalAnalystService.Get_MedicalAnalyst(id);
            if (medicalAnalyst == null)
                return View("NotFound");
            //var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Upload", medicalAnalyst.User.ProfileImg);
            try
            {
                //if (System.IO.File.Exists(imagePath))
                //    System.IO.File.Delete(imagePath);
                _medicalAnalystService.Remove(medicalAnalyst);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage($"MedicalAnalyst with ID {id} removed successfully");
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
