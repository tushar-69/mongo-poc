using AutoMapper;
using MongoPOC.Models;
using MongoPOC.Models.DTOs;

namespace MongoPOC.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>();
            CreateMap<Review, ReviewDTO>();
        }
    }
}
