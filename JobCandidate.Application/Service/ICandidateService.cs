using JobCandidate.Application.DTOs;

namespace JobCandidate.Application.Service
{
    public interface ICandidateService
    {
        Task CreateOrUpdateCandidateAsync(CandidateDTO requestModel);
    }
}
