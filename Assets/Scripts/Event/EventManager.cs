#region Summary
// EventManager serves as a central hub for managing game events in the HouseDefence namespace.
// This class uses the Singleton pattern to ensure only one instance exists throughout the game.
// Events are initialized in the Awake method, and the class persists across scene transitions.
#endregion
using UnityEngine;

namespace SlowpokeStudio.Event
{
    public class EventManager : MonoBehaviour
    {
        public EventController OnLevelCompleteEvent { get; private set; }
        public EventController OnNextCompletedEvent { get; private set; }
        //public EventsController<float> OnEnemyDeathEvent { get; private set; }

        public EventManager()
        {
            OnLevelCompleteEvent = new EventController();
            OnNextCompletedEvent = new EventController();
            //OnEnemyDeathEvent = new EventsController<float>();
        }
    }
}

