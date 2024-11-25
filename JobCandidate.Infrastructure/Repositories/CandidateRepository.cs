using JobCandidate.Domain.Interfaces;
using JobCandidate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JobCandidate.Infrastructure.Repositories
{
    public class CandidateRepository<T> : ICandidateRepository<T> where T : class
    {
        private readonly CandidateDbContext _context;
        private readonly DbSet<T> _dbSet;

        public CandidateRepository(CandidateDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T candidate)
        {
            await _dbSet.AddAsync(candidate);
            await SaveChangesAsync();
        }

        public async Task<T?> GetByEmailAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).FirstOrDefaultAsync();
        }


        public async Task UpdateAsync(T candidate)
        {
            _dbSet.Update(candidate);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        //public async Task<Candidate> GetByEmailAsync(string email)
        //{
        //    var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Email == email);
        //    return candidate;
        //}

        //public async Task AddAsync(Candidate candidate)
        //{
        //    _context.Candidates.Add(candidate);
        //    await _context.SaveChangesAsync();
        //}
        //public async Task UpdateAsync(Candidate candidate)
        //{
        //    candidate.UpdatedOn = DateTime.UtcNow;
        //    _context.Candidates.Update(candidate);
        //    await _context.SaveChangesAsync();
        //}
    }

}
