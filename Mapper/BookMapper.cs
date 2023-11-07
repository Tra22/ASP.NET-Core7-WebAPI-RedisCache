using APICacheWithRedis.Dtos;
using APICacheWithRedis.Entities;
using AutoMapper;

namespace APICacheWithRedis.Mapper{
    public class BookMapper : Profile{
        public BookMapper()
        {
            CreateMap <Book, BookDto> ();
        }
    }
}