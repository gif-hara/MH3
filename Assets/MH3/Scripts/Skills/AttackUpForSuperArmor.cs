using System.Threading;
using HK;
using MH3.ActorControllers;
using R3;

namespace MH3.SkillSystems
{
    public class AttackUpForSuperArmor : Skill
    {
        public AttackUpForSuperArmor(int level) : base(Define.SkillType.AttackUpForSuperArmor, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            var superArmorTime = -9999.0f;
            owner.SpecController.OnInvokeSuperArmor
                .Subscribe(_ =>
                {
                    superArmorTime = UnityEngine.Time.time;
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
    }
}
