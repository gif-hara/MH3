using System;
using MH3.ActorControllers;

namespace MH3.SkillSystems
{
    public interface ISkill
    {
        Define.SkillType SkillType { get; }

        int Level { get; }

        void Attach(Actor owner);

        float GetParameter(Define.ActorParameterType type, Actor owner);

        int GetParameterInt(Define.ActorParameterType type, Actor owner);

        ISkill RegisterParameterSelector(Define.ActorParameterType type, Func<Actor, float> selector);
    }
}
