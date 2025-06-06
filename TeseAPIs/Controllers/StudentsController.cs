using Microsoft.AspNetCore.Mvc;
using Polly.Registry;
using TeseAPIs.Models;
using TeseAPIs.Models.ProgressResponse;
using TeseAPIs.Services;

namespace TeseAPIs.Controllers
{
    [ApiController]
    public class StudentsController(IStudentService studentRepository, IStudentProgressService studentProgressService, ResiliencePipelineProvider<string> _pipelineProvider, IRewardService rewardService, IChallengeService challengeService) : ControllerBase
    {

        [HttpPost(ApiEndpoints.Tese.Create)]
        public async Task<IActionResult> Create([FromBody]RegistrationData studentData)
        {         
            var pipeline = _pipelineProvider.GetPipeline("default");

            var created = await pipeline.ExecuteAsync(async ct => await studentRepository.CreateAsync(studentData));
            if (string.Equals(created.PlayerId,"ERROR"))
            {
                return Conflict("User already registered.");
            }

            return Created(ApiEndpoints.Tese.Create, created);
        }

        [HttpGet(ApiEndpoints.Tese.Get)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var studentProgress = await pipeline.ExecuteAsync( async ct => await studentProgressService.GetByIdAsync(id));

            if (studentProgress == null)
            {
                return NotFound();
            }

            return Ok(studentProgress);
        }

        //[Authorize("Admin")]
        [HttpPut(ApiEndpoints.Tese.UpdateCredits)]
        public async Task<IActionResult> UpdateCredits([FromRoute] string id, [FromBody] int credits)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var updatedProgress = await pipeline.ExecuteAsync(async ct => await studentProgressService.UpdateCreditsByIdAsync(id, credits));
            
            
            if(updatedProgress == null)
            {
                return Conflict("User does not exist.");
            }

            return Ok(updatedProgress);
        }

        [HttpGet(ApiEndpoints.Tese.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var studentsProgress = await pipeline.ExecuteAsync(async ct => await studentProgressService.GetAllAsync());

            if (studentsProgress == null)
            {
                return NotFound();
            }

            var students = studentsProgress;
            return Ok(students);
        }

        //[Authorize("Unity")]
        [HttpPut(ApiEndpoints.Tese.UpdateTutorial)]
        public async Task<IActionResult> UpdateTutorial([FromBody] string id)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var updatedProgress = await pipeline.ExecuteAsync(async ct => await studentProgressService.UpdateTutorialByIdAsync(id));

            if (updatedProgress == null)
            {
                return Conflict("User does not exist.");
            }

            return Ok(updatedProgress);
        }

        //[Authorize("Unity")]
        [HttpPut(ApiEndpoints.Tese.UpdateDay)]
        public async Task<IActionResult> UpdateDayProgress([FromRoute] string id,[FromBody] DayResponse dayDto)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var updatedProgress = await pipeline.ExecuteAsync(async ct => await studentProgressService.UpdateDayByIdAsync(id, dayDto));

            if (updatedProgress == null)
            {
                return Conflict("User does not exist.");
            }

            return Ok(updatedProgress);
        }

        //[Authorize("Unity")]
        [HttpPut(ApiEndpoints.Tese.UpdateModules)]
        public async Task<IActionResult> UpdateModulesProgress([FromRoute] string id, [FromBody] ModuleResponse moduleDto)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var updatedProgress = await pipeline.ExecuteAsync(async ct => await studentProgressService.UpdateModulesByIdAsync(id, moduleDto));

            if (updatedProgress == null)
            {
                return Conflict("User does not exist.");
            }

            return Ok(updatedProgress);
        }

        //[Authorize("Unity")]
        [HttpPut(ApiEndpoints.Tese.UpdateAchievements)]
        public async Task<IActionResult> UpdateAchievementsProgress([FromRoute] string id, [FromBody] AchievementResponse achievDto)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var updatedProgress = await pipeline.ExecuteAsync(async ct => await studentProgressService.UpdateAchievementsByIdAsync(id, achievDto));

            if (updatedProgress == null)
            {
                return Conflict("User does not exist.");
            }

            return Ok(updatedProgress);
        }

        //[Authorize("Unity")]
        [HttpPut(ApiEndpoints.Tese.UpdateDayStreak)]
        public async Task<IActionResult> UpdateDayStreakProgress([FromRoute] string id, [FromBody] DayStreakResponse dayStreakDto)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var updatedProgress = await pipeline.ExecuteAsync(async ct => await studentProgressService.UpdateDayStreakByIdAsync(id, dayStreakDto));

            if (updatedProgress == null)
            {
                return Conflict("User does not exist.");
            }

            return Ok(updatedProgress);
        }

        [HttpGet(ApiEndpoints.Tese.Rewards)]
        public async Task<IActionResult> GetAllRewards()
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var  rewards = await pipeline.ExecuteAsync(async ct => await rewardService.GetRewardsAsync());

            if (rewards == null)
            {
                return NotFound();
            }

            return Ok(rewards);
        }

        [HttpPut(ApiEndpoints.Tese.UpdateReward)]
        public async Task<IActionResult> UpdateReward([FromRoute] int id, [FromBody] UpdateRewardDto dto)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var result = await pipeline.ExecuteAsync(async ct => await rewardService.UpdateRewardById(id, dto.Name, dto.Price));

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet(ApiEndpoints.Tese.Challenges)]
        public async Task<IActionResult> GetChallenges()
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var challenge = await pipeline.ExecuteAsync(async ct => await challengeService.GetChallengesAsync());

            if (challenge == null)
            {
                return NotFound();
            }

            return Ok(challenge);
        }

        [HttpGet(ApiEndpoints.Tese.Challenge)]
        public async Task<IActionResult> GetLatestChallenge()
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var challenge = await pipeline.ExecuteAsync(async ct => await challengeService.GetChallengeAsync());

            if (challenge == null)
            {
                return NotFound();
            }

            return Ok(challenge);
        }

        [HttpPost(ApiEndpoints.Tese.Challenges)]
        public async Task<IActionResult> CreateChallenge(ChallengeDTO challengeDTO)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var challenge = await pipeline.ExecuteAsync(async ct => await challengeService.CreateChallenge(challengeDTO));

            if (challenge == null)
            {
                return BadRequest();
            }

            return Ok(challenge);
        }

        [HttpPut(ApiEndpoints.Tese.UpdateChallenge)]
        public async Task<IActionResult> UpdateChallenge([FromBody] Challenge dto)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var result = await pipeline.ExecuteAsync(async ct => await challengeService.UpdateChallengeById(dto));

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut(ApiEndpoints.Tese.EndChallenge)]
        public async Task<IActionResult> EndChallenge([FromBody] Challenge dto)
        {
            var pipeline = _pipelineProvider.GetPipeline("default");
            var result = await pipeline.ExecuteAsync(async ct => await challengeService.EndChallengeById(dto));

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}