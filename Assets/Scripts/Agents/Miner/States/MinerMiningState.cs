using System;

public class MinerMiningState : FsmState<Miner>
{
    private TaskScheduler taskScheduler = null;
    private float gemsCollectionRatio = 2.0f;
    private float miningTimeDuration = 11.0f;

    public override void OnInitialize()
    {
        taskScheduler = new TaskScheduler();
    }

    public override void OnEnter()
    {
        taskScheduler.Schedule(CollectGem, gemsCollectionRatio);
        taskScheduler.Schedule(FinishCollecting, miningTimeDuration);
    }

    public override void OnExit()
    {
        taskScheduler.Clear();
        owner.TargetGem.Release();
        owner.TargetGem = null;
    }

    public override void OnUpdate(float deltaTime)
    {
        taskScheduler.Tick(deltaTime);
    }

    private void CollectGem()
    {
        owner.AddGems(2);
        taskScheduler.Schedule(CollectGem, gemsCollectionRatio);
    }

    private void FinishCollecting()
    {
        owner.onGemCollected?.Invoke();
    }
}
