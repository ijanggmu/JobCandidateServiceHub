using System.Linq.Expressions;

namespace JobCandidate.Domain.Interfaces
{
    public interface ICandidateRepository<T> where T : class
    {
          
        Task<T> GetByEmailAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T candidate);
        Task UpdateAsync(T candidate);
        Task SaveChangesAsync();
    }

}
