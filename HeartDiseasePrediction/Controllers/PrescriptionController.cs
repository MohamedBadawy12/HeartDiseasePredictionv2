﻿using Database.Entities;
using HeartDiseasePrediction.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using Repositories;
using Repositories.ViewModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeartDiseasePrediction.Controllers
{
	public class PrescriptionController : Controller
	{
		private readonly IToastNotification _toastNotification;
		private readonly IUnitOfWork _unitOfWork;
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public PrescriptionController(IToastNotification toastNotification,
			AppDbContext context, IUnitOfWork unitOfWork)
		{
			_toastNotification = toastNotification;
			_unitOfWork = unitOfWork;
			_context = context;
		}

		//Get All Prescriptions By User ID
		[Authorize(Roles = "Doctor")]
		public async Task<IActionResult> Index()
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			string userRole = User.FindFirstValue(ClaimTypes.Role);
			var prescriptions = await _unitOfWork.prescriptions.GetPrescriptionByUserId(userId, userRole);
			return View(prescriptions);
		}

		//Get All Prescriptions
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetPrescriptions()
		{
			string PatientEmail = User.FindFirstValue(ClaimTypes.Email);
			string userRole = User.FindFirstValue(ClaimTypes.Role);
			var prescriptions = await _unitOfWork.prescriptions.GetPrescriptionByEmail(PatientEmail, userRole);
			return View(prescriptions);
		}
		//Search for Prescriptions
		[HttpPost]
		public async Task<IActionResult> Index(long search)
		{
			var prescriptions = await _unitOfWork.prescriptions.FilterPrescriptions(search);
			return View(prescriptions);
		}

		//get Prescription details
		[AllowAnonymous]
		public async Task<IActionResult> PrescriptionDetails(int id)
		{
			try
			{
				var prescription = await _unitOfWork.prescriptions.GetPrescription(id);
				if (prescription == null)
					return View("NotFound");

				var prescriptionVM = new PrescriptionVM
				{
					MedicineName = prescription.MedicineName,
					PatientSSN = prescription.PatientSSN,
					date = prescription.date,
					ApDoctorId = prescription.ApDoctorId,
					DoctorEmail = prescription.DoctorEmail,
					PatientEmail = prescription.PatientEmail,
					DoctorId = prescription.DoctorId,
					DoctorName = prescription.Doctor.Name,
				};
				return View(prescriptionVM);
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return View();
			}
		}

		//Create Prescriptions
		[Authorize(Roles = "Doctor")]
		public async Task<IActionResult> Create()
		{
			var doctorDropDownList = await _unitOfWork.prescriptions.GetDoctorDropDownsValues();
			ViewBag.Doctor = new SelectList(doctorDropDownList.doctors, "Id", "Name");
			return View();
		}
		[Authorize(Roles = "Doctor")]
		[HttpPost]
		public async Task<IActionResult> Create(PrescriptionViewModel model)
		{
			try
			{
				var doctorDropDownList = await _unitOfWork.prescriptions.GetDoctorDropDownsValues();
				ViewBag.Doctor = new SelectList(doctorDropDownList.doctors, "Id", "Name");
				var patient = await _unitOfWork.Patients.GetPatient(model.PatientSSN);
				if (patient == null)
					return View("NotFound");
				string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				string doctorEmail = User.FindFirstValue(ClaimTypes.Email);
				model.ApDoctorId = userId;
				model.DoctorEmail = doctorEmail;
				await _unitOfWork.prescriptions.AddPrescriptionAsync(model);
				_toastNotification.AddSuccessToastMessage("Prescription Created successfully");
				return View("CompletedSuccessfully");
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				_toastNotification.AddErrorToastMessage("An error occurred while saving the prescription.");
				return View(model);
			}
		}

		//Edit details of Prescription
		[Authorize(Roles = "Doctor")]
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			try
			{
				var prescription = await _unitOfWork.prescriptions.GetPrescription(id);
				if (prescription == null)
					return View("NotFound");
				var doctorDropDownList = await _unitOfWork.prescriptions.GetDoctorDropDownsValues();
				ViewBag.Doctor = new SelectList(doctorDropDownList.doctors, "Id", "Name");
				var prescriptionVM = new Prescription
				{
					MedicineName = prescription.MedicineName,
					PatientSSN = prescription.PatientSSN,
					date = prescription.date,
					ApDoctorId = prescription.ApDoctorId,
					DoctorEmail = prescription.DoctorEmail,
					PatientEmail = prescription.PatientEmail,
					DoctorId = prescription.DoctorId,
				};
				return View(prescriptionVM);
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return View();
			}
		}
		[Authorize(Roles = "Doctor")]
		[HttpPost]
		public async Task<IActionResult> Edit(int id, Prescription model)
		{
			try
			{
				var prescription = await _unitOfWork.prescriptions.GetPrescription(id);
				if (prescription == null)
					return View("NotFound");

				var doctorDropDownList = await _unitOfWork.prescriptions.GetDoctorDropDownsValues();
				ViewBag.Doctor = new SelectList(doctorDropDownList.doctors, "Id", "Name");
				prescription.MedicineName = model.MedicineName;
				prescription.PatientSSN = model.PatientSSN;
				prescription.date = model.date;
				prescription.DoctorId = model.DoctorId;
				prescription.ApDoctorId = model.ApDoctorId;
				prescription.DoctorEmail = model.DoctorEmail;
				prescription.PatientEmail = model.PatientEmail;
				//prescription.Doctor = model.Doctor;

				_context.Prescriptions.Update(prescription);
				await _unitOfWork.Complete();
				_toastNotification.AddSuccessToastMessage("Prescription Updated successfully");
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				_toastNotification.AddErrorToastMessage("Prescription Updated Failed");
				return View();
			}
		}

		//Delete Prescription 
		[Authorize(Roles = "Doctor")]
		public IActionResult Delete(int id)
		{
			var isDeleted = _unitOfWork.prescriptions.Delete(id);
			return isDeleted ? Ok() : BadRequest();
		}
	}
}
