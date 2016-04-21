using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AssetManager.Entities;
using AssetManager.IOWs.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs
{
    public class IowVariableAppService : AssetManagerAppServiceBase, IIowVariableAppService
    {
        private readonly IIOWVariableRepository _iowVariableRepository;
        private readonly ITagRepository _tagRepository;

        public IowVariableAppService(IIOWVariableRepository iowVariableRepository, ITagRepository tagRepository)
        {
            _iowVariableRepository = iowVariableRepository;
            _tagRepository = tagRepository;
        }


        public IowVariableDto GetOneIowVariable(GetOneIowVariableInput input)
        {
            IOWVariable variable = null;

            // Look for the variable
            if (input.Id.HasValue)
                variable = _iowVariableRepository.Get(input.Id.Value);
            else if (!string.IsNullOrEmpty(input.Name))
                variable = _iowVariableRepository.FirstOrDefault(p => p.Name == input.Name);

            return variable.MapTo<IowVariableDto>();
        }

        public GetIowVariableOutput GetIowVariables(GetIowVariableInput input)
        {
            List<IOWVariable> variables = null;

            // If the input includes a variable name or a tag name, treat those as wild card matches.
            if( !string.IsNullOrEmpty(input.Name) && !string.IsNullOrEmpty(input.TagName) )
            {
                variables = _iowVariableRepository.GetAll()
                    .Where(v => v.Name.Contains(input.Name) && v.Tag.Name.Contains(input.TagName))
                    .OrderBy(v => v.Name)
                    .ToList();
            }
            else if( !string.IsNullOrEmpty(input.Name) )
            {
                variables = _iowVariableRepository.GetAll()
                    .Where(v => v.Name.Contains(input.Name))
                    .OrderBy(v => v.Name)
                    .ToList();
            }
            else if( !string.IsNullOrEmpty(input.TagName) )
            {
                variables = _iowVariableRepository.GetAll()
                    .Where(v => v.Tag.Name.Contains(input.TagName))
                    .OrderBy(v => v.Name)
                    .ToList();
            }
            else
            {
                variables = _iowVariableRepository.GetAll()
                    .OrderBy(v => v.Name)
                    .ToList();
            }

            return new GetIowVariableOutput
            {
                IowVariables = variables.MapTo<List<IowVariableDto>>()
            };
        }

        public async Task<GetIowVariableOutput> GetIowVariablesAsync(GetIowVariableInput input)
        {
            List<IOWVariable> variables = null;

            // If the input includes a variable name or a tag name, treat those as wild card matches.
            if (!string.IsNullOrEmpty(input.Name) && !string.IsNullOrEmpty(input.TagName))
            {
                variables = await _iowVariableRepository.GetAllListAsync(v => v.Name.Contains(input.Name) && v.Tag.Name.Contains(input.TagName));
            }
            else if( !string.IsNullOrEmpty(input.Name) )
            {
                variables = await _iowVariableRepository.GetAllListAsync(v => v.Name.Contains(input.Name));
            }
            else if( !string.IsNullOrEmpty(input.TagName) )
            {
                variables = await _iowVariableRepository.GetAllListAsync(v => v.Tag.Name.Contains(input.TagName));
            }
            else
            {
                variables = await _iowVariableRepository.GetAllListAsync();
            }

            var sorted = variables.OrderBy(p => p.Name);

            return new GetIowVariableOutput
            {
                IowVariables = sorted.MapTo<List<IowVariableDto>>()
            };
        }


        public IowVariableDto CreateIowVariable(CreateIowVariableInput input)
        {
            Logger.Info("Creating a variable for input: " + input.Name);

            Tag tag = null;
            IOWVariable variable = null;

            // Check to see if a variable by this name already exists
            variable = _iowVariableRepository.FirstOrDefault(p => p.Name == input.Name);
            if( variable == null )
            {
                // All variables belong to a tenant. If not specified, put them in the default tenant.
                int tenantid = (AbpSession.TenantId != null) ? (int)AbpSession.TenantId : 1;

                // Create a new variable object from the input
                variable = new IOWVariable
                {
                    Name = input.Name,
                    Description = input.Description,
                    UOM = input.UOM,
                    TenantId = tenantid
                };

                // Having a tag is optional. The caller may have specified a tag id or name, or neither.
                // If a tag is specified but not found, ignore it.
                if (input.TagId.HasValue)
                    tag = _tagRepository.FirstOrDefault(p => p.Id == input.TagId);
                else if ( !string.IsNullOrEmpty(input.TagName) )
                    tag = _tagRepository.FirstOrDefault(p => p.Name == input.TagName);

                if (tag != null)
                {
                    variable.TagId = tag.Id;
                    // If UOM was not specified for the variable AND the tag has a UOM, then use the tag's UOM
                    if (string.IsNullOrEmpty(variable.UOM) && !string.IsNullOrEmpty(tag.UOM))
                        variable.UOM = tag.UOM;
                }

                // Add the new variable to the repository
                variable.Id = _iowVariableRepository.InsertAndGetId(variable);
            }
            return variable.MapTo<IowVariableDto>();
        }

        public IowVariableDto DeleteIowVariable(GetOneIowVariableInput input)
        {
            IOWVariable variable = null;

            Logger.Info("Delete a variable for input Id= " + (input.Id.HasValue ? (input.Id.Value).ToString() : "n/a") + " Name= " + input.Name);

            // Look for the variable
            if (input.Id.HasValue)
                variable = _iowVariableRepository.Get(input.Id.Value);
            else if ( !string.IsNullOrEmpty(input.Name) )
                variable = _iowVariableRepository.FirstOrDefault(p => p.Name == input.Name);

            if (variable != null)
                _iowVariableRepository.Delete(variable);

            return variable.MapTo<IowVariableDto>();
        }

        public IowVariableDto UpdateIowVariable(UpdateIowVariableInput input)
        {
            IOWVariable variable = null;
            Tag tag = null;

            Logger.Info("Update a variable for input Id= " + (input.Id.HasValue ? (input.Id.Value).ToString() : "n/a") + " Name= " + input.Name);

            // Look for the variable
            if (input.Id.HasValue)
                variable = _iowVariableRepository.Get(input.Id.Value);
            else if (input.Name != "")
                variable = _iowVariableRepository.FirstOrDefault(p => p.Name == input.Name);

            // If we find a variable, update its properties
            if (variable != null)
            {
                if (!string.IsNullOrEmpty(input.Name))
                    variable.Name = input.Name;

                if (!string.IsNullOrEmpty(input.Description))
                    variable.Description = input.Description;
                
                // If tag information (id or name) was specified, then look for this tag
                if (input.TagId.HasValue && input.TagId.Value != variable.TagId)
                    variable.TagId = input.TagId.Value;
                else if (!string.IsNullOrEmpty(input.TagName) && input.TagName != variable.Tag.Name)
                {
                    tag = _tagRepository.FirstOrDefault(p => p.Name == input.TagName);
                    if (tag != null)
                        variable.TagId = tag.Id;
                }

                // If a not-null UOM was passed in, use it, even if the empty string was passed in. Ignore null inputs.
                if ( input.UOM != null)
                    variable.UOM = input.UOM;
                // If UOM does not exist for the variable AND the tag has a UOM, then use the tag's UOM
                if (string.IsNullOrEmpty(variable.UOM))
                {
                    if (tag != null && !string.IsNullOrEmpty(tag.UOM))
                        variable.UOM = tag.UOM;
                    else if (variable.Tag != null)
                        variable.UOM = variable.Tag.UOM;
               }
            }
            // If we did not find a variable, attempt to create one. This will work if all required fields are specified.
            else
            {
                return CreateIowVariable(new CreateIowVariableInput
                {
                    Name = input.Name,
                    Description = input.Description,
                    TagId = input.TagId.HasValue ? input.TagId.Value : 0,
                    TagName = input.TagName,
                    UOM = input.UOM
                });
            }


            //We do not call Update method of the repository.
            //Because an application service method is a 'unit of work' scope as default.
            //ABP automatically saves all changes when a 'unit of work' scope ends (without any exception).

            return variable.MapTo<IowVariableDto>();
        }
    }
}
