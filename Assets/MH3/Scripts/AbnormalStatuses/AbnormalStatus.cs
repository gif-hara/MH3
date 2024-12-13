using Cysharp.Threading.Tasks;
using MH3.ActorControllers;

namespace MH3.AbnormalStatusSystems
{
    public abstract class AbnormalStatus : IAbnormalStatus
    {
        public abstract UniTaskVoid Apply(Actor target);
    }
}
