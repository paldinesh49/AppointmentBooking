using AppointmentBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int TenantId { get; set; }            // <-- new
        public Tenant? Tenant { get; set; }


        public int ServiceId { get; set; }
        public Service Service { get; set; } = default!;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? Notes { get; set; }
    }
}
