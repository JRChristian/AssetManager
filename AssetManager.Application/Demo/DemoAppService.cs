using AssetManager.Demo.Dtos;
using AssetManager.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Demo
{
    public class DemoAppService : AssetManagerAppServiceBase, IDemoAppService
    {
        private readonly IIowManager _iowManager;
        private readonly ITagManager _tagManager;

        public DemoAppService(
            IIowManager iowManager,
            ITagManager tagManager
            )
        {
            _iowManager = iowManager;
            _tagManager = tagManager;
        }

        public GetDemoStatusOutput GetDemoStatus(GetDemoStatusInput input)
        {
            GetDemoStatusOutput output = new GetDemoStatusOutput
            {
                NumberTags = _tagManager.GetAllListTag().Count(),
                NumberVariables = _iowManager.GetAllVariables().Count(),
                NumberLimits = _iowManager.GetAllLimits().Count(),
                EarliestTagDataTimestamp = _tagManager.GetMinimumTagDataTimestamp(),
                LatestTagDataTimestamp = _tagManager.GetMaximumTagDataTimestamp()
            };
            output.TagDataTimeRangeDays = Math.Ceiling((output.LatestTagDataTimestamp - output.EarliestTagDataTimestamp).TotalDays);
            output.NumberHoursToMoveAhead = Math.Floor((DateTime.Now - output.LatestTagDataTimestamp).TotalHours);

            return output;
        }

        public UpdateTagDataForDemoOutput UpdateTagDataForDemo(UpdateTagDataForDemoInput input)
        {
            int daysToAdd = input.DaysToAdd;
            if( daysToAdd <= 0 )
            {
                // What's the time range in the data?
                DateTime earliestTagDataTimestamp = _tagManager.GetMinimumTagDataTimestamp();
                DateTime latestTagDataTimestamp = _tagManager.GetMaximumTagDataTimestamp();
                daysToAdd = (int)Math.Ceiling((latestTagDataTimestamp - earliestTagDataTimestamp).TotalDays);
            }

            int numberUpdates = _tagManager.UpdateTagDataForDemo(daysToAdd);
            return new UpdateTagDataForDemoOutput { NumberUpdates = numberUpdates };
        }

    }
}
