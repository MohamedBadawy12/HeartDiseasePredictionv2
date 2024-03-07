using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public DateTime date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        [MaxLength(300)]
        public string Detail { get; set; }
        public bool Status { get; set; }
        public long? PatientSSN { get; set; }
        [ForeignKey(nameof(PatientSSN))]
        public virtual Patient Patient { get; set; }
        public int DoctorId { get; set; }
        [ForeignKey(nameof(DoctorId))]
        public virtual Doctor Doctor { get; set; }
    }
}
