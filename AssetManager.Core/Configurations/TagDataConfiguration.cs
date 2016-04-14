﻿using AssetManager.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Configurations
{
    public class TagDataConfiguration  : EntityTypeConfiguration<TagData>
    {
        public TagDataConfiguration()
        {
            Property(x => x.TenantId).IsRequired();
            Property(x => x.TagId).IsRequired();
            Property(x => x.Date).IsRequired();
            Property(x => x.Value).IsRequired();
        }
    }
}
