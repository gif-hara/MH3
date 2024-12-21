using UnityEngine.EventSystems;

namespace MH3
{
    public class OnSelectSequencer : EventHandlerSequencer, ISelectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
            Play();
        }
    }
}
