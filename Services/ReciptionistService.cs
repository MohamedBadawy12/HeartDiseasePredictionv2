using Database.Entities;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class ReciptionistService : IReciptionistService
    {
        private readonly IReciptionistRepository _reciptionistRepository;
        public ReciptionistService(IReciptionistRepository reciptionistRepository)
        {
            _reciptionistRepository = reciptionistRepository;
        }
        public async Task Add(Reciptionist receptionist) => await _reciptionistRepository.Add(receptionist);

        public async Task<IEnumerable<Reciptionist>> FilterReciptionist(string search) =>
            await _reciptionistRepository.FilterReciptionist(search);

        public async Task<Reciptionist> GetProfile(string userId) => await _reciptionistRepository.GetProfile(userId);

        public async Task<Reciptionist> GetReciptionist(int id) => await _reciptionistRepository.GetReciptionist(id);

        public async Task<IEnumerable<Reciptionist>> GetReciptionists() => await _reciptionistRepository.GetReciptionists();

        public Reciptionist Get_Reciptionist(int id) => _reciptionistRepository.Get_Reciptionist(id);

        public void Remove(Reciptionist receptionist) => _reciptionistRepository.Remove(receptionist);
    }
}
