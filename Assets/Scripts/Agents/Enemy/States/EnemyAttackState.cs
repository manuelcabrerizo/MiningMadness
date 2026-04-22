using System;
using UnityEngine;

public class EnemyAttackState : FsmState<Enemy>
{
    TaskScheduler taskScheduler = null;
    private float updateMinerPositionRatio = 1.0f;
    private float minerScapeDistance = 5.0f;

    public override void OnInitialize()
    {
        taskScheduler = new TaskScheduler();
    }

    public override void OnEnter()
    {
        taskScheduler.Schedule(UpdateMinerPosition, updateMinerPositionRatio);
    }

    public override void OnExit()
    {
        taskScheduler.Clear();
    }

    public override void OnUpdate(float deltaTime)
    {
        taskScheduler.Tick(Time.deltaTime);
    }

    private void UpdateMinerPosition()
    {
        Debug.Log("Update Miner Position");
        Vector3 enemyPosition = owner.transform.position;
        Vector3 minerPosition = owner.TargetMiner.transform.position;
        if ((minerPosition - enemyPosition).sqrMagnitude > (minerScapeDistance * minerScapeDistance))
        {
            Debug.Log("Miner Scape");
            owner.TargetMiner = null;
            owner.onMinerScape?.Invoke();
        }
        owner.SetDestination(minerPosition);
        taskScheduler.Schedule(UpdateMinerPosition, updateMinerPositionRatio);
    }
}