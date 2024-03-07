using Database.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services.IRepository
{
	public interface IPrescriptionRepository
	{
		Task<IEnumerable<Prescription>> GetPrescriptions();
		Task<List<Prescription>> GetPrescriptionsByUserSSN(long ssn);
		Task<IEnumerable<Prescription>> FilterPrescriptions(long search);
		Task<Prescription> GetPrescription(int id);
		Prescription Get_Prescription(int id);
		//IQueryable<Prescription> FilterPrescriptionsAsync(long patientSSN,AppointmentSearchDto searchDto);
		Task Add(Prescription prescription);
		void Remove(Prescription prescription);
	}
}
