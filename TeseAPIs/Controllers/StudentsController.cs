using Microsoft.AspNetCore.Mvc;
using TeseAPIs.Mapping;
using TeseAPIs.Services;

namespace TeseAPIs.Controllers
{
    [ApiController]
    public class StudentsController(IStudentService studentRepository, IStudentProgressService studentProgressService) : ControllerBase
    {
        private readonly IStudentService _studentService = studentRepository; 
        private readonly IStudentProgressService _studentProgressService = studentProgressService;

        [HttpPost(ApiEndpoints.Tese.Create)]
        public async Task<IActionResult> Create([FromBody]string studentId)
        {
            var created = await _studentService.CreateAsync(studentId);
            if (!created)
            {
                return BadRequest();
            }

            return Created();
        }

        [HttpGet(ApiEndpoints.Tese.Get)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var studentProgress = await _studentProgressService.GetByIdAsync(id);
            if(studentProgress == null)
            {
                return NotFound();
            }

            return Ok(studentProgress);
        }

        [HttpPut(ApiEndpoints.Tese.Update)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] int credits)
        {
            var updatedProgress = await _studentProgressService.UpdateCreditsByIdAsync(id, credits);
            if(updatedProgress == null || updatedProgress == false)
            {
                return NotFound();
            }

            return Ok(updatedProgress);
        }

        [HttpGet(ApiEndpoints.Tese.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var studentsProgress = await _studentProgressService.GetAllAsync();

            if (studentsProgress == null)
            {
                return NotFound();
            }

            var students = studentsProgress;
            return Ok(students);
        }
    }
}