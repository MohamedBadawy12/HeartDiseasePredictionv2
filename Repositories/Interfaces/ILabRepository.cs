using Database.Entities;

namespace Repositories.Interfaces
{
    public interface ILabRepository
    {
        Task<IEnumerable<Lab>> GetLabs();
        Task<Lab> GetLab(int id);
        Lab Get_Lab(int id);
        Task AddAsync(Lab lab);
        void Delete(Lab lab);
    }
}
