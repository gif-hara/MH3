using UnityEngine.EventSystems;

namespace MH3
{
    public class OnDeselectSequencer : EventHandlerSequencer, IDeselectHandler
    {
        public void OnDeselect(BaseEventData eventData)
        {
            Play();
        }
    }
}
