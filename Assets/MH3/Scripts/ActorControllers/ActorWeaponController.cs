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
                        Object.Destroy(weapon.gameObject);
                    }
                    if(id == 0)
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
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
