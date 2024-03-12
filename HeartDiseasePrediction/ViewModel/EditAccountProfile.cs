using Database.Entities;
using System.ComponentModel.DataAnnotations;

namespace HeartDiseasePrediction.ViewModel
{
    public class EditAccountProfile : ApplicationUser
    {
        [Display(Name = "SSN")]
        public long SSN { get; set; }
        [Display(Name = "Insurance Number")]
        public int Insurance_No { get; set; }
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [StringLength(250)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords Not Match")]
        public string ConfirmPassword { get; set; }
    }
}
