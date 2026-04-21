using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Miner : Agent
{
    private static int generation = 0;

    public UnityEvent onGemFound;
    public UnityEvent onGemReach;
    public UnityEvent onGemCollected;
    public UnityEvent onGemDeposited;

    public LayerMask gemLayer;
    public LayerMask baseLayer;

    public Gem TargetGem { get; set; } = null;
    public int Id => id;
    public Color Color => color;

    private int id;
    private Color color;
    private FsmStateMachine<Miner> fsm = null;
    private MinerIdleState idleState = null;
    private MinerMoveToGemState moveToGemState = null;
    private MinerMiningState miningState = null;
    private MinerDepositGoldState depositGoldState = null;

    protected override void OnAwaken()
    {
        idleState = new MinerIdleState();
        moveToGemState = new MinerMoveToGemState();
        miningState = new MinerMiningState();
        depositGoldState = new MinerDepositGoldState();

        idleState.Initialize(this);
        moveToGemState.Initialize(this);
        miningState.Initialize(this);
        depositGoldState.Initialize(this);

        fsm = new FsmStateMachine<Miner>(
            new FsmState<Miner>[] { idleState, moveToGemState, miningState, depositGoldState },
            new UnityEvent[] { onGemFound, onGemReach, onGemCollected, onGemDeposited },
            idleState);

        fsm.ConfigureTransition(idleState, moveToGemState, onGemFound);
        fsm.ConfigureTransition(moveToGemState, miningState, onGemReach);
        fsm.ConfigureTransition(miningState, depositGoldState, onGemCollected);
        fsm.ConfigureTransition(depositGoldState, idleState, onGemDeposited);

        id = generation++;
        color = Random.ColorHSV();
    }

    protected override void OnStart()
    {
        EventBus.Instance.Raise<MinerSpawnEvent>(id, color);
    }

    protected override void OnUpdate()
    {
        fsm.Update(Time.deltaTime);
    }
}
