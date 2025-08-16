using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.Tenancy
{
    public class CurrentTenant : ICurrentTenant
    {
        public int? TenantId { get; set; }
        public string? Slug { get; set; }
       public string? BusinessName { get; set; }
       public string? PrimaryColor { get; set; }
       public string? SecondaryColor { get; set; }
    }
}
