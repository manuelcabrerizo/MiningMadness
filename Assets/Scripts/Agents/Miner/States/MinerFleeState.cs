using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class MinerFleeState : FsmState<Miner>
{
    private TaskScheduler taskScheduler = null;
    private float safeRadius = 2.5f;
    private float checkForEnemiesInterval = 0.25f;

    public override void OnInitialize()
    {
        taskScheduler = new TaskScheduler();
    }

    public override void OnEnter()
    {
        taskScheduler.Clear();
        if (owner.TargetGem != null)
        {
            owner.TargetGem.Release();
            owner.TargetGem = null;
        }
        SetRandomDestination();
        taskScheduler.Schedule(CheckForCloseEnemies, checkForEnemiesInterval);
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        taskScheduler.Tick(deltaTime);
        if (owner.IsInDestination())
        {
            SetRandomDestination();
        }
    }

    private void SetRandomDestination()
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized;
        Vector3 direction = new Vector3(randomPoint.x, 0.0f, randomPoint.y);
        Vector3 destination = owner.transform.position + direction * Random.Range(20, 30);
        owner.SetDestination(destination);
    }

    private void CheckForCloseEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(owner.transform.position, safeRadius, owner.enemyLayer);
        if (colliders.Length == 0)
        {
            owner.onEscapeFromEnemy?.Invoke();
        }
        else
        {
            taskScheduler.Schedule(CheckForCloseEnemies, checkForEnemiesInterval);
        }
    }
}