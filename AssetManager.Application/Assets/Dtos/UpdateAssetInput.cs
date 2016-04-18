using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace AssetManager.Assets.Dtos
{
    /// <summary>
    /// This DTO class is used to send needed data to <see cref="IAssetAppService.UpdateAsset"/> method.
    /// 
    /// Implements <see cref="IInputDto"/>, thus ABP applies standard input process (like automatic validation) for it. 
    /// Implements <see cref="ICustomValidate"/> for additional custom validation.
    /// </summary>
    public class UpdateAssetInput : IInputDto, ICustomValidate
    {
        [Range(1, long.MaxValue)] //Data annotation attributes work as expected.
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? AssetTypeId { get; set; }

        //Custom validation method. It's called by ABP after data annotation validations.
        public void AddValidationErrors(List<ValidationResult> results)
        {
            if (AssetTypeId == null)
            {
                results.Add(new ValidationResult("Asset type can not be null in order to update an asset", new[] { "AssetTypeId" }));
            }
        }
    }
}
