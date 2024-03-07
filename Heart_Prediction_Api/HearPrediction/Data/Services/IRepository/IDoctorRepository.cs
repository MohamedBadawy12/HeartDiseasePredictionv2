using Database.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.IRepository
{
	public interface IDoctorRepository
	{
		Task<IEnumerable<Doctor>> GetDectors();
		Task<IEnumerable<Doctor>> GetAvailableDoctors();
		Task<Doctor> GetDoctor(int id);
		Doctor FindDoctor(int id);
		//Task<NewDoctorDropDownViewModel> GetNewDoctorDropDownsValues();
		Task<IEnumerable<Doctor>> FilterDoctors(string search);
		Doctor Get_Doctor(int id);
		Task<Doctor> GetProfile(string userId);
		Task Add(Doctor doctor);
		void Delete(Doctor doctor);
	}
}
