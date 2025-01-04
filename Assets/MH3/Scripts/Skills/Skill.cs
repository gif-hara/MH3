using System;
using System.Collections.Generic;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public abstract class Skill : ISkill
    {
        private readonly Dictionary<Define.ActorParameterType, Func<Actor, float>> parameterSelectors = new();

        public Define.SkillType SkillType { get; private set; }

        public int Level { get; private set; }

        public Skill(Define.SkillType skillType, int level)
        {
            SkillType = skillType;
            Level = level;
        }

        public float GetParameter(Define.ActorParameterType type, Actor owner)
        {
            if (parameterSelectors.ContainsKey(type))
            {
                return parameterSelectors[type](owner);
            }
            return 0.0f;
        }

        public int GetParameterInt(Define.ActorParameterType type, Actor owner)
        {
            return Mathf.FloorToInt(GetParameter(type, owner));
        }

        public ISkill RegisterParameterSelector(Define.ActorParameterType type, Func<Actor, float> selector)
        {
            parameterSelectors[type] = selector;
            return this;
        }

        public abstract void Attach(Actor owner);
    }
}
