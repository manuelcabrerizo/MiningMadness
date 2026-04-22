using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.GridLayoutGroup;

public class Miner : Agent
{
    private static int generation = 0;

    public UnityEvent onGemFound;
    public UnityEvent onGemReach;
    public UnityEvent onGemCollected;
    public UnityEvent onBaseReach;
    public UnityEvent onGemDeposited;
    public UnityEvent onEnemyAttack;
    public UnityEvent onEscapeFromEnemy;

    public LayerMask gemLayer;
    public LayerMask baseLayer;
    public LayerMask enemyLayer;

    public Gem TargetGem { get; set; } = null;
    public Color Color => color;

    private int id;
    private Color color;

    private int gemsCapasity = 10;
    private int gemsAmount;
    private float searchRadius = 5.0f;
    private float searchTimeRatio = 2.0f;

    private TaskScheduler taskScheduler = null;

    private FsmStateMachine<Miner> fsm = null;
    private MinerIdleState idleState = null;
    private MinerMoveToGemState moveToGemState = null;
    private MinerMiningState miningState = null;
    private MinerMoveToBaseState moveToBaseState = null;
    private MinerDepositGemState depositGemState = null;
    private MinerFleeState fleeState = null;

    protected override void OnAwaken()
    {
        idleState = new MinerIdleState();
        moveToGemState = new MinerMoveToGemState();
        miningState = new MinerMiningState();
        moveToBaseState = new MinerMoveToBaseState();
        depositGemState = new MinerDepositGemState();
        fleeState = new MinerFleeState();

        idleState.Initialize(this);
        moveToGemState.Initialize(this);
        miningState.Initialize(this);
        moveToBaseState.Initialize(this);
        depositGemState.Initialize(this);
        fleeState.Initialize(this);

        id = generation++;
        color = Random.ColorHSV();
        gemsAmount = 0;
        taskScheduler = new TaskScheduler();
    }

    protected override void OnStart()
    {
        fsm = new FsmStateMachine<Miner>(
            new FsmState<Miner>[] { idleState, moveToGemState, miningState, moveToBaseState, depositGemState, fleeState },
            new UnityEvent[] { onGemFound, onGemReach, onGemCollected, onBaseReach, onGemDeposited, onEnemyAttack, onEscapeFromEnemy },
            idleState);

        fsm.ConfigureTransition(idleState, moveToGemState, onGemFound);
        fsm.ConfigureTransition(moveToGemState, miningState, onGemReach);
        fsm.ConfigureTransition(miningState, moveToBaseState, onGemCollected);
        fsm.ConfigureTransition(moveToBaseState, depositGemState, onBaseReach);
        fsm.ConfigureTransition(depositGemState, idleState, onGemDeposited);

        fsm.ConfigureTransition(idleState, fleeState, onEnemyAttack);
        fsm.ConfigureTransition(moveToGemState, fleeState, onEnemyAttack);
        fsm.ConfigureTransition(miningState, fleeState, onEnemyAttack);
        fsm.ConfigureTransition(moveToBaseState, fleeState, onEnemyAttack);
        fsm.ConfigureTransition(depositGemState, fleeState, onEnemyAttack);

        fsm.ConfigureTransition(fleeState, idleState, onEscapeFromEnemy);

        EventBus.Instance.Raise<MinerSpawnEvent>(id, color);

        taskScheduler.Schedule(SearchForCloseEnemies, searchTimeRatio);
    }

    protected override void OnShutdown()
    {
    }

    protected override void OnUpdate()
    {
        taskScheduler.Tick(Time.deltaTime);
        fsm.Update(Time.deltaTime);
    }

    public void AddGems(int value)
    {
        gemsAmount = Mathf.Min(gemsAmount + value, gemsCapasity);
        EventBus.Instance.Raise<MinerModifyGemsEvent>(id, gemsAmount);
    }

    public int GetGems(int value)
    {
        int result = value;
        if (gemsAmount < value)
        {
            result = gemsAmount;
        }
        gemsAmount -= result;
        EventBus.Instance.Raise<MinerModifyGemsEvent>(id, gemsAmount);
        return result;
    }

    public bool IsFull()
    {
        return gemsAmount == gemsCapasity;
    }

    public void SearchForCloseEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, enemyLayer);
        if (colliders.Length > 0)
        {
            onEnemyAttack?.Invoke();
        }
        taskScheduler.Schedule(SearchForCloseEnemies, searchTimeRatio);
    }
}
