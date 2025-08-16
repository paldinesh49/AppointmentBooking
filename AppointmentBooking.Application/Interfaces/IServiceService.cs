using AppointmentBooking.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Interfaces
{
    public interface IServiceService
    {
        Task<List<ServiceDto>> GetAllAsync();
        Task<ServiceDto?> GetByIdAsync(int id);
    }
}
