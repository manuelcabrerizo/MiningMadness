using System;
using UnityEngine;

public class EnemyAttackState : FsmState<Enemy>
{
    TaskScheduler taskScheduler = null;
    private float updateMinerPositionRatio = 1.5f;
    private float minerScapeDistance = 10.0f;
    private float attackRadio = 5.0f;
    private float attackTimeRatio = 0.5f;
    private int attackForce = 2;

    public override void OnInitialize()
    {
        taskScheduler = new TaskScheduler();
    }

    public override void OnEnter()
    {
        taskScheduler.Clear();
        taskScheduler.Schedule(UpdateMinerPosition, updateMinerPositionRatio);
        taskScheduler.Schedule(Attack, attackTimeRatio);
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        taskScheduler.Tick(Time.deltaTime);
    }

    private void UpdateMinerPosition()
    {
        if (owner.TargetMiner == null)
        {
            return;
        }

        Vector3 enemyPosition = owner.transform.position;
        Vector3 minerPosition = owner.TargetMiner.transform.position;
        if ((minerPosition - enemyPosition).sqrMagnitude > (minerScapeDistance * minerScapeDistance))
        {
            owner.TargetMiner = null;
            owner.onMinerScape?.Invoke();
        }
        owner.SetDestination(minerPosition);
        taskScheduler.Schedule(UpdateMinerPosition, updateMinerPositionRatio);
    }

    private void Attack()
    {
        Collider[] colliders = Physics.OverlapSphere(owner.transform.position, attackRadio, owner.minerLayer);
        foreach (Collider collider in colliders)
        {
            Miner miner = collider.gameObject.GetComponent<Miner>();
            if (miner.Id == owner.TargetMiner.Id)
            {
                if (miner.TakeDamage(attackForce))
                {
                    return;
                }
            }
        }
        taskScheduler.Schedule(Attack, attackTimeRatio);
    }
}