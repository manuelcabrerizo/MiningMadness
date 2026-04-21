public class MinerMoveToGemState : FsmState<Miner>
{
    public override void OnInitialize()
    {
    }
    public override void OnEnter()
    {
        owner.SetDestination(owner.TargetGem.transform.position);
    }
    public override void OnExit()
    {
    }
    public override void OnUpdate(float deltaTime)
    {
        if (owner.IsInDestination())
        {
            owner.onGemReach?.Invoke();
        }
    }
}
