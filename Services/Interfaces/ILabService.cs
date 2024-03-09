using Database.Entities;

namespace Services.Interfaces
{
    public interface ILabService
    {
        Task<IEnumerable<Lab>> GetLabs();
        Task<Lab> GetLab(int id);
        Lab Get_Lab(int id);
        Task AddAsync(Lab lab);
        void Delete(Lab lab);
    }
}
