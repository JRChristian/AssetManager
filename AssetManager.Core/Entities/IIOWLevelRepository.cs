﻿using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Entities
{
    public interface IIOWLevelRepository : IRepository<IOWLevel, long>
    {
    }
}