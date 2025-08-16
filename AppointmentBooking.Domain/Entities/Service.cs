using AppointmentBooking.Domain.Enums;
namespace AppointmentBooking.Domain.Entities
{
    public class Service
    {
        
       
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; } // default length of an appointment
        public int TenantId { get; set; }            // <-- new
        public Tenant? Tenant { get; set; }          // optional navigation

        public string? Description { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
