using System;
using MH3.ActorControllers;

namespace MH3.SkillSystems
{
    public interface ISkill
    {
        int Level { get; }

        float GetParameter(Define.ActorParameterType type, Actor owner, Actor target);

        int GetParameterInt(Define.ActorParameterType type, Actor owner, Actor target);

        ISkill RegisterParameterSelector(Define.ActorParameterType type, Func<Actor, Actor, float> selector);
    }
}
