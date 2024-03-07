using Database.Entities;
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
	public class PrescriptionController : Controller
	{
		private readonly IToastNotification _toastNotification;
		Uri baseAddress = new Uri("https://localhost:44304/api");
		HttpClient _client;
		public PrescriptionController(IToastNotification toastNotification)
		{
			_toastNotification = toastNotification;
			_client = new HttpClient();
			_client.BaseAddress = baseAddress;
		}
		public async Task<IActionResult> Index()
		{
			var accessToken = HttpContext.Session.GetString("JWToken");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			List<Prescription> prescriptionViewModel = new List<Prescription>();
			HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Prescription").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = await response.Content.ReadAsStringAsync();
				prescriptionViewModel = JsonConvert.DeserializeObject<List<Prescription>>(data);
			}
			return View(prescriptionViewModel);
		}
		[HttpPost]
		public async Task<IActionResult> Index(string search)
		{
			var accessToken = HttpContext.Session.GetString("JWToken");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			List<Prescription> prescriptionViewModel = new List<Prescription>();
			HttpResponseMessage response = _client.GetAsync(_client.BaseAddress +
				$"/Prescription/Search?search={search}").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = await response.Content.ReadAsStringAsync();
				prescriptionViewModel = JsonConvert.DeserializeObject<List<Prescription>>(data);
			}
			return View(prescriptionViewModel);
		}
		//get Prescription details
		public async Task<IActionResult> PrescriptionDetails(int id)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				Prescription PrescriptionViewModel = new Prescription();
				HttpResponseMessage response = _client.GetAsync(_client.BaseAddress +
					$"/Prescription/GetPrescriptionById?id={id}").Result;
				if (response.IsSuccessStatusCode)
				{
					string data = response.Content.ReadAsStringAsync().Result;
					PrescriptionViewModel = JsonConvert.DeserializeObject<Prescription>(data);
				}
				return View(PrescriptionViewModel);
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return View();
			}
		}

		public async Task<IActionResult> Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(Prescription model)
		{
			var accessToken = HttpContext.Session.GetString("JWToken");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			string data = JsonConvert.SerializeObject(model);
			StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress
				+ "/Prescription/Create", content);
			if (response.IsSuccessStatusCode)
			{
				_toastNotification.AddSuccessToastMessage("Prescription Created successfully");
				return RedirectToAction("Index");
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Error In Creating Prescription");
			}
			return View(model);
		}

		//Edit details of Prescription
		[HttpGet]
		public async Task<IActionResult> EditPrescription(int id)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress +
					$"/Prescription/GetPrescriptionById?id={id}");
				if (response.IsSuccessStatusCode)
				{
					string data = response.Content.ReadAsStringAsync().Result;
					var prescription = JsonConvert.DeserializeObject<Prescription>(data);
					return View(prescription);
				}
				else
				{
					return RedirectToAction("PrescriptionDetails");
				}
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return View();
			}
		}
		[HttpPost]
		public async Task<IActionResult> EditPrescription(int id, Prescription model)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				string data = JsonConvert.SerializeObject(model);
				StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
				HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress +
					$"/Prescription/EditPrescription?id={id}", content);
				if (response.IsSuccessStatusCode)
				{
					TempData["successMessage"] = "Prescription Details Updated.";
					_toastNotification.AddSuccessToastMessage("Prescription Updated successfully");
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
		public async Task<IActionResult> DeletePrescription(int id)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress +
					"/Prescription/" + id).Result;
				if (response.IsSuccessStatusCode)
				{
					TempData["successMessage"] = "Prescription Details Deleted.";
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
