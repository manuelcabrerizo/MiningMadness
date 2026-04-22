using System;
using UnityEditor;
using UnityEngine;

public class MinerDepositGemState : FsmState<Miner>
{
    private TaskScheduler taskScheduler = null;
    private float gemsDepositeRatio = 1.0f;
    public override void OnInitialize()
    {
        taskScheduler = new TaskScheduler();
    }
    public override void OnEnter()
    {
        taskScheduler.Schedule(DepositeGem, gemsDepositeRatio);
    }

    public override void OnExit()
    {
        taskScheduler.Clear();
    }

    public override void OnUpdate(float deltaTime)
    {
        taskScheduler.Tick(deltaTime);
    }

    private void DepositeGem()
    {
        int gems = owner.GetGems(1);
        if (gems > 0)
        {
            EventBus.Instance.Raise<MinerDepositGemsEvent>(gems);
            taskScheduler.Schedule(DepositeGem, gemsDepositeRatio);
        }
        else
        {
            owner.onGemDeposited?.Invoke();
        }
    }
}
