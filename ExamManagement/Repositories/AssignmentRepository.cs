using ExamManagement.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ExamManagement.Repositories;
using ExamManagement.Models;
using ExamManagement.DTOs.AssignmentDTOs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ExamManagement.DTOs.QuestionDTOs;
using Microsoft.AspNetCore.Authorization;

public class AssignmentRepository : IAssignmentRepository
{

    private readonly SgsDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssignmentRepository(SgsDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) // Add IHttpContextAccessor
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task AddQuestionToAssignmentAsync(int assignmentId, Question question)
    {
        var assignment = await _context.Assignments.FindAsync(assignmentId);
        if (assignment == null)
            throw new ArgumentException("Assignment not found.");

        var assignmentQuestion = new AssignmentQuestion
        {
            AssignmentId = assignmentId,
            QuestionId = question.Id
        };

        _context.AssignmentQuestions.Add(assignmentQuestion);
        await _context.SaveChangesAsync();
    }

    [Authorize] // Requires user to be logged in. reutrn 401 if not logged in
    public async Task<Assignment> CreateAssignmentAsync(CreateAssignment newAssignment, string teacherId)
    {
        // Create a new Assignment object from the CreateAssignment
        //var assignment = new Assignment
        //{
        //    Title = newAssignment.Title,
        //    Description = newAssignment.Description,
        //    PublishTime = newAssignment.PublishTime,
        //    CloseTime = newAssignment.CloseTime,
        //    //Status = newAssignment.Status,
        //    // You may need to map other properties as well
        //};

        var assignment = _mapper.Map<Assignment>(newAssignment);  // Map DTO to Assignment entity 

        // Add the new Assignment to the context and get its Id
        var addedAssignment = await _context.Assignments.AddAsync(assignment);

        _context.Assignments.Add(assignment); // No need for async here
        await _context.SaveChangesAsync(); // Save to get generated Id

        // Set the TeacherAssignment's AssignmentId to the saved assignment's Id
        var teacherAssignment = new TeacherAssignment
        {
            TeacherId = teacherId,
            AssignmentId = assignment.AssignmentId
        };

        // Add the TeacherAssignment to the context
        _context.TeacherAssignments.Add(teacherAssignment);

        // Save changes to the database
        await _context.SaveChangesAsync();

        // Return the newly created Assignment
        return assignment;
    }


    public async Task AddQuestionToAssignmentAsync(string teacherId, int assignmentId, int questionId)
    {
        var assignmentQuestion = new AssignmentQuestion 
        { 
            AssignmentId = assignmentId,
            QuestionId = questionId
        }; 

        await _context.AssignmentQuestions.AddAsync(assignmentQuestion);
        await _context.SaveChangesAsync();
    }

    public async Task<CreateAssignment?> GetAssignmentByIdAsync(int id)
    {
        var assignment = await _context.Assignments
            .Include(a => a.TeacherAssignments)
                .ThenInclude(ta => ta.Teacher)
            .Include(a => a.AssignmentQuestions)
                .ThenInclude(aq => aq.Question)
            .FirstOrDefaultAsync(a => a.AssignmentId == id);

        if (assignment == null)
        {
            return null;
        }

        var teacherAssignment = assignment.TeacherAssignments.FirstOrDefault();
        var teacherDTO = new Teacher
        {
            TeacherId = teacherAssignment?.TeacherId ?? string.Empty, // Changed to string
            Name = teacherAssignment?.Teacher.Name ?? string.Empty
        };

        var questions = assignment.AssignmentQuestions.Select(aq => new ViewQuestion
        {
            QuestionId = aq.QuestionId,
            Name = aq.Question.Name,
            Description = aq.Question.Description,
            TotalPoints = aq.Question.TotalPoints ?? 0
        }).ToList();

        var newAssignment = new CreateAssignment
        {
            //AssignmentId = assignment.Id,
            Title = assignment.Title,
            Description = assignment.Description,
            PublishTime = assignment.PublishTime,
            CloseTime = assignment.CloseTime,
            //Status = assignment.Status,
            //TeacherId = teacherDTO.TeacherId // Assign TeacherId from teacherDTO
        };

        return newAssignment;
    }
}


    