using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDto>> GetAllAsync();
        Task<AppointmentDto?> GetByIdAsync(int id);
        Task<(bool ok, string? error)> CreateAsync(CreateAppointmentRequest request);
        Task<(bool ok, string? error)> UpdateStatusAsync(int id, string newStatus);
    }
}
