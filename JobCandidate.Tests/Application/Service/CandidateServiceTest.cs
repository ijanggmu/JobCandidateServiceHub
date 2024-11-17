using JobCandidate.Application.DTOs;
using JobCandidate.Application.Service;
using JobCandidate.Domain.Entities;
using JobCandidate.Domain.Interfaces;
using Moq;
using Xunit;

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
            var result = await _candidateService.CreateOrUpdateCandidateAsync(candidateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Errors); // Should not have any errors for a successful creation
            Assert.Equal(200, result.StatusCode); // Success status code
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
            var result = await _candidateService.CreateOrUpdateCandidateAsync(candidateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Errors); // Should not have any errors for a successful update
            Assert.Equal(200, result.StatusCode); // Success status code
            _candidateRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Candidate>(c => c.Email == candidateDto.Email && c.FirstName == candidateDto.FirstName)), Times.Once);
            _cacheRepositoryMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<Candidate>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrUpdateCandidateAsync_ShouldReturnFailure_WhenCandidateDtoIsNull()
        {
            // Arrange
            CandidateDTO? candidateDto = null;

            // Act
            var result = await _candidateService.CreateOrUpdateCandidateAsync(candidateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors); // Should contain errors
            Assert.Equal(400, result.StatusCode); // Bad Request status code
            _candidateRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Candidate>()), Times.Never);
            _candidateRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Candidate>()), Times.Never);
            _cacheRepositoryMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<Candidate>()), Times.Never);
        }
    }
}
