#region Summary
/// <summary>
/// EventController and EventsController<T> are classes responsible for managing events in the game. 
/// - EventController handles basic events with no parameters, allowing listeners to be added, removed, and invoked.
/// - EventsController<T> extends this functionality by allowing events with a generic parameter, enabling more flexible event handling.
/// Both classes provide methods for invoking events and managing listeners, ensuring that events are triggered and handled as needed.
/// </summary>
#endregion
using System;

namespace SlowpokeStudio.Event
{
    public class EventController
    {
        public event Action baseEvent;
        public void InvokeEvent() => baseEvent?.Invoke();
        public void AddListener(Action listener) => baseEvent += listener;
        public void RemoveListener(Action listener) => baseEvent -= listener;
    }

    public class EventsController<T>
    {
        public event Action<T> baseEvent;
        public void InvokeEvents(T value) => baseEvent?.Invoke(value);
        public void AddListeners(Action<T> listener) => baseEvent += listener;
        public void RemoveListeners(Action<T> listener) => baseEvent -= listener;
    }
}
