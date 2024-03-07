using System.ComponentModel.DataAnnotations;

namespace HeartDiseasePrediction.ViewModel
{
	public class LoginVM
	{
		[Display(Name = "Email")]
		[Required(ErrorMessage = "Email Is Required")]
		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
