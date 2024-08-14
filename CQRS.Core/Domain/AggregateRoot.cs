using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
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

            if(isNew) // If this is a new event, not one that has been retrieved from the event store
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
