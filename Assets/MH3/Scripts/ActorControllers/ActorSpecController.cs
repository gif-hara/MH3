namespace MH3
{
    public class ActorSpecController
    {
        private readonly ActorSpec spec;

        public ActorSpecController(ActorSpec spec)
        {
            this.spec = spec;
        }

        public float MoveSpeed => spec.MoveSpeed;

        public float RotationSpeed => spec.RotationSpeed;
    }
}
