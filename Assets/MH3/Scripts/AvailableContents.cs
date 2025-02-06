using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class AvailableContents
    {
        [SerializeField]
        private SerializableHashSet<string> contents = new();

        [SerializeField]
        private SerializableHashSet<string> newContents = new();

        public SerializableHashSet<string> NewContents => newContents;

        public void Add(string content)
        {
            if (contents.Contains(content))
            {
                return;
            }
            contents.Add(content);
            newContents.Add(content);
        }

        public bool Contains(string content)
        {
            return contents.Contains(content);
        }

        public void ClearNewContents()
        {
            newContents.Clear();
        }

        public void RemoveNewContent(string content)
        {
            newContents.Remove(content);
        }

        public static class Key
        {
            public const string FirstPlay = "FirstPlay";

            public const string AcquireInstanceWeapon = "Acquire.InstanceWeapon";

            public const string AcquireInstanceArmor = "Acquire.InstanceArmor";

            public const string AcquireInstanceSkillCore = "Acquire.InstanceSkillCore";

            public const string FirstBattle = "FirstBattle";
            
            public const string FirstInvokeSharpen = "FirstInvokeSharpen";
            
            public const string FirstInvokeEndurance = "FirstInvokeEndurance";

            public static string GetQuestClear(string questSpecId)
            {
                return $"Clear.{questSpecId}";
            }

            public static string GetSeenWeapon(int weaponSpecId)
            {
                return $"Seen.Weapon.{weaponSpecId}";
            }

            public static string GetSeenArmor(int armorSpecId)
            {
                return $"Seen.Armor.{armorSpecId}";
            }

            public static string GetSeenSkillCore(int skillCoreSpecId)
            {
                return $"Seen.SkillCore.{skillCoreSpecId}";
            }

            public static string GetSeenInstanceWeapon(int weaponInstanceId)
            {
                return $"Seen.InstanceWeapon.{weaponInstanceId}";
            }

            public static string GetSeenInstanceArmor(int armorInstanceId)
            {
                return $"Seen.InstanceArmor.{armorInstanceId}";
            }

            public static string GetSeenInstanceSkillCore(int skillCoreInstanceId)
            {
                return $"Seen.InstanceSkillCore.{skillCoreInstanceId}";
            }
        }
    }
}
