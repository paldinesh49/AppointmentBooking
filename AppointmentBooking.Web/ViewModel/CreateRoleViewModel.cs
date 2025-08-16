using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.Web.ViewModel
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Role name is required.")]
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }
    }
}
