using Database.Entities;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class LabService : ILabService
    {
        private readonly ILabRepository _labRepository;
        public LabService(ILabRepository labRepository)
        {
            _labRepository = labRepository;
        }
        public async Task AddAsync(Lab lab) => await _labRepository.AddAsync(lab);

        public void Delete(Lab lab) => _labRepository.Delete(lab);

        public async Task<Lab> GetLab(int id) => await _labRepository.GetLab(id);

        public async Task<IEnumerable<Lab>> GetLabs() => await _labRepository.GetLabs();

        public Lab Get_Lab(int id) => _labRepository.Get_Lab(id);
    }
}
