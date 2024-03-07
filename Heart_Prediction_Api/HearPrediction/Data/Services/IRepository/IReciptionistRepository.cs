using Database.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.IRepository
{
	public interface IReciptionistRepository
	{
		Task<IEnumerable<Reciptionist>> GetReciptionists();
		Task<Reciptionist> GetReciptionist(int id);
		Reciptionist Get_Reciptionist(int id);
		Task<IEnumerable<Reciptionist>> FilterReciptionist(string search);
		Task<Reciptionist> GetProfile(string userId);
		Task Add(Reciptionist receptionist);
		void Remove(Reciptionist receptionist);
	}
}
