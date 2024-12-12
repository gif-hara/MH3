using System;
using System.Collections.Generic;
using MH3.ActorControllers;

namespace MH3.SkillSystems
{
    public sealed class Skill : ISkill
    {
        public int Level { get; private set; }

        private readonly Dictionary<Define.ActorParameterType, Func<Actor, Actor, float>> parameterSelectors = new();

        public Skill(int level)
        {
            Level = level;
        }

        public float GetParameter(Define.ActorParameterType type, Actor owner, Actor target)
        {
            return parameterSelectors[type](owner, target);
        }

        public int GetParameterInt(Define.ActorParameterType type, Actor owner, Actor target)
        {
            return (int)GetParameter(type, owner, target);
        }

        public ISkill RegisterParameterSelector(Define.ActorParameterType type, Func<Actor, Actor, float> selector)
        {
            parameterSelectors[type] = selector;
            return this;
        }
    }
}
