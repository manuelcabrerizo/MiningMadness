using System;
using UnityEditor;
using UnityEngine;

public class MinerDepositGoldState : FsmState<Miner>
{
    private TaskScheduler taskScheduler = null;
    private float gemsDepositeRatio = 2.0f;
    private float depositTimeDuration = 11.0f;
    public override void OnInitialize()
    {
        taskScheduler = new TaskScheduler();
    }
    public override void OnEnter()
    {
        taskScheduler.Schedule(DepositeGem, gemsDepositeRatio);
        taskScheduler.Schedule(FinishDeposit, depositTimeDuration);
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
        int gems = owner.GetGems(2);
        EventBus.Instance.Raise<MinerDepositGemsEvent>(gems);
        taskScheduler.Schedule(DepositeGem, gemsDepositeRatio);
    }

    private void FinishDeposit()
    {
        owner.onGemDeposited?.Invoke();
    }
}