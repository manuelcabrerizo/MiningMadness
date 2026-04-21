public class MinerMiningState : FsmState<Miner>
{
    private TaskScheduler taskScheduler = null;
    private float miningTimeDuration = 4.0f;

    public override void OnInitialize()
    {
        taskScheduler = new TaskScheduler();
    }

    public override void OnEnter()
    {
        taskScheduler.Schedule(CollectGem, miningTimeDuration);
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
        owner.onGemCollected?.Invoke();
    }
}
