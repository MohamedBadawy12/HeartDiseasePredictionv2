﻿using HearPrediction.Api.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HearPrediction.Api.Controllers
{
	[Authorize(Roles = "Admin")]
	[Route("api/[controller]")]
	[ApiController]
	public class DoctorController : ControllerBase
	{

		private readonly IUnitOfWork _unitOfWork;

		private readonly IWebHostEnvironment _webHostEnvironment;
		public DoctorController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}
		//Get All Doctors from db
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetAllDoctors()
		{
			var doctors = await _unitOfWork.Doctors.GetDoctors();
			return Ok(doctors);
		}

		//Get Doctor details from db
		[HttpGet("GetDoctorById")]
		public async Task<IActionResult> GetDoctorDetails(int id)
		{
			var doctor = await _unitOfWork.Doctors.GetDoctor(id);
			if (doctor == null)
				return NotFound($"No doctor was found with Id: {id}");

			var DoctorDetail = new DoctorFormDTO
			{
				FirstName = doctor.User.FirstName,
				LastName = doctor.User.LastName,
				BirthDate = doctor.User.BirthDate,
				Name = doctor.User.Name,
				Location = doctor.User.Location,
				Price = doctor.User.Price,
				Email = doctor.User.Email,
				Gender = doctor.User.Gender,
				PhoneNumber = doctor.User.PhoneNumber,
				ProfileImg = doctor.User.ProfileImg,
			};
			return Ok(DoctorDetail);
		}

		//Search for doctor
		[HttpGet("Search")]
		public async Task<IActionResult> SearchForDoctor([FromQuery] string search)
		{
			try
			{
				var result = await _unitOfWork.Doctors.FilterDoctors(search);
				return Ok(result);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Error in searchin for data");
			}
		}

		//Edit Doctor 
		[HttpPut("EditDoctor")]
		public async Task<IActionResult> EditDoctor(int id, [FromForm] DoctorFormDTO model)
		{
			var doctor = await _unitOfWork.Doctors.GetDoctor(id);
			if (doctor == null)
				return NotFound($"No doctor was found with Id: {id}");

			string wwwRootPath = _webHostEnvironment.WebRootPath;
			string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
			string extension = Path.GetExtension(model.ImageFile.FileName);
			model.ProfileImg = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
			string path = Path.Combine(wwwRootPath + "/Upload", fileName);

			using (var fileStream = new FileStream(path, FileMode.Create))
			{
				await model.ImageFile.CopyToAsync(fileStream);
			}

			doctor.User.PhoneNumber = model.PhoneNumber;
			doctor.User.Email = model.Email;
			doctor.User.Name = model.Name;
			doctor.User.Location = model.Location;
			doctor.User.Price = model.Price;
			doctor.User.FirstName = model.FirstName;
			doctor.User.LastName = model.LastName;
			doctor.User.BirthDate = model.BirthDate;
			doctor.User.Gender = model.Gender;
			doctor.User.ProfileImg = model.ProfileImg;

			await _unitOfWork.Complete();
			return Ok(doctor);
		}

		//Delete Doctor
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var doctor = _unitOfWork.Doctors.Get_Doctor(id);
			if (doctor == null)
				return NotFound($"No doctor was found with Id: {id}");
			//var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Upload", doctor.User.ProfileImg);
			try
			{
				//if (System.IO.File.Exists(imagePath))
				//	System.IO.File.Delete(imagePath);
				_unitOfWork.Doctors.Delete(doctor);
				await _unitOfWork.Complete();
				return Ok($"Doctor with ID {id} removed successfully");
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
			}
		}
	}
}
