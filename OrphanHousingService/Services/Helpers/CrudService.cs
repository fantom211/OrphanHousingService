using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Helpers
{
    public class CrudService<T> where T : class
    {
        protected readonly OrphanHousingDbContext _context;
        private readonly IValidator<T>? _validator;
        public CrudService(OrphanHousingDbContext context, IValidator<T>? validator = null)
        {
            _context = context;
            _validator = validator;
        }

        protected async Task ValidateAsync(T entity)
        {
            if (_validator == null) return;

            var result = await _validator.ValidateAsync(entity);

            if(!result.IsValid)
            {
                var errors = string.Join("\n", result.Errors.Select(e => e.ErrorMessage));
                throw new Exception(errors);
            }
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            if (entity is IHasCreatedAt withCreatedAt && withCreatedAt.CreatedAt == default)
                withCreatedAt.CreatedAt = DateTime.UtcNow;

            await ValidateAsync(entity);

            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            await ValidateAsync(entity);
            await SaveChangesAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }

        public string GenerateNumber()
        {
            return Random.Shared.Next(100, 1000).ToString();
        }
    }
}
