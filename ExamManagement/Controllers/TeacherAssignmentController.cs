using AutoMapper;
using ExamManagement.Data;
using ExamManagement.DTOs.AssignmentDTOs;
using ExamManagement.Models;
using ExamManagement.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web.Http.ModelBinding;

namespace ExamManagement.Controllers
{

    public class TeacherAssignmentnController : ControllerBase
    {
        private readonly IAssignmentRepository assignmentRepository;
        private readonly SgsDbContext context;
        private readonly Mapper mapper;

        public TeacherAssignmentnController(
            IAssignmentRepository repository,
            SgsDbContext context,
            Mapper mapper)
        {
            this.assignmentRepository = repository;
            this.context = context;
            this.mapper = mapper;
        }



    }
}