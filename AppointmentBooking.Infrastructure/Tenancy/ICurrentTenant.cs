using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.Tenancy
{
    public interface ICurrentTenant
    {
        int? TenantId { get; set; }
        string? Slug { get; set; }
        string? BusinessName { get; set; }
        string? PrimaryColor { get; set; }
        string? SecondaryColor { get; set; }

    }
}
