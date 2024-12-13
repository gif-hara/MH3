namespace MH3.AbnormalStatusSystems
{
    public static class AbnormalStatusFactory
    {
        public static IAbnormalStatus Create(Define.AbnormalStatusType type)
        {
            return type switch
            {
                Define.AbnormalStatusType.Poison => new Poison(),
                Define.AbnormalStatusType.Paralysis => new Paralysis(),
                Define.AbnormalStatusType.Collapse => new Collapse(),
                _ => throw new System.NotImplementedException($"未対応の異常状態です {type}"),
            };
        }
    }
}
