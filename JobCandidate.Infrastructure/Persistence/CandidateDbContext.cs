using JobCandidate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobCandidate.Infrastructure.Persistence
{
    public class CandidateDbContext : DbContext
    {
        public DbSet<Candidate> Candidates { get; set; }

        public CandidateDbContext(DbContextOptions<CandidateDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Candidate>(entity =>
                {
                    entity.HasKey(c => c.Id);
                    entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                    entity.HasIndex(c => c.Email).IsUnique(true);
                });
        }
    }

}
