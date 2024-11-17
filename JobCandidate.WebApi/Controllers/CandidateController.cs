using JobCandidate.Application.DTOs;
using JobCandidate.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace JobCandidate.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly CandidateService _candidateService;

        public CandidateController(CandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateCandidate([FromBody] CandidateDTO candidate)
        {
            if (candidate == null) return BadRequest("Invalid candidate data");

            await _candidateService.CreateOrUpdateCandidateAsync(candidate);
            return Ok();
        }
    }

}
