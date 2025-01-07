using System.Threading;
using HK;
using MH3.ActorControllers;
using R3;

namespace MH3.SkillSystems
{
    public class AttackUpForRecoveryCommand : Skill
    {
        private float recoveryCommandTime = -9999.0f;

        public AttackUpForRecoveryCommand(int level) : base(Define.SkillType.AttackUpForRecoveryCommand, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.RecoveryCommandType = Define.RecoveryCommandType.AttackUp;
            owner.ActionController.OnBeginRecoveryCommand
                .Where(x => x == Define.RecoveryCommandType.AttackUp)
                .Subscribe(this, static (_, @this) =>
                {
                    @this.recoveryCommandTime = UnityEngine.Time.time;
                })
                .RegisterTo(scope);
            owner.SpecController.Attack.RegisterAdds(
                "Skill.AttackUpForRecoveryCommand",
                () =>
                {
                    var diffTime = UnityEngine.Time.time - recoveryCommandTime;
                    if (diffTime > TinyServiceLocator.Resolve<GameRules>().SkillAttackUpForRecoveryCommandDuration)
                    {
                        return 0;
                    }
                    return TinyServiceLocator.Resolve<MasterData>().SkillAttackUpForRecoveryCommand.GetFromLevel(Level).Value;
                }
                );
        }

        public override void Reset()
        {
            recoveryCommandTime = -9999.0f;
        }
    }
}
