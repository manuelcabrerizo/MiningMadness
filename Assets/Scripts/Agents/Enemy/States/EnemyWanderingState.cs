using UnityEngine;

public class EnemyWanderingState : FsmState<Enemy>
{
    private float minSearchInterval = 4;
    private float maxSearchInterval = 10;
    private TaskScheduler taskScheduler = null;

    public override void OnInitialize()
    {
        taskScheduler = new TaskScheduler();
    }

    public override void OnEnter()
    {
        taskScheduler.Clear();
        SetRandomDestination();
        taskScheduler.Schedule(SearchForMiner, Random.Range(minSearchInterval, maxSearchInterval));
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        taskScheduler.Tick(Time.deltaTime);
        if (owner.IsInDestination())
        {
            SetRandomDestination();
        }
    }

    private void SearchForMiner()
    {
        owner.onLookForMiners?.Invoke();
    }

    private void SetRandomDestination()
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized;
        Vector3 direction = new Vector3(randomPoint.x, 0.0f, randomPoint.y);
        Vector3 destination = owner.transform.position + direction * Random.Range(10, 20);
        owner.SetDestination(destination);
    }
}
