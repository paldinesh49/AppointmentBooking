using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Application.Requests;
using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Domain.Enums;
using AppointmentBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _db;
        public AppointmentService(AppDbContext db) => _db = db;

        public async Task<List<AppointmentDto>> GetAllAsync()
        {
            return await _db.Appointments
                .Include(a => a.Service)
                .Include(a => a.Customer)
                .OrderBy(a => a.StartTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    ServiceName = a.Service.Name,
                    CustomerName = a.Customer.FullName,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status.ToString()
                }).ToListAsync();
        }

        public async Task<AppointmentDto?> GetByIdAsync(int id)
        {
            return await _db.Appointments
                .Include(a => a.Service)
                .Include(a => a.Customer)
                .Where(a => a.Id == id)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    ServiceName = a.Service.Name,
                    CustomerName = a.Customer.FullName,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status.ToString()
                }).FirstOrDefaultAsync();
        }

        public async Task<(bool ok, string? error)> CreateAsync(CreateAppointmentRequest req)
        {
            var service = await _db.Services.FirstOrDefaultAsync(s => s.Id == req.ServiceId);
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == req.CustomerId);
            if (service == null) return (false, "Service not found.");
            if (customer == null) return (false, "Customer not found.");

            var start = DateTime.SpecifyKind(req.StartTime, DateTimeKind.Local);
            var end = start.AddMinutes(service.DurationMinutes);

            // Overlap rule: requestedStart < existingEnd AND requestedEnd > existingStart (same service)
            var overlap = await _db.Appointments.AnyAsync(a =>
                a.ServiceId == req.ServiceId &&
                a.Status != AppointmentStatus.Cancelled &&
                start < a.EndTime && end > a.StartTime);

            if (overlap) return (false, "Time slot is not available.");

            var entity = new Appointment
            {
                ServiceId = service.Id,
                CustomerId = customer.Id,
                StartTime = start,
                EndTime = end,
                Status = AppointmentStatus.Pending,
                Notes = req.Notes
            };

            _db.Appointments.Add(entity);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool ok, string? error)> UpdateStatusAsync(int id, string newStatus)
        {
            var appt = await _db.Appointments.FindAsync(id);
            if (appt == null) return (false, "Appointment not found.");

            if (!Enum.TryParse<AppointmentStatus>(newStatus, ignoreCase: true, out var status))
                return (false, "Invalid status.");

            appt.Status = status;
            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}
