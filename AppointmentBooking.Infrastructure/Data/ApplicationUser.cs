using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.Data
{
    public class ApplicationUser : IdentityUser
    {
        
        public int? TenantId { get; set; }    // null for SuperAdmin accounts
    }
}
