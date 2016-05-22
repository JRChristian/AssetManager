﻿using Abp.Application.Services.Dto;
using Abp.Extensions;
using AssetManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class LimitDeviationDto : EntityDto<long>
    {
        public long IOWLevelId { get; set; }
        public string LevelName { get; set; }
        public string LevelDescription { get; set; }
        public int Criticality { get; set; }
        public string ResponseGoal { get; set; }
        public string MetricGoal { get; set; }

        public Direction Direction { get; set; }
        public double LimitValue { get; set; }
        public string Cause { get; set; }
        public string Consequences { get; set; }
        public string Action { get; set; }

        public List<DeviationDto> Deviations { get; set; }

        public int SortOrder
        {
            get
            {
                return Direction != Direction.Low ? Criticality : 100 - Criticality;
            }
        }

    }
}