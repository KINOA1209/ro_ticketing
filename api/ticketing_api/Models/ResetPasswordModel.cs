using System.ComponentModel.DataAnnotations;

namespace ticketing_api.Models
{
    public class ResetPasswordModel
    {
        public string ResetPasswordToken { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
