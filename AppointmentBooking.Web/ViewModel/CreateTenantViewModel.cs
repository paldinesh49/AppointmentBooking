using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.Web.ViewModel
{
    public class CreateTenantViewModel
    {
        [Required]
        [Display(Name = "Business Name")]
        [StringLength(100, ErrorMessage = "{0} must be between {2} and {1} characters.", MinimumLength = 2)]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Slug (Unique URL)")]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug must contain only lowercase letters, numbers, and hyphens.")]
        [StringLength(50)]
        public string Slug { get; set; } = string.Empty;

        [Display(Name = "Logo URL")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? LogoUrl { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Owner Email")]
        public string OwnerEmail { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
