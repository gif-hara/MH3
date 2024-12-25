using System.Collections.Generic;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;

namespace MH3
{
    public class ActorArmorController
    {
        private readonly List<Armor> armorHeads = new();
        
        private readonly List<Armor> armorArms = new();
        
        private readonly List<Armor> armorBodies = new();

        public ActorArmorController(Actor actor)
        {
            actor.SpecController.ArmorHeadId
                .Subscribe((this, actor), static (armorId, t) =>
                {
                    var (@this, actor) = t;
                    foreach (var armor in @this.armorHeads)
                    {
                        Object.Destroy(armor.gameObject);
                    }
                    @this.armorHeads.Clear();
                    if (armorId == 0)
                    {
                        return;
                    }
                    
                    var masterData = TinyServiceLocator.Resolve<MasterData>();
                    var armorSpec = masterData.ArmorSpecs.Get(armorId);
                    foreach (var element in armorSpec.ModelData.Elements)
                    {
                        var armor = Object.Instantiate(element.ModelPrefab, actor.LocatorHolder.Get(element.LocatorName));
                        armor.transform.localPosition = Vector3.zero;
                        armor.transform.localRotation = Quaternion.identity;
                        @this.armorHeads.Add(armor);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
            actor.SpecController.ArmorArmsId
                .Subscribe((this, actor), static (armorId, t) =>
                {
                    var (@this, actor) = t;
                    foreach (var armor in @this.armorArms)
                    {
                        Object.Destroy(armor.gameObject);
                    }
                    @this.armorArms.Clear();
                    if (armorId == 0)
                    {
                        return;
                    }
                    
                    var masterData = TinyServiceLocator.Resolve<MasterData>();
                    var armorSpec = masterData.ArmorSpecs.Get(armorId);
                    foreach (var element in armorSpec.ModelData.Elements)
                    {
                        var armor = Object.Instantiate(element.ModelPrefab, actor.LocatorHolder.Get(element.LocatorName));
                        armor.transform.localPosition = Vector3.zero;
                        armor.transform.localRotation = Quaternion.identity;
                        @this.armorArms.Add(armor);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
            actor.SpecController.ArmorBodyId
                .Subscribe((this, actor), static (armorId, t) =>
                {
                    var (@this, actor) = t;
                    foreach (var armor in @this.armorBodies)
                    {
                        Object.Destroy(armor.gameObject);
                    }
                    @this.armorBodies.Clear();
                    if (armorId == 0)
                    {
                        return;
                    }
                    
                    var masterData = TinyServiceLocator.Resolve<MasterData>();
                    var armorSpec = masterData.ArmorSpecs.Get(armorId);
                    foreach (var element in armorSpec.ModelData.Elements)
                    {
                        var armor = Object.Instantiate(element.ModelPrefab, actor.LocatorHolder.Get(element.LocatorName));
                        armor.transform.localPosition = Vector3.zero;
                        armor.transform.localRotation = Quaternion.identity;
                        @this.armorBodies.Add(armor);
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }
    }
}
