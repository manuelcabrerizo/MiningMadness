public class MinerModifyGemsEvent : Event
{
    public int Id;
    public int Amount;
    public override void Initialize(params object[] parameters)
    {
        Id = (int)parameters[0];
        Amount = (int)parameters[1];
    }
}
