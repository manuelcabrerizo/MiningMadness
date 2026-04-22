public class MinerDepositGemsEvent : Event
{
    public int Amount;
    public override void Initialize(params object[] parameters)
    {
        Amount = (int)parameters[0];
    }
}
