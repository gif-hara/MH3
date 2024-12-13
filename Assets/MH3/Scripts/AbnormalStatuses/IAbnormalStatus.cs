using Cysharp.Threading.Tasks;
using MH3.ActorControllers;

namespace MH3.AbnormalStatusSystems
{
    public interface IAbnormalStatus
    {
        UniTaskVoid Apply(Actor target);
    }
}
