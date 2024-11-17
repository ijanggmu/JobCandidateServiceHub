using JobCandidate.Domain.Entities;
using JobCandidate.Domain.Interfaces;
using JobCandidate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobCandidate.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly CandidateDbContext _context;

        public CandidateRepository(CandidateDbContext context)
        {
            _context = context;
        }

        public async Task<Candidate> GetByEmailAsync(string email)
        {
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Email == email);
            return candidate;
        }

        public async Task AddAsync(Candidate candidate)
        {
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Candidate candidate)
        {
            candidate.UpdatedOn = DateTime.UtcNow;
            _context.Candidates.Update(candidate);
            await _context.SaveChangesAsync();
        }
    }

}
