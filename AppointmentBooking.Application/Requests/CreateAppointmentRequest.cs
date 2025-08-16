using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Requests
{
    public class CreateAppointmentRequest
    {
        public int ServiceId { get; set; }
        public int CustomerId { get; set; }
        public DateTime StartTime { get; set; } // local time chosen by user
        public string? Notes { get; set; }
    }
}
