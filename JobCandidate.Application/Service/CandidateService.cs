﻿using JobCandidate.Application.DTOs;
using JobCandidate.Domain.Entities;
using JobCandidate.Domain.Interfaces;
using JobCandidate.Shared.Models;

namespace JobCandidate.Application.Service
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly ICacheRepository<Candidate> _cacheRepository;

        public CandidateService(ICandidateRepository candidateRepository, ICacheRepository<Candidate> cacheRepository)
        {
            _candidateRepository = candidateRepository;
            _cacheRepository = cacheRepository;
        }

        public async Task<Result<string>> CreateOrUpdateCandidateAsync(CandidateDTO requestModel)
        {
            if (requestModel == null)
                return Result<string>.Failure(["Request model is null"], 400);

            var cacheKey = $"candidate:{requestModel.Email}";

            Candidate existingCandidate;

            existingCandidate = _cacheRepository.Get(cacheKey);

            if (existingCandidate == null)
            {
                existingCandidate = await _candidateRepository.GetByEmailAsync(requestModel.Email);

                if (existingCandidate != null)
                {
                    _cacheRepository.Set(cacheKey, existingCandidate);
                }
            }

            if (existingCandidate != null)
            {
                existingCandidate.FirstName = requestModel.FirstName;
                existingCandidate.LastName = requestModel.LastName;
                existingCandidate.PhoneNumber = requestModel.PhoneNumber;
                existingCandidate.CallTimeInterval = requestModel.CallTimeInterval;
                existingCandidate.LinkedInProfileUrl = requestModel.LinkedInProfileUrl;
                existingCandidate.GitHubProfileUrl = requestModel.GitHubProfileUrl;
                existingCandidate.Comments = requestModel.Comments;

                await _candidateRepository.UpdateAsync(existingCandidate);

                _cacheRepository.Set(cacheKey, existingCandidate);

                return Result<string>.Success("Candidate updated successfully");

            }
            else
            {
                var candidate = new Candidate
                {
                    FirstName = requestModel.FirstName,
                    LastName = requestModel.LastName,
                    PhoneNumber = requestModel.PhoneNumber,
                    CallTimeInterval = requestModel.CallTimeInterval,
                    LinkedInProfileUrl = requestModel.LinkedInProfileUrl,
                    GitHubProfileUrl = requestModel.GitHubProfileUrl,
                    Comments = requestModel.Comments,
                    Email = requestModel.Email
                };

                await _candidateRepository.AddAsync(candidate);

                _cacheRepository.Set(cacheKey, candidate);
                return Result<string>.Success("Candidate created successfully");
            }
        }
    }
}
