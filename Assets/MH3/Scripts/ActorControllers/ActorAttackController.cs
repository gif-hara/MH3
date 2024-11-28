namespace MH3.ActorControllers
{
    public class ActorAttackController
    {
        private readonly Actor actor;

        private int attackCount = 0;

        private readonly string[] attackNames = new string[]
        {
            "Attack.0",
            "Attack.1",
            "Attack.0",
            "Attack.1",
        };

        public ActorAttackController(Actor actor)
        {
            this.actor = actor;
        }

        public bool TryAttack()
        {
            if (attackCount >= attackNames.Length)
            {
                return false;
            }

            if (actor.StateMachine.TryChangeState(
                actor.SpecController.AttackSequences,
                containerAction: c =>
                {
                    c.Register("AttackName", attackNames[attackCount++]);
                }))
            {
                return true;
            }

            return false;
        }

        public void ResetAttackCount()
        {
            attackCount = 0;
        }
    }
}
