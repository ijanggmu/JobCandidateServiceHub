using JobCandidate.Application.DTOs;
using JobCandidate.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace JobCandidate.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateCandidate([FromBody] CandidateDTO candidate)
        {
            var result = await _candidateService.CreateOrUpdateCandidateAsync(candidate);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }

}
