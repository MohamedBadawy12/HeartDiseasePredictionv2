using Database.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.IRepository
{
	public interface ILabRepository
	{
		Task<IEnumerable<Lab>> GetLabs();
	}
}
