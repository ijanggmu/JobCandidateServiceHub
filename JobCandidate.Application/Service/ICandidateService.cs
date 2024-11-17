using JobCandidate.Application.DTOs;
using JobCandidate.Shared.Models;

namespace JobCandidate.Application.Service
{
    public interface ICandidateService
    {
        Task<Result<string>> CreateOrUpdateCandidateAsync(CandidateDTO requestModel);
    }
}
