using JobCandidate.Application.DTOs;
using JobCandidate.Application.Service;
using JobCandidate.Domain.Entities;
using JobCandidate.Domain.Interfaces;
using Moq;

namespace JobCandidate.Tests.Application.Service
{
    public class CandidateServiceTests
    {
        private readonly Mock<ICandidateRepository> _candidateRepositoryMock;
        private readonly Mock<ICacheRepository<Candidate>> _cacheRepositoryMock;
        private readonly CandidateService _candidateService;

        public CandidateServiceTests()
        {
            _candidateRepositoryMock = new Mock<ICandidateRepository>();
            _cacheRepositoryMock = new Mock<ICacheRepository<Candidate>>();
            _candidateService = new CandidateService(_candidateRepositoryMock.Object, _cacheRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateOrUpdateCandidateAsync_ShouldCreateNewCandidate_WhenCandidateNotInCacheOrRepository()
        {
            // Arrange
            var candidateDto = new CandidateDTO
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "1234567890",
                CallTimeInterval = "Morning",
                LinkedInProfileUrl = "https://linkedin.com/johndoe",
                GitHubProfileUrl = "https://github.com/johndoe",
                Comments = "New Candidate"
            };

            _cacheRepositoryMock.Setup(c => c.Get(It.IsAny<string>())).Returns((Candidate)null);
            _candidateRepositoryMock.Setup(r => r.GetByEmailAsync(candidateDto.Email)).ReturnsAsync((Candidate)null);

            // Act
            await _candidateService.CreateOrUpdateCandidateAsync(candidateDto);

            // Assert
            _candidateRepositoryMock.Verify(r => r.AddAsync(It.Is<Candidate>(c => c.Email == candidateDto.Email)), Times.Once);
            _cacheRepositoryMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<Candidate>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrUpdateCandidateAsync_ShouldUpdateCandidate_WhenCandidateExistsInCache()
        {
            // Arrange
            var candidateDto = new CandidateDTO
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "1234567890",
                CallTimeInterval = "Morning",
                LinkedInProfileUrl = "https://linkedin.com/johndoe",
                GitHubProfileUrl = "https://github.com/johndoe",
                Comments = "Updated Candidate"
            };

            var existingCandidate = new Candidate
            {
                Email = candidateDto.Email,
                FirstName = "Old",
                LastName = "Name"
            };

            _cacheRepositoryMock.Setup(c => c.Get(It.IsAny<string>())).Returns(existingCandidate);

            // Act
            await _candidateService.CreateOrUpdateCandidateAsync(candidateDto);

            // Assert
            _candidateRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Candidate>(c => c.Email == candidateDto.Email && c.FirstName == candidateDto.FirstName)), Times.Once);
            _cacheRepositoryMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<Candidate>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrUpdateCandidateAsync_ShouldNotAddOrUpdateCandidate_WhenCandidateDtoIsNull()
        {
            // Arrange
            CandidateDTO? candidateDto = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _candidateService.CreateOrUpdateCandidateAsync(candidateDto));

            _candidateRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Candidate>()), Times.Never);
            _candidateRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Candidate>()), Times.Never);
            _cacheRepositoryMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<Candidate>()), Times.Never);
        }
    }
}
