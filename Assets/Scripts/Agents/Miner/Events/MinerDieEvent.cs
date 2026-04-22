public class MinerDieEvent : Event
{
    public int Id;
    public override void Initialize(params object[] parameters)
    {
        Id = (int)parameters[0];
    }
}
