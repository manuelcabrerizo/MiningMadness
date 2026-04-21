using UnityEditor;
using UnityEngine;

public class MinerDepositGoldState : FsmState<Miner>
{
    private float searchRadius = 100.0f;
    public override void OnInitialize()
    {
    }
    public override void OnEnter()
    {
        owner.TargetGem.Deposite();
        owner.TargetGem = null;

        Collider[] colliders = Physics.OverlapSphere(owner.transform.position, searchRadius, owner.baseLayer);

        Transform targetTransform = null;
        float minSqrDistance = float.MaxValue;
        foreach (Collider collider in colliders)
        {
            float sqrDistance = (collider.transform.position - owner.transform.position).sqrMagnitude;
            if (sqrDistance < minSqrDistance)
            {
                targetTransform = collider.transform;
                minSqrDistance = sqrDistance;
            }
        }
        if (targetTransform != null)
        {
            owner.SetDestination(targetTransform.position);
        }
    }
    
    public override void OnExit()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        if (owner.IsInDestination())
        {
            owner.onGemDeposited?.Invoke();
        }
    }
}