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

namespace AssetManager
{
    internal static class DtoMappings
    {
        public static void Map()
        {
            Mapper.CreateMap<Asset, AssetDto>();
            Mapper.CreateMap<IOWLevel, IOWLevelDto>();
            Mapper.CreateMap<Tag, TagDto>();
            Mapper.CreateMap<TagDataRaw, TagDataRawDto>();
        }
    }
}
