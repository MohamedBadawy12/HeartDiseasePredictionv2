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
	public class ReciptionistController : Controller
	{
		private readonly IToastNotification _toastNotification;
		Uri baseAddress = new Uri("https://localhost:44304/api");
		HttpClient _client;
		public ReciptionistController(IToastNotification toastNotification)
		{
			_toastNotification = toastNotification;
			_client = new HttpClient();
			_client.BaseAddress = baseAddress;
		}
		//get all Reciptionists in list
		public async Task<IActionResult> Index()
		{
			var accessToken = HttpContext.Session.GetString("JWToken");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			List<Reciptionist> ReciptionistViewModel = new List<Reciptionist>();
			HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Reciptionist").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = await response.Content.ReadAsStringAsync();
				ReciptionistViewModel = JsonConvert.DeserializeObject<List<Reciptionist>>(data);
			}
			return View(ReciptionistViewModel);
		}
		[HttpPost]
		public async Task<IActionResult> Index(string search)
		{
			var accessToken = HttpContext.Session.GetString("JWToken");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			List<Reciptionist> ReciptionistViewModel = new List<Reciptionist>();
			HttpResponseMessage response = _client.GetAsync(_client.BaseAddress +
				$"/Reciptionist/Search?search={search}").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = await response.Content.ReadAsStringAsync();
				ReciptionistViewModel = JsonConvert.DeserializeObject<List<Reciptionist>>(data);
			}
			return View(ReciptionistViewModel);
		}
		//get Reciptionist details
		public async Task<IActionResult> ReciptionistDetails(int id)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = _client.GetAsync(_client.BaseAddress +
					$"/Reciptionist/GetReciptionistById?id={id}").Result;
				if (response.IsSuccessStatusCode)
				{
					string data = response.Content.ReadAsStringAsync().Result;
					var reciptionist = JsonConvert.DeserializeObject<ReciptionistVM>(data);
					return View(reciptionist);
				}
				else
				{
					return RedirectToAction("ReciptionistDetails");
				}

			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return View();
			}
		}
		//Edit details of Reciptionist
		[HttpGet]
		public async Task<IActionResult> EditReciptionist(int id)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress +
					$"/Reciptionist/GetReciptionistById?id={id}");
				if (response.IsSuccessStatusCode)
				{
					string data = response.Content.ReadAsStringAsync().Result;
					var reciptionist = JsonConvert.DeserializeObject<ReciptionistVM>(data);
					return View(reciptionist);
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
		public async Task<IActionResult> EditReciptionist(int id, ReciptionistVM model)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				string data = JsonConvert.SerializeObject(model);
				StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
				HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress +
					$"/Reciptionist/EditReciptionist?id={id}", content);
				if (response.IsSuccessStatusCode)
				{
					TempData["successMessage"] = "Reciptionist Details Updated.";
					_toastNotification.AddSuccessToastMessage("Reciptionist Updated successfully");
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

		//Delete Reciptionist 
		public async Task<IActionResult> DeleteReciptionist(int id)
		{
			try
			{
				var accessToken = HttpContext.Session.GetString("JWToken");
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				HttpResponseMessage response = await _client.DeleteAsync(_client.BaseAddress +
					$"/Reciptionist/{id}");
				if (response.IsSuccessStatusCode)
				{
					TempData["successMessage"] = "Reciptionist Details Deleted.";
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
