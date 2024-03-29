﻿using Database.Entities;
using HeartDiseasePrediction.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Repositories;
using Repositories.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeartDiseasePrediction.Controllers
{
	public class AppointmentController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IToastNotification _toastNotification;

		public AppointmentController(IUnitOfWork unitOfWork, IToastNotification toastNotification,
			IFileRepository fileRepository)
		{
			_unitOfWork = unitOfWork;
			_toastNotification = toastNotification;
		}

		//Get All Appointments by User ID
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Index()
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			string userRole = User.FindFirstValue(ClaimTypes.Role);
			var appointments = await _unitOfWork.appointments.GetAppointmenByUserId(userId, userRole);
			return View(appointments);
		}

		//Get All Appointments by Email
		[Authorize(Roles = "Doctor")]
		public async Task<IActionResult> GetAppointmentByEmail()
		{
			string doctorEmail = User.FindFirstValue(ClaimTypes.Email);
			string userRole = User.FindFirstValue(ClaimTypes.Role);
			var appointments = await _unitOfWork.appointments.GetAppointmenByEmail(doctorEmail, userRole);
			return View(appointments);
		}

		//Get All Appointments by User ID
		[Authorize(Roles = "Doctor")]
		public async Task<IActionResult> GetMessageById()
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			string userRole = User.FindFirstValue(ClaimTypes.Role);
			var messages = await _unitOfWork.appointments.GetMessageByUserId(userId, userRole);
			return View(messages);
		}

		//Get All Appointments by Email
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetMessagetByEmail()
		{
			string patientEmail = User.FindFirstValue(ClaimTypes.Email);
			string userRole = User.FindFirstValue(ClaimTypes.Role);
			var messages = await _unitOfWork.appointments.GetMessageByEmail(patientEmail, userRole);
			return View(messages);
		}

		//Create Appointment
		public async Task<IActionResult> Create(int id)
		{
			var doctor = await _unitOfWork.Doctors.GetDoctor(id);
			if (doctor == null)
				return View("NotFound");
			var DoctorDetail = new BookAppointmentViewModel
			{
				FirstName = doctor.User.FirstName,
				LastName = doctor.User.LastName,
				BirthDate = doctor.User.BirthDate,
				Email = doctor.User.Email,
				Gender = doctor.User.Gender,
				PhoneNumber = doctor.User.PhoneNumber,
				Name = doctor.Name,
				Location = doctor.Location,
				Price = doctor.Price,
				//ProfileImg = doctor.User.ProfileImg,
			};
			return View(DoctorDetail);
		}
		[Authorize(Roles = "User")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(int id, BookAppointmentViewModel model)
		{
			var doctor = await _unitOfWork.Doctors.GetDoctor(id);
			if (doctor == null)
				return View("NotFound");

			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			string patientEmail = User.FindFirstValue(ClaimTypes.Email);
			model.PatientID = userId;
			model.PatientEmail = patientEmail;
			var appointment = new Appointment()
			{
				PatientID = model.PatientID,
				PateintName = model.PateintName,
				PatientEmail = model.PatientEmail,
				DoctorEmail = doctor.User.Email,
				date = model.Date,
				Time = model.Time,
				PhoneNumber = model.PatientPhone,
				PatientSSN = model.PatientSSN,
				DoctorId = doctor.Id,
			};

			await _unitOfWork.appointments.AddAsync(appointment);
			await _unitOfWork.Complete();
			_toastNotification.AddSuccessToastMessage("Appointment Created Successfully");
			return View("CompletedSuccessfully");
		}

		//Accept Appointment
		[Authorize(Roles = "Doctor")]
		public async Task<IActionResult> AcceptsAppointment(int id, MessageViewModel model)
		{
			var appointment = await _unitOfWork.appointments.GetAppointment(id);
			if (appointment == null)
				return View("NotFound");
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			string doctorEmail = User.FindFirstValue(ClaimTypes.Email);
			model.DoctorId = userId;
			model.DoctorEmail = doctorEmail;
			var message = new Message
			{
				Messages = "Your Appointment is Accepted",
				Date = DateTime.Now,
				PatientEmail = appointment.PatientEmail,
				DoctorEmail = doctorEmail,
				DoctorId = userId,
			};
			await _unitOfWork.appointments.AddMessageAsync(message);
			await _unitOfWork.Complete();
			_toastNotification.AddSuccessToastMessage($"Message has sent successfully");
			return RedirectToAction("Index");
		}

		//Cancel Appointment by doctor
		[Authorize(Roles = "Doctor")]
		public async Task<IActionResult> CanceledAppointment(int id, MessageViewModel model)
		{
			var appointment = _unitOfWork.appointments.Get_Appointment(id);
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			string doctorEmail = User.FindFirstValue(ClaimTypes.Email);
			model.DoctorId = userId;
			model.DoctorEmail = doctorEmail;
			var message = new Message
			{
				Messages = "Sorry,Your Appointment is Canceled because Doctor is busy in this time",
				Date = model.Date,
				PatientEmail = appointment.PatientEmail,
				DoctorEmail = doctorEmail,
				DoctorId = userId,
			};
			await _unitOfWork.appointments.AddMessageAsync(message);
			await _unitOfWork.Complete();
			var isDeleted = _unitOfWork.appointments.Canceled(id);
			return isDeleted ? Ok() : BadRequest();
		}

		//Cancel appointment by user
		[Authorize(Roles = "User")]
		public IActionResult Canceled(int id)
		{
			var isDeleted = _unitOfWork.appointments.Canceled(id);
			return isDeleted ? Ok() : BadRequest();
		}


		[Authorize(Roles = "User")]
		public async Task<IActionResult> DoctorDetailsWithAppointment(int id)
		{
			try
			{
				var doctor = await _unitOfWork.Doctors.GetDoctor(id);
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
					Name = doctor.Name,
					Location = doctor.Location,
					Price = doctor.Price,
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

	}
}
