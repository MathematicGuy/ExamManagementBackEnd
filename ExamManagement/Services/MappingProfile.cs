using AutoMapper;
using ExamManagement.DTOs.AssignmentDTOs;
using ExamManagement.DTOs.QuestionDTOs;
using ExamManagement.Models;

namespace ExamManagement.Services
{
    public class AssignmentProfile : Profile
    {
        public AssignmentProfile()
        {
            CreateMap<CreateAssignment, Assignment>()
                .ForMember(dest => dest.AssignmentId, opt => opt.Ignore()) // Ignore the ID since it's auto-generated
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Open")) // Set a default status of "Open"
                .ForMember(dest => dest.TeacherAssignments, opt => opt.Ignore()) // Ignore the TeacherAssignments collection
                .ForMember(dest => dest.StudentAssignments, opt => opt.Ignore()) // Ignore the StudentAssignments collection
                .ForMember(dest => dest.AssignmentQuestions, opt => opt.Ignore()); // Ignore the AssignmentQuestions collection
        }
    }
}
