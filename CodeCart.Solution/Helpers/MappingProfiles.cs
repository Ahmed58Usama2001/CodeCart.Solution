﻿using AutoMapper;
using CodeCart.API.DTOs;
using CodeCart.API.DTOs.AccountDtos;
using CodeCart.Core.Entities;

namespace CodeCart.API.Helpers;

public class MappingProfiles:Profile
{
    public MappingProfiles()
    {
        CreateMap<Product, ProductToReturnDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        CreateMap<AddressDto,Address>().ReverseMap();

    }
}
