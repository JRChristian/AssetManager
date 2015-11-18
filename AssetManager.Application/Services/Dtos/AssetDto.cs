using Abp.Application.Services.Dto;
using Abp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Services.Dtos
{
    /// <summary>
    /// A DTO class that can be used in various application service methods when needed to send/receive Asset objects.
    /// </summary>
    public class AssetDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long? AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
