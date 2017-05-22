using System.Collections.Generic;

namespace Craftsmanship.Inventory.Tests
{
    internal class SpyDomainEvents: IDomainEvents
    {
        public IReadOnlyCollection<object> RaisedEvents => _raisedEvents;

        public void Raise<T>(T @event)
        {
            _raisedEvents.Add(@event);
        }

        public void Clear()
        {
            _raisedEvents.Clear();
        }

        private readonly List<object> _raisedEvents = new List<object>();
    }
}