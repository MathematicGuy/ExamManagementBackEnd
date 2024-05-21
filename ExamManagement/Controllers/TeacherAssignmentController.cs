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
using Microsoft.EntityFrameworkCore;
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
        [HttpPost("CreateAssignment")]
        //[Authorize(Roles = "Teacher")] // Ensure only authorized teachers can create assignments
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignment newAssignment, string userId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get teacher's ID
            //var teacherId = userId;
            var teacherExists = await _context.Teacher.AnyAsync(t => t.TeacherId == teacherId);
            if (!teacherExists)
            {
                return BadRequest("Teacher not found.");
            }

            //if (teacherId == null)
            //{
            //    return Unauthorized("Teacher not authenticated.");
            //}
            // 16d32e0b-2735-45e6-b199-0eebb66b864d
            var createdAssignment = await _assignmentRepository.CreateAssignmentAsync(newAssignment, teacherId);
                return CreatedAtAction(nameof(GetAssignmentById), new { id = createdAssignment.AssignmentId }, createdAssignment);
        }


        //{
        //  "email": "teacher@gmail.com",
        //  "password": "TeachMen@123"
        //}
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