using System.Threading;
using HK;
using MH3.ActorControllers;
using R3;

namespace MH3.SkillSystems
{
    public class AttackUpForSuperArmor : Skill
    {
        private float superArmorTime = -9999.0f;
        public AttackUpForSuperArmor(int level) : base(Define.SkillType.AttackUpForSuperArmor, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.OnInvokeSuperArmor
                .Subscribe(this, static (_, @this) =>
                {
                    @this.superArmorTime = UnityEngine.Time.time;
                })
                .RegisterTo(owner.destroyCancellationToken);
            owner.SpecController.Attack.RegisterAdds(
                "Skill.AttackUpForSuperArmor",
                () =>
                {
                    var diffTime = UnityEngine.Time.time - superArmorTime;
                    if (diffTime > TinyServiceLocator.Resolve<GameRules>().SkillAttackUpForSuperArmorDuration)
                    {
                        return 0;
                    }
                    return TinyServiceLocator.Resolve<MasterData>().SkillAttackUpForSuperArmor.GetFromLevel(Level).Value;
                }
                );
        }

        public override void Reset()
        {
            superArmorTime = -9999.0f;
        }
    }
}
