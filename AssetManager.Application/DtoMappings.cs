﻿using AssetManager.Entities;
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
using AssetManager.DomainServices;
using AssetManager.AssetHealth.Dtos;

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
            Mapper.CreateMap<AssetVariableCombinations, AssetVariableDto>();
            Mapper.CreateMap<AssetDeviationSummary, AssetLevelTimeDto>();
            Mapper.CreateMap<DeviationDetails, DeviationDetailDto>();
            Mapper.CreateMap<AssetLimitStatsByDay, AssetLimitStatsByDayDto>();
            Mapper.CreateMap<LimitStatsByDay, LimitStatsByDayDto>();
            Mapper.CreateMap<LimitStatDays, LimitStatDaysDto>();
            Mapper.CreateMap<HealthMetric, HealthMetricDto>();
        }
    }
}
