using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.IOWs.Dtos
{
    public class GetLimitStatsByDayOutput
    {
        public List<LimitStatsByDayDto> Stats { get; set; }
    }
}
