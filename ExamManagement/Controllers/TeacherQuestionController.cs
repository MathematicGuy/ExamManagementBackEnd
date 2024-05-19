using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ExamManagement.Services;
using Microsoft.Data.SqlClient;
using ExamManagement.Models;
using ExamManagement.Repositories;
using ExamManagement.Data;
using ExamManagement.DTOs.QuestionDTOs;
using ExamManagement.Models.Errors;
using System.Security.Claims;

namespace ExamManagement.Controllers
{

    [Authorize(Roles = "Admin")] // turn this on to test authorization
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherQuestionController : ControllerBase
    {
        private readonly ITeacherQuestionRepository _repository;
        private readonly SgsDbContext _context;

        public TeacherQuestionController(
            ITeacherQuestionRepository repository,
            SgsDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // GET: api/TeacherQuestion
        [HttpGet("GetAllQuestion")]
        public async Task<ActionResult<IEnumerable<Question>>> GetTeacherQuestions()
        {

            return Ok(await _repository.GetAllQuestionsAsync());
        }

        // GET: api/TeacherQuestion/5
        [HttpGet("GetQuestionById{id}")]
        public async Task<ActionResult<Question>> GetQuestionById(int id)
        {
            var teacherQuestion = await _repository.GetQuestionByIdAsync(id);
            if (teacherQuestion == null)
            {
                return NotFound();
            }
            return teacherQuestion;
        }

        // POST: api/TeacherQuestion
        [HttpPost("CreateQuestion")]
        public async Task<ActionResult<Question>> CreateQuestionAsync(CreateQuestion newQuestion)
        {
            // Validate the incoming NewQuestion data.
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
                // Do not set the question ID manually. Let the database generate it.
                var question = new Question
                {
                    Name = newQuestion.Name,
                    Description = newQuestion.Description,
                    TotalPoints = newQuestion.TotalPoints ?? 0, // Provide a default value if null
                    AnswerFileURL = newQuestion.AnswerFileURL,
                    Status = "unsubmit" // Set a default status if null
                };

                var createdQuestion = await _repository.CreateQuestionAsync(question);
                return CreatedAtAction(nameof(GetQuestionById), new { id = createdQuestion.Id }, createdQuestion);
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

        // PUT: api/TeacherQuestion/5
        [HttpPut("UpdateQuestion{id}")]
        public async Task<IActionResult> UpdateTeacherQuestion(int id, UpdateQuestion updatedQuestion)
        {
            // Validate the ID
            if (id <= 0)
            {
                return BadRequest(new ErrorResponse { Message = "Invalid question ID." });
            }

            // Basic model validation (add annotations to QuestionUpdateViewModel if needed)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var existingQuestion = await _repository.GetQuestionByIdAsync(id);
                if (existingQuestion == null)
                {
                    return NotFound(new ErrorResponse { Message = "Question not found." });
                }

                // Update the properties
                var concurrencyHandler = new ConcurrencyExceptionHandlingService(_context);
                return await concurrencyHandler.HandleConcurrencyExceptionAsync(existingQuestion, async () =>
                {
                    // Update properties of existingQuestion from updatedQuestion
                    existingQuestion.Name = updatedQuestion.Name;
                    existingQuestion.Description = updatedQuestion.Description;
                    existingQuestion.TotalPoints = updatedQuestion.TotalPoints ?? existingQuestion.TotalPoints;
                    existingQuestion.AnswerFileURL = updatedQuestion.AnswerFileURL;
                    existingQuestion.Status = updatedQuestion.Status;


                    await _repository.UpdateQuestionAsync(existingQuestion);
                }, ModelState); // Pass ModelState to the service

            }
            // catch error if 2 users trying to modifying 1 data
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues if needed 
                throw; // Or choose a different error handling strategy
            }

            //    return NoContent();
        }

        // DELETE: api/TeacherQuestion/5
        [HttpDelete("DeleteQuestion{id}")]
        public async Task<IActionResult> DeleteTeacherQuestion(int id)
        {
            var questionToDelete = await _repository.GetQuestionByIdAsync(id);
            if (questionToDelete == null)
            {
                return NotFound();
            }

            var concurrencyHandler = new ConcurrencyExceptionHandlingService(_context);
            return await concurrencyHandler.HandleConcurrencyExceptionAsync(questionToDelete, async () =>
            {
                await _repository.DeleteQuestionAsync(id);
            }, ModelState);
        }
    }
}
