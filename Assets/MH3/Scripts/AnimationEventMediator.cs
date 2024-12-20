using HK;
using UnityEngine;

namespace MH3
{
    public class AnimationEventMediator : MonoBehaviour
    {
        public void PlaySfx(string sfxName)
        {
            TinyServiceLocator.Resolve<AudioManager>().PlaySfx(sfxName);
        }
    }
}
