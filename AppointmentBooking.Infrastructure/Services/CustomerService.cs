using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _db;
        public CustomerService(AppDbContext db) => _db = db;

        public async Task<List<CustomerDto>> GetAllAsync() =>
            await _db.Customers
                .Select(c => new CustomerDto { Id = c.Id, FullName = c.FullName })
                .ToListAsync();

        public async Task<CustomerDto?> GetByIdAsync(int id) =>
            await _db.Customers
                .Where(c => c.Id == id)
                .Select(c => new CustomerDto { Id = c.Id, FullName = c.FullName })
                .FirstOrDefaultAsync();
    }
}
