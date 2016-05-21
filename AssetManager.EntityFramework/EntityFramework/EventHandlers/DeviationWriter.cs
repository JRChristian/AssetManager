using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using AssetManager.Entities;
using AssetManager.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetManager.EntityFramework.Repositories;
using Abp.Domain.Uow;

namespace AssetManager.EventHandlers
{
    public class DeviationWriter :
        IEventHandler<EntityCreatedEventData<TagDataRawUpdate>>,
        IEventHandler<EntityChangedEventData<TagDataRawUpdate>>,
        ITransientDependency
    {
        private readonly IowManager _iowManager;
        private readonly TagDataRawUpdateRepository _tagDataRawUpdateRepository;

        public DeviationWriter(IowManager iowManager, TagDataRawUpdateRepository tagDataRawUpdateRepository)
        {
            _iowManager = iowManager;
            _tagDataRawUpdateRepository = tagDataRawUpdateRepository;
        }

        [UnitOfWork]
        public virtual void HandleEvent(EntityCreatedEventData<TagDataRawUpdate> eventData)
        {
            // Go look for deviations in the period of time specified
            _iowManager.DetectDeviations(eventData.Entity.Tag, eventData.Entity.StartTimestamp, eventData.Entity.EndTimestamp);

            // Delete the record for the working table
            _tagDataRawUpdateRepository.Delete(eventData.Entity.Id);
        }

        [UnitOfWork]
        public virtual void HandleEvent(EntityChangedEventData<TagDataRawUpdate> eventData)
        {
            // Do nothing -- this event seems to be raised when the underlying record is deleted in the previous event handler
        }
    }
}
