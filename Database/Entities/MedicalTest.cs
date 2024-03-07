using Microsoft.ML.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    public class MedicalTest
    {
        [Key]
        public int Id { get; set; }
        [LoadColumn(0)]
        public float Age { get; set; }
        [LoadColumn(1)]
        public float Sex { get; set; }
        [LoadColumn(2)]
        public float Cholesterol { get; set; }
        [LoadColumn(3)]
        public float BloodPresureAbove { get; set; }
        [LoadColumn(4)]
        public float BloodPresureDown { get; set; }
        [LoadColumn(5)]
        public float HeartRate { get; set; }
        [LoadColumn(6)]
        public float Diabets { get; set; }
        [LoadColumn(7)]
        public float FamilyHistory { get; set; }
        [LoadColumn(8)]
        public float Smoking { get; set; }
        [LoadColumn(9)]
        public float Obesity { get; set; }
        [LoadColumn(10)]
        public float AlcoholConsumption { get; set; }
        [LoadColumn(11)]
        public float ExcersiceHoursPerWeek { get; set; }
        [LoadColumn(12)]
        public float Diet { get; set; }
        [LoadColumn(13)]
        public float PreviousHeartProblems { get; set; }
        [LoadColumn(14)]
        public float MedicationUse { get; set; }
        [LoadColumn(15)]
        public float StressLevel { get; set; }
        [LoadColumn(16)]
        public float SedentaryHoursPerDay { get; set; }
        [LoadColumn(17)]
        public float BMI { get; set; }
        [LoadColumn(18)]
        public float Triglycerides { get; set; }
        [LoadColumn(19)]
        public float PhysicalActivityDaysPerWeek { get; set; }
        [LoadColumn(20)]
        public float SleepHoursPerDay { get; set; }
        [LoadColumn(21)]
        public bool Label { get; set; }
        //public int PredictionId { get; set; }
        //[ForeignKey(nameof(PredictionId))]
        //public Prediction prediction { get; set; }
        public long PatientSSN { get; set; }
        [ForeignKey(nameof(PatientSSN))]
        public Patient patient { get; set; }
        public int MedicalAnalystId { get; set; }
        [ForeignKey(nameof(MedicalAnalystId))]
        public MedicalAnalyst medicalAnalyst { get; set; }
        public DateTime date { get; set; }
        public MedicalTest()
        {
            date = DateTime.Now;
        }
    }
}
