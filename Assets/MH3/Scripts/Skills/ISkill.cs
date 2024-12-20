using System;
using MH3.ActorControllers;

namespace MH3.SkillSystems
{
    public interface ISkill
    {
        float GetParameter(Define.ActorParameterType type, Actor owner, Actor target);

        int GetParameterInt(Define.ActorParameterType type, Actor owner, Actor target);

        float GetParameter(Define.ActorParameterType type, Actor owner);

        int GetParameterInt(Define.ActorParameterType type, Actor owner);

        ISkill RegisterParameterSelector(Define.ActorParameterType type, Func<Actor, Actor, float> selector);

        ISkill RegisterParameterSelector(Define.ActorParameterType type, Func<Actor, float> selector);
    }
}
