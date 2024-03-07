using System;
using System.ComponentModel.DataAnnotations;

namespace HearPrediction.Api.DTO
{
	public class PrescriptionFormDTO
	{
		[Required]
		[Display(Name = "Medicine Name")]
		public string MedicineName { get; set; }
		public DateTime date { get; set; }
		public long PatientSSN { get; set; }
		public int DoctorId { get; set; }
		public PrescriptionFormDTO()
		{
			date = DateTime.Now;
		}
	}
}
