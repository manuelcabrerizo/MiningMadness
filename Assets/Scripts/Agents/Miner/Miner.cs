using UnityEngine;
using UnityEngine.Events;

public class Miner : Agent
{
    private static int generation = 0;

    public UnityEvent onGemFound;
    public UnityEvent onGemReach;
    public UnityEvent onGemCollected;
    public UnityEvent onBaseReach;
    public UnityEvent onGemDeposited;

    public LayerMask gemLayer;
    public LayerMask baseLayer;

    public Gem TargetGem { get; set; } = null;
    public Color Color => color;

    private int id;
    private Color color;

    private int gemsCapasity = 10;
    private int gemsAmount;

    private FsmStateMachine<Miner> fsm = null;
    private MinerIdleState idleState = null;
    private MinerMoveToGemState moveToGemState = null;
    private MinerMiningState miningState = null;
    private MinerMoveToBaseState moveToBaseState = null;
    private MinerDepositGoldState depositGoldState = null;

    protected override void OnAwaken()
    {
        idleState = new MinerIdleState();
        moveToGemState = new MinerMoveToGemState();
        miningState = new MinerMiningState();
        moveToBaseState = new MinerMoveToBaseState();
        depositGoldState = new MinerDepositGoldState();

        idleState.Initialize(this);
        moveToGemState.Initialize(this);
        miningState.Initialize(this);
        moveToBaseState.Initialize(this);
        depositGoldState.Initialize(this);

        fsm = new FsmStateMachine<Miner>(
            new FsmState<Miner>[] { idleState, moveToGemState, miningState, moveToBaseState, depositGoldState },
            new UnityEvent[] { onGemFound, onGemReach, onGemCollected, onBaseReach,  onGemDeposited },
            idleState);

        fsm.ConfigureTransition(idleState, moveToGemState, onGemFound);
        fsm.ConfigureTransition(moveToGemState, miningState, onGemReach);
        fsm.ConfigureTransition(miningState, moveToBaseState, onGemCollected);
        fsm.ConfigureTransition(moveToBaseState, depositGoldState, onBaseReach);
        fsm.ConfigureTransition(depositGoldState, idleState, onGemDeposited);

        id = generation++;
        color = Random.ColorHSV();
        gemsAmount = 0;
    }

    protected override void OnStart()
    {
        EventBus.Instance.Raise<MinerSpawnEvent>(id, color);
    }

    protected override void OnShutdown()
    {
    }

    protected override void OnUpdate()
    {
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
}
