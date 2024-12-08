using System.Collections.Generic;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;

namespace MH3
{
    public class ActorWeaponController
    {
        private readonly List<Weapon> weapons = new();

        public ActorWeaponController(Actor actor)
        {
            actor.SpecController.WeaponId
                .Subscribe((this, actor), static (id, t) =>
                {
                    var (@this, actor) = t;
                    foreach (var weapon in @this.weapons)
                    {
                        weapon.Dispose();
                        Object.Destroy(weapon.gameObject);
                    }
                    @this.weapons.Clear();
                    if (id == 0)
                    {
                        return;
                    }

                    var masterData = TinyServiceLocator.Resolve<MasterData>();
                    var weaponSpec = masterData.WeaponSpecs.Get(id);
                    foreach (var element in weaponSpec.ModelData.Elements)
                    {
                        var weapon = Object.Instantiate(element.ModelPrefab, actor.LocatorHolder.Get(element.LocatorName));
                        weapon.transform.localPosition = Vector3.zero;
                        weapon.transform.localRotation = Quaternion.identity;
                        @this.weapons.Add(weapon);
                        weapon.Setup(actor);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        public void SetActiveTrail(string key, bool isActive)
        {
            foreach (var weapon in weapons)
            {
                weapon.SetActiveTrail(key, isActive);
            }
        }

        public void SetDeactiveAllTrail()
        {
            foreach (var weapon in weapons)
            {
                weapon.SetDeactiveAllTrail();
            }
        }
    }
}
