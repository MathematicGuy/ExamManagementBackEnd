using AutoMapper;
using ExamManagement.DTOs.AssignmentDTOs;
using ExamManagement.DTOs.QuestionDTOs;
using ExamManagement.Models;

namespace ExamManagement.Services
{
    public class MappingProfile : Profile
    {
        //public MappingProfile()
        //{
        //    CreateMap<Assignment, CreateAssignment>()
        //        .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.AssignmentQuestions.Select(aq => aq.Question)))
        //        .ForMember(dest => dest.TotalPoints, opt => opt.Ignore()); // TotalPoints is calculated separately

        //    CreateMap<Question, ViewQuestion>()
        //        .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id));
        //}
    }

}
