using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Infrastructure.Data;
using AppointmentBooking.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;


namespace AppointmentBooking.Infrastructure.Services
{
    public class ServiceService : IServiceService
    {
        private readonly AppDbContext _db;
        private readonly ICurrentTenant _currentTenant;
        public ServiceService(AppDbContext db, ICurrentTenant currentTenant)
        {
            _db = db;
            _currentTenant = currentTenant;
        }
        public async Task<ServiceDto> CreateAsync(CreateServiceRequest req)
        {
            if (!_currentTenant.TenantId.HasValue)
                throw new InvalidOperationException("No tenant selected.");

            var entity = new Service
            {
                TenantId = _currentTenant.TenantId.Value,
                Name = req.Name,
                Price = req.Price,
                Description = req.Description
            };

            _db.Services.Add(entity);
            await _db.SaveChangesAsync();
            // map to dto...
            // Map to DTO and return
            return new ServiceDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                DurationMinutes = entity.DurationMinutes
            };
        }

        public async Task<List<ServiceDto>> GetAllAsync() =>
            await _db.Services
                .Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Price = s.Price, DurationMinutes = s.DurationMinutes })
                .ToListAsync();

        public async Task<ServiceDto?> GetByIdAsync(int id) =>
            await _db.Services
                .Where(s => s.Id == id)
                .Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Price = s.Price, DurationMinutes = s.DurationMinutes })
                .FirstOrDefaultAsync();
    }
}
