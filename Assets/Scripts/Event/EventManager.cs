using UnityEngine;

namespace SlowpokeStudio.Event
{
    public class EventManager : MonoBehaviour
    {
        public EventController OnLevelCompleteEvent { get; private set; }
        public EventsController<int> OnCoinsChanged { get; private set; }

        public EventManager()
        {
            OnLevelCompleteEvent = new EventController();
            OnCoinsChanged =  new EventsController<int>();
        }
    }
}

