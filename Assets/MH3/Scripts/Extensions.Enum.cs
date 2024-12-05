namespace MH3
{
    public static partial class Extensions
    {
        public static bool Evaluate(this Define.ComparisonType self, float a, float b)
        {
            switch (self)
            {
                case Define.ComparisonType.Equal:
                    return a == b;
                case Define.ComparisonType.NotEqual:
                    return a != b;
                case Define.ComparisonType.Greater:
                    return a > b;
                case Define.ComparisonType.Less:
                    return a < b;
                case Define.ComparisonType.GreaterOrEqual:
                    return a >= b;
                case Define.ComparisonType.LessOrEqual:
                    return a <= b;
                default:
                    return false;
            }
        }
    }
}
