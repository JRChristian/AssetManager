using AssetManager.Entities;
using AssetManager.Services.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager
{
    internal static class DtoMappings
    {
        public static void Map()
        {
            Mapper.CreateMap<Asset, AssetDto>();
        }
    }
}
