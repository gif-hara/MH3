using UnitySequencerSystem;

namespace MH3.ContainerEvaluators
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContainerEvaluator
    {
        bool Evaluate(Container container);

        public enum CompareType
        {
            Equals,
            NotEquals,
            GreaterThan,
            GreaterThanOrEquals,
            LessThan,
            LessThanOrEquals
        }
    }
}
