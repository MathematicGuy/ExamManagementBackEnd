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
        [Authorize(Roles = "Teacher")]
        [HttpPost("CreateAssignment")]

        public async Task<IActionResult> CreateAssignment(CreateAssignment newAssignment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assignment = new CreateAssignment
            {
                //AssignmentId = newAssignment.AssignmentId,
                //TeacherId = newAssignment.TeacherId, // take the Login Teacher Id. Front-end job to get teacherId
                Title = newAssignment.Title,
                Description = newAssignment.Description,
                PublishTime = newAssignment.PublishTime, // 05/20/2024 09:16:33
                CloseTime = newAssignment.CloseTime,
                //Status = "Published"
            };

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get User ID

            var teacherAssignment = new TeacherAssignmentCreate
            {
                TeacherId = userId,
            };

            var createdAssignment = await _assignmentRepository.CreateAssignmentAsync(assignment, teacherAssignment);

            return CreatedAtAction(nameof(GetAssignmentById), new { id = createdAssignment.AssignmentId }, createdAssignment);
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