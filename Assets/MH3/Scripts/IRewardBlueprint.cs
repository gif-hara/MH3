namespace MH3
{
    public interface IRewardBlueprint
    {
        int Id { get; }
        
        Define.RewardType Type { get; }
    }
}
