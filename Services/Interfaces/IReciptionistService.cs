using Database.Entities;

namespace Services.Interfaces
{
	public interface IReciptionistService
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
