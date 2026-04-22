using UnityEngine;

public class MinerIdleState : FsmState<Miner>
{
    private TaskScheduler taskScheduler = null;
    private float searchRadius = 100.0f;
    private float searchRate = 2.0f;

    public override void OnInitialize()
    {
        taskScheduler = new TaskScheduler();
    }
    
    public override void OnEnter()
    {
        taskScheduler.Clear();
        if (owner.IsFull())
        {
            owner.onGemCollected?.Invoke();
        }
        else
        {
            taskScheduler.Schedule(SearchGem, searchRate);
        }
    }
    
    public override void OnExit()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        taskScheduler.Tick(deltaTime);
    }

    private void SearchGem()
    {
        Collider[] colliders = Physics.OverlapSphere(owner.transform.position, searchRadius, owner.gemLayer);
        if (colliders.Length == 0)
        {
            taskScheduler.Schedule(SearchGem, searchRate);
            return;
        }

        owner.TargetGem = null;
        foreach (Collider collider in colliders)
        {
            Gem gem = collider.gameObject.GetComponent<Gem>();
            if (!gem.IsOccupy)
            {
                gem.Reseve();
                owner.TargetGem = gem;
                break;
            }
        }

        if (owner.TargetGem != null)
        {
            owner.onGemFound?.Invoke();
        }
        else 
        {
            taskScheduler.Schedule(SearchGem, searchRate);
        }
    }
}
