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
	public class MedicalAnalystController : Controller
	{
		private readonly IToastNotification _toastNotification;
		Uri baseAddress = new Uri("https://localhost:44304/api");
		HttpClient _client;
		public MedicalAnalystController(IToastNotification toastNotification)
		{
			_toastNotification = toastNotification;
			_client = new HttpClient();
			_client.BaseAddress = baseAddress;
		}
		//get all Medical Analysts in list
		public async Task<IActionResult> Index()
		{
			var accessToken = HttpContext.Session.GetString("JWToken");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			List<MedicalAnalyst> medicalAnalystViewModel = new List<MedicalAnalyst>();
			HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/MedicalAnalyst").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = await response.Content.ReadAsStringAsync();
				medicalAnalystViewModel = JsonConvert.DeserializeObject<List<MedicalAnalyst>>(data);
			}
			return View(medicalAnalystViewModel);
		}
		[HttpPost]
		public async Task<IActionResult> Index(string search)
		{
			var accessToken = HttpContext.Session.GetString("JWToken");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			List<MedicalAnalyst> medicalAnalystViewModel = new List<MedicalAnalyst>();
			HttpResponseMessage response = _client.GetAsync(_client.BaseAddress +
				$"/MedicalAnalyst/Search?search={search}").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = await response.Content.ReadAsStringAsync();
				medicalAnalystViewModel = JsonConvert.DeserializeObject<List<MedicalAnalyst>>(data);
			}
			return View(medicalAnalystViewModel);
		}
		public async Task<IActionResult> MedicalAnalystDetails(int id)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress +
					$"/MedicalAnalyst/GetMedicalAnalystById?id={id}");
				if (response.IsSuccessStatusCode)
				{
					string data = response.Content.ReadAsStringAsync().Result;
					var medicalAnalyst = JsonConvert.DeserializeObject<MedicalAnalystVM>(data);
					return View(medicalAnalyst);
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
		//Edit details of Medical Analyst
		[HttpGet]
		public async Task<IActionResult> EditMedicalAnalyst(int id)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress +
					$"/MedicalAnalyst/GetMedicalAnalystById?id={id}");
				if (response.IsSuccessStatusCode)
				{
					string data = response.Content.ReadAsStringAsync().Result;
					var medicalAnalyst = JsonConvert.DeserializeObject<MedicalAnalystVM>(data);
					return View(medicalAnalyst);
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
		public async Task<IActionResult> EditMedicalAnalyst(int id, MedicalAnalystVM model)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				string data = JsonConvert.SerializeObject(model);
				StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
				HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress +
					$"/MedicalAnalyst/EditMedicalAnalyst?id={id}", content);
				if (response.IsSuccessStatusCode)
				{
					TempData["successMessage"] = "Medical Analyst Details Updated.";
					_toastNotification.AddSuccessToastMessage("Medical Analyst Updated successfully");
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

		//Delete Medical Analyst 
		public async Task<IActionResult> DeleteMedicalAnalyst(int id)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = await _client.DeleteAsync(_client.BaseAddress +
					$"/MedicalAnalyst/{id}");
				if (response.IsSuccessStatusCode)
				{
					TempData["successMessage"] = "Medical Analyst Details Updated.";
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
