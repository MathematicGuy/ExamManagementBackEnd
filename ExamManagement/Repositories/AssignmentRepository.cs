using ExamManagement.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ExamManagement.Repositories;
using ExamManagement.Models;
using ExamManagement.DTOs.AssignmentDTOs;
using Microsoft.AspNetCore.Identity;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using System.Security.Claims;

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

    public async Task<Assignment> CreateAssignmentAsync(Assignment assignment, ExamManagement.Models.TeacherAssignment teacherAssignment)
    {
        _context.Assignments.Add(assignment);
        _context.TeacherAssignments.Add(teacherAssignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task<CreateAssignment?> GetAssignmentByIdAsync(int id)
    {
        var assignment = await _context.Assignments
            .Include(a => a.AssignmentQuestions)
                .ThenInclude(aq => aq.Question)
            .Include(a => a.TeacherAssignments)
                .ThenInclude(ta => ta.Teacher)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (assignment == null)
            return null;

        var currentUser = _httpContextAccessor.HttpContext?.User;
        if (currentUser == null || !currentUser.IsInRole("Teacher"))
        {
            return null;
        }

        var teacherIdClaim = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(teacherIdClaim, out int teacherId) && assignment.TeacherAssignments.Any(ta => ta.TeacherId == teacherId))
        {
            var createAssignment = _mapper.Map<CreateAssignment>(assignment);
            createAssignment.TotalPoints = createAssignment.Questions?.Sum(q => q.TotalPoints) ?? 0;
            return createAssignment;
        }

        return null;
    }

    public async Task<bool> IsTeacherAuthorized(int assignmentId, int teacherId)
    {
        return await _context.TeacherAssignments.AnyAsync(ta => ta.AssignmentId == assignmentId && ta.TeacherId == teacherId);
    }
}


