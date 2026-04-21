using UnityEngine;
using UnityEngine.Events;

public class Miner : Agent
{
    public UnityEvent onGemFound;
    public UnityEvent onGemReach;
    public UnityEvent onGemCollected;
    public UnityEvent onGemDeposited;

    public LayerMask gemLayer;
    public LayerMask baseLayer;

    public Gem TargetGem { get; set; } = null;

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
    }

    protected override void OnUpdate()
    {
        fsm.Update(Time.deltaTime);
    }
}
