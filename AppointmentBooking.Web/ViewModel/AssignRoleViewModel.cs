using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.Web.ViewModel
{
    public class AssignRoleViewModel
    {
        [Required(ErrorMessage = "User is required")]
        [Display(Name = "Select User")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Select Role")]
        public string? RoleName { get; set; }

        // Optional: For dropdowns in the view
        public List<UserItem> Users { get; set; } = new List<UserItem>();
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UserItem
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
    }
}
