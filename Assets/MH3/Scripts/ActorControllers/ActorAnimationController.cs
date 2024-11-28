namespace MH3.ActorControllers
{
    public class ActorAnimationController
    {
        private readonly SimpleAnimation simpleAnimation;

        public ActorAnimationController(SimpleAnimation simpleAnimation)
        {
            this.simpleAnimation = simpleAnimation;
        }

        public void CrossFade(string animationName, float fadeLength)
        {
            var state = simpleAnimation.GetState(animationName);
            if(state.enabled)
            {
                simpleAnimation.Rewind();
            }
            else
            {
                simpleAnimation.CrossFade(animationName, fadeLength);
            }
        }
    }
}
