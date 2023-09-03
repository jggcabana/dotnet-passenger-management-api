using Microsoft.EntityFrameworkCore;
using Qless.Repositories.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Repositories
{
    public abstract class BaseRepository<TEntity>
        where TEntity : Entity
    {
        protected readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<TEntity?> GetAsync(int id)
        {
            return await _context.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var result = _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public virtual void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }
    }
}
