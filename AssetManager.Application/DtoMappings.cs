using AssetManager.Entities;
using AssetManager.Assets.Dtos;
using AssetManager.IOWs.Dtos;
using AssetManager.Tags.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetManager.EntityFramework.DomainServices;
using AssetManager.Images.Dtos;

namespace AssetManager
{
    internal static class DtoMappings
    {
        public static void Map()
        {
            Mapper.CreateMap<Asset, AssetDto>();
            Mapper.CreateMap<AssetHierarchy, AssetHierarchyDto>();
            Mapper.CreateMap<Image, ImageDto>();
            Mapper.CreateMap<IOWLevel, LevelDto>();
            Mapper.CreateMap<IOWVariable, VariableDto>();
            Mapper.CreateMap<IOWLimit, LimitDto>();
            Mapper.CreateMap<IOWVariable, VariableLimitDto>();
            Mapper.CreateMap<Tag, TagDto>();
            Mapper.CreateMap<TagDataRaw, TagDataRawDto>();
            Mapper.CreateMap<DetectDeviationsOut, DetectDeviationsOutput>();
        }
    }
}
