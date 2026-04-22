public class MinerMiningState : FsmState<Miner>
{
    private TaskScheduler taskScheduler = null;
    private float gemsCollectionRatio = 2.0f;

    public override void OnInitialize()
    {
        taskScheduler = new TaskScheduler();
    }

    public override void OnEnter()
    {
        taskScheduler.Schedule(CollectGem, gemsCollectionRatio);
    }

    public override void OnExit()
    {
        taskScheduler.Clear();
    }

    public override void OnUpdate(float deltaTime)
    {
        taskScheduler.Tick(deltaTime);
    }

    private void CollectGem()
    {
        owner.AddGems(owner.TargetGem.GetGems(2));
        if (owner.TargetGem.IsEmpty())
        {
            owner.TargetGem.Delete();
            owner.TargetGem = null;
            owner.onGemCollected?.Invoke();
        }
        else if (owner.IsFull())
        {
            owner.TargetGem.Release();
            owner.TargetGem = null;
            owner.onGemCollected?.Invoke();
        }
        else
        {
            taskScheduler.Schedule(CollectGem, gemsCollectionRatio);
        }
    }
}
