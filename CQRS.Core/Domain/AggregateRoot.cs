using CQRS.Core.Events;
using CQRS.Core.Notifications;

namespace CQRS.Core.Domain
{
    /**
     * 
     * The Aggregate Root maintains the list of uncommitted changes in the form of events, that need to be applied to the aggregate and be persisted to the event store.
     * It manages which apply method is invoked on the concrete Aggregate based on the event type.
     * The Aggregate Root is the entity within the aggregate that is responsible for always keeping it in a consistent state.
     * Commits the changes that have been applied to the Aggregate to the event store.
     * 
     */
    public abstract class AggregateRoot : Notifiable
    {
        private readonly List<BaseEvent> _uncommitedChanges = [];

        public Guid Id { get; protected set; }
        public int Version { get; set; } = -1;

        public IEnumerable<BaseEvent> GetUncommitedChanges()
        {
            return _uncommitedChanges;
        }

        public void MarkChangesAsCommited()
        {
            _uncommitedChanges.Clear();
        }

        public void ApplyChange(BaseEvent @event, bool isNew)
        {
            // Gets the apply method implemented in the concrete subclass aggregate that receives the type of the event as argument
            var applyMethod = this.GetType().GetMethod("Apply", [@event.GetType()]);

            if(applyMethod == null)
            {
                throw new InvalidOperationException($"The Apply method was not found in the aggregate for {@event.GetType().Name}");
            }

            applyMethod.Invoke(this, [@event]);

            if(isNew && !HasNotifications()) // If this is a new event, not one that has been retrieved from the event store
            {
                _uncommitedChanges.Add(@event);
            }
        }

        protected void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event, isNew: true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events) // These will be events retrieved from the event store
        {
            foreach(var @event in events)
            {
                ApplyChange(@event, isNew: false);
            }
        }
    }
}
