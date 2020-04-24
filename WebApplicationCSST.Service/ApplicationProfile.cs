using AutoMapper;
using WebApplicationCSST.Data;
using WebApplicationCSST.Service.Models;

namespace WebApplicationCSST.Service
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Product, ProductModel>()
                .ReverseMap();

            CreateMap<ProductDetails, ProductDetailsModel>()
                .ReverseMap();
        }
    }
}