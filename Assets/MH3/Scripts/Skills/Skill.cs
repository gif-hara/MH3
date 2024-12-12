using System;
using System.Collections.Generic;
using MH3.ActorControllers;

namespace MH3.SkillSystems
{
    public sealed class Skill : ISkill
    {
        public int Level { get; private set; }

        private readonly Dictionary<Define.ActorParameterType, Func<Actor, float>> parameterOwnerOnlySelectors = new();

        private readonly Dictionary<Define.ActorParameterType, Func<Actor, Actor, float>> parameterOwnerTargetSelectors = new();

        public Skill(int level)
        {
            Level = level;
        }

        public float GetParameter(Define.ActorParameterType type, Actor owner, Actor target)
        {
            return parameterOwnerTargetSelectors[type](owner, target);
        }

        public int GetParameterInt(Define.ActorParameterType type, Actor owner, Actor target)
        {
            return (int)GetParameter(type, owner, target);
        }

        public float GetParameter(Define.ActorParameterType type, Actor owner)
        {
            return parameterOwnerOnlySelectors[type](owner);
        }

        public int GetParameterInt(Define.ActorParameterType type, Actor owner)
        {
            return (int)GetParameter(type, owner);
        }

        public ISkill RegisterParameterSelector(Define.ActorParameterType type, Func<Actor, Actor, float> selector)
        {
            parameterOwnerTargetSelectors[type] = selector;
            return this;
        }

        public ISkill RegisterParameterSelector(Define.ActorParameterType type, Func<Actor, float> selector)
        {
            parameterOwnerOnlySelectors[type] = selector;
            return this;
        }
    }
}
