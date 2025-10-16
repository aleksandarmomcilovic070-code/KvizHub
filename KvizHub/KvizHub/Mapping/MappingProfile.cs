using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KvizHub.Dto;
using KvizHub.DTOs;
using KvizHub.Models;

namespace KvizHub.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap(); //Kazemo mu da mapira Subject na SubjectDto i obrnuto
            CreateMap<Quiz, QuizDto>().ReverseMap();
            CreateMap<AnswerQuiz, AnswerQuizDto>().ReverseMap();
        }
    }
}
