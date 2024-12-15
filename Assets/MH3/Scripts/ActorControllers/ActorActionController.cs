namespace MH3.ActorControllers
{
    public class ActorActionController
    {
        private readonly Actor actor;

        public ActorActionController(Actor actor)
        {
            this.actor = actor;
        }

        public bool TryDodge()
        {
            return actor.StateMachine.TryChangeState(
                actor.SpecController.DodgeSequences,
                containerAction: c =>
                {
                    c.Register("DodgeName", "Dodge");
                });
        }
    }
}
