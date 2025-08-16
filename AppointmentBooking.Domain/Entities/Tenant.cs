using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Domain.Entities
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;        // unique URL slug
        public string? LogoUrl { get; set; }
        public string? BusinessName { get; set; }

        // Branding fields
        public string? PrimaryColor { get; set; } // e.g. "#FF5733"
        public string? SecondaryColor { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
