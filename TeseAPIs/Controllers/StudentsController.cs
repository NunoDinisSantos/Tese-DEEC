using Microsoft.AspNetCore.Mvc;
using System.Net;
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

        [HttpPut(ApiEndpoints.Tese.Update)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] int credits)
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
    }
}