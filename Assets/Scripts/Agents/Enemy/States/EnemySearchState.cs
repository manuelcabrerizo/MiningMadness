using UnityEngine;

public class EnemySearchState : FsmState<Enemy>
{
    private float searchRadius = 50.0f;

    public override void OnInitialize()
    {
    }

    public override void OnEnter()
    {
        Collider[] colliders = Physics.OverlapSphere(owner.transform.position, searchRadius, owner.minerLayer);
        if (colliders.Length == 0)
        {
            Debug.Log("Miner Not Found");
            owner.onMinerNotFound?.Invoke();
        }
        else
        {
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
                Miner miner = targetTransform.GetComponent<Miner>();
                owner.TargetMiner = miner;
                Debug.Log("Miner Found");
                owner.onMinerFound?.Invoke();
            }
            else
            {
                Debug.Log("Miner Not Found");
                owner.onMinerNotFound?.Invoke();
            }
        }
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
    }
}
