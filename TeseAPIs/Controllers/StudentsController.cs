using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeseAPIs.Models.ProgressResponse;
using TeseAPIs.Services;

namespace TeseAPIs.Controllers
{
    [ApiController]
    public class StudentsController(IStudentService studentRepository, IStudentProgressService studentProgressService) : ControllerBase
    {
        [HttpPost(ApiEndpoints.Tese.Create)]
        public async Task<IActionResult> Create([FromBody]string studentId)
        {
            var created = await studentRepository.CreateAsync(studentId);
            if (string.Equals(created.PlayerId,"ERROR"))
            {
                return Conflict("User already registered.");
            }

            return Created(ApiEndpoints.Tese.Create, created);
        }

        [HttpGet(ApiEndpoints.Tese.Get)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var studentProgress = await studentProgressService.GetByIdAsync(id);
            if(studentProgress == null)
            {
                return NotFound();
            }

            return Ok(studentProgress);
        }

        //[Authorize("Admin")]
        [HttpPut(ApiEndpoints.Tese.UpdateCredits)]
        public async Task<IActionResult> UpdateCredits([FromRoute] string id, [FromBody] int credits)
        {
            var updatedProgress = await studentProgressService.UpdateCreditsByIdAsync(id, credits);
            if(updatedProgress == null)
            {
                return Conflict("User does not exist.");
            }

            return Ok(updatedProgress);
        }

        [HttpGet(ApiEndpoints.Tese.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var studentsProgress = await studentProgressService.GetAllAsync();

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
            var updatedProgress = await studentProgressService.UpdateTutorialByIdAsync(id);

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
            var updatedProgress = await studentProgressService.UpdateDayByIdAsync(id, dayDto);

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
            var updatedProgress = await studentProgressService.UpdateModulesByIdAsync(id, moduleDto);

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
            var updatedProgress = await studentProgressService.UpdateAchievementsByIdAsync(id, achievDto);

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
            var updatedProgress = await studentProgressService.UpdateDayStreakByIdAsync(id, dayStreakDto);

            if (updatedProgress == null)
            {
                return Conflict("User does not exist.");
            }

            return Ok(updatedProgress);
        }
    }
}