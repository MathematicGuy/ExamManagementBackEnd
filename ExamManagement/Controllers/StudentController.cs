using ExamManagement.Data;
using ExamManagement.Repositories;
using ExamManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ExamManagement.Services;
using ExamManagement.DTOs.StudentDTOs;
using ExamManagement.Models.Errors;
using Microsoft.AspNetCore.Authorization;

namespace ExamManagement.Controllers
{
    //[Authorize(Roles = "User")] 
    [Route("api/[controller]")]
    [ApiController]

    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly SgsDbContext _context;

        public StudentController(IStudentRepository studentRepository, SgsDbContext context)
        {
            _studentRepository = studentRepository;
            _context = context;
        }


        // GET: api/Students
        [HttpGet("GetAllStudent")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            var students = await _studentRepository.GetAllStudentsAsync();
            return Ok(students);
        }

        // GET: api/Students/5
        [HttpGet("GetStudentById{id}")]
        public async Task<ActionResult<Student>> GetStudentById(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return student;
        }

        // POST: api/Students
        [HttpPost("CreateStudent")]
        public async Task<ActionResult<Student>> CreateStudent([FromBody] CreateStudent newStudent)
        {
            // Validate the incoming question data.
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Invalid input",
                    Details = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }


            try
            {
                // Check if the StudentId already exists
                var existingStudent = await _studentRepository.GetStudentByIdAsync(newStudent.StudentId);
                if (existingStudent != null)
                {
                    return Conflict(new ErrorResponse { Message = "Student with this ID already exists." });
                }

                // Mapping ViewModel to Student 
                var student = new Student
                {
                    StudentId = newStudent.StudentId,
                    Name = newStudent.Name,
                    Role = newStudent.Role,
                    Status = "active",
                };

                var createdStudent = await _studentRepository.CreateStudentAsync(student);
                return CreatedAtAction(nameof(GetStudentById), new { id = createdStudent.StudentId }, createdStudent);
            }
            catch (DbUpdateException ex)
            {
                // Check for specific exceptions (e.g., unique constraint violation)
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627) // Unique constraint
                {
                    return Conflict(new ErrorResponse { Message = "A question with this name already exists." });
                }

                // For other exceptions, return a generic error
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while creating the question."
                });
            }
        }

        // PUT: api/Students/5
        // Update Status instead of Delete:
        // Create a Table for Deactivate Student. Transfer Student data to DeletedStudent Table 

        [HttpPut("UpdateStudent{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudent updateStudent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Fetch the existing student
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // Update properties
            student.Name = updateStudent.Name;
            student.Role = updateStudent.Role;
            student.Status = updateStudent.Status;

            // Concurrency handling service
            var concurrencyHandler = new ConcurrencyExceptionHandlingService(_context); // Assuming _context is your DbContext
            return await concurrencyHandler.HandleConcurrencyExceptionAsync(student, async () =>
            {
                await _studentRepository.UpdateStudentAsync(student);
            }, ModelState);
        }
    }

}
