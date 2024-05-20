using AutoMapper;
using ExamManagement.Data;
using ExamManagement.DTOs.AssignmentDTOs;
using ExamManagement.DTOs.QuestionDTOs;
using ExamManagement.Models;
using ExamManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web.Http.ModelBinding;

namespace ExamManagement.Controllers
{

    public class TeacherAssignmenntController : ControllerBase
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly SgsDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public TeacherAssignmenntController(
            IAssignmentRepository repository,
            SgsDbContext context,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _assignmentRepository = repository;
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // Endpoint to create a new assignment
        //[Authorize(Roles = "Teacher")]
        [HttpPost("CreateAssignment")]

        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignment newAssignment) // Use [FromBody] for model binding
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get authenticated teacher's ID

            var teacherAssignment = new TeacherAssignment
            {
                TeacherId = teacherId,
                // AssignmentId will be assigned by the repository
            };

            var createdAssignment = await _assignmentRepository.CreateAssignmentAsync(newAssignment, teacherAssignment);
            return CreatedAtAction(nameof(GetAssignmentById), new { id = createdAssignment.AssignmentId }, createdAssignment);
            
            //catch (Exception ex)
            //{
            //    // Log the exception (using a logger of your choice)
            //    return StatusCode(500, "An error occurred while creating the assignment.");
            //}
        }


        // Endpoint to get an assignment by its ID
        [HttpGet("GetAssignmentById{id}")]
        //[Authorize(Roles = "Teacher,Student")]
        public async Task<IActionResult> GetAssignmentById(int id)
        {
            var assignment = await _assignmentRepository.GetAssignmentByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            return Ok(assignment);
        }


        [HttpPost("{assignmentId}/Questions/{questionId}")]
        public async Task<IActionResult> AddQuestionToAssignment(int assignmentId, int questionId, [FromQuery] string teacherId)
        {
            await _assignmentRepository.AddQuestionToAssignmentAsync(teacherId, assignmentId, questionId);
            return NoContent();
        }
    }
}