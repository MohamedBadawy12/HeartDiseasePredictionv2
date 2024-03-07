using Database.Entities;
using HeartDiseasePrediction.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HeartDiseasePrediction.Controllers
{
	public class PatientController : Controller
	{
		private readonly IToastNotification _toastNotification;
		Uri baseAddress = new Uri("https://localhost:44304/api");
		HttpClient _client;
		public PatientController(IToastNotification toastNotification)
		{
			_toastNotification = toastNotification;
			_client = new HttpClient();
			_client.BaseAddress = baseAddress;
		}
		//get all Patients in list
		public async Task<IActionResult> Index()
		{
			var accessToken = HttpContext.Session.GetString("JWToken");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			List<Patient> patientViewModel = new List<Patient>();
			HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Patient").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = await response.Content.ReadAsStringAsync();
				patientViewModel = JsonConvert.DeserializeObject<List<Patient>>(data);
			}
			return View(patientViewModel);
		}
		[HttpPost]
		public async Task<IActionResult> Index(string search)
		{
			var accessToken = HttpContext.Session.GetString("JWToken");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			List<Patient> patientViewModel = new List<Patient>();
			HttpResponseMessage response = _client.GetAsync(_client.BaseAddress +
				$"/Patient/Search?search={search}").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = await response.Content.ReadAsStringAsync();
				patientViewModel = JsonConvert.DeserializeObject<List<Patient>>(data);
			}
			return View(patientViewModel);
		}
		//get Patient details
		public async Task<IActionResult> PatientDetails(long ssn)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress +
					$"/Patient/GetPatientBySSN?ssn={ssn}");
				if (response.IsSuccessStatusCode)
				{
					string data = response.Content.ReadAsStringAsync().Result;
					var patient = JsonConvert.DeserializeObject<PatientVM>(data);
					return View(patient);
				}
				else
				{
					return RedirectToAction("Index");
				}

			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return View();
			}
		}
		//Edit details of Patient
		[HttpGet]
		public async Task<IActionResult> EditPatient(long ssn)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress +
					$"/Patient/GetPatientBySSN?ssn={ssn}");
				if (response.IsSuccessStatusCode)
				{
					string data = response.Content.ReadAsStringAsync().Result;
					var patient = JsonConvert.DeserializeObject<PatientVM>(data);
					return View(patient);
				}
				else
				{
					return RedirectToAction("Index");
				}
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return View();
			}
		}
		[HttpPost]
		public async Task<IActionResult> EditPatient(long ssn, PatientVM model)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				string data = JsonConvert.SerializeObject(model);
				StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
				HttpResponseMessage response = _client.PutAsync(_client.BaseAddress +
					$"/Patient/EditPatient?ssn={ssn}", content).Result;
				if (response.IsSuccessStatusCode)
				{
					TempData["successMessage"] = "Patient Details Updated.";
					_toastNotification.AddSuccessToastMessage("Patient Updated successfully");
					return RedirectToAction("Index");
				}
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return View();
			}
			return View();
		}

		//Delete Patient 
		public async Task<IActionResult> DeletePatient(long ssn)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = await _client.DeleteAsync(_client.BaseAddress +
					$"/Patient/{ssn}");
				if (response.IsSuccessStatusCode)
				{
					TempData["successMessage"] = "Patient Details Deleted.";
					return RedirectToAction("Index");
				}
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return View();
			}
			return View();
		}
	}
}
