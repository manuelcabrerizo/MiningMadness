using System;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.GridLayoutGroup;

public class Enemy : Agent
{
    public UnityEvent onLookForMiners;
    public UnityEvent onMinerFound;
    public UnityEvent onMinerNotFound;
    public UnityEvent onMinerScape;
    public UnityEvent onMinerDie;

    public LayerMask minerLayer;

    public Miner TargetMiner { get; set; } = null;

    private FsmStateMachine<Enemy> fsm = null;
    private EnemyWanderingState wanderingState = null;
    private EnemySearchState searchState = null;
    private EnemyAttackState attackState = null;

    protected override void OnAwaken()
    {
        wanderingState = new EnemyWanderingState();
        searchState = new EnemySearchState();
        attackState = new EnemyAttackState();

        wanderingState.Initialize(this);
        searchState.Initialize(this);
        attackState.Initialize(this);

        EventBus.Instance.Subscribe<MinerDieEvent>(OnMinerDie);
    }

    protected override void OnStart()
    {
        fsm = new FsmStateMachine<Enemy>(
            new FsmState<Enemy>[] { wanderingState, searchState, attackState },
            new UnityEvent[] { onLookForMiners, onMinerFound, onMinerNotFound, onMinerScape, onMinerDie },
            wanderingState);

        fsm.ConfigureTransition(wanderingState, searchState, onLookForMiners);
        fsm.ConfigureTransition(searchState, wanderingState, onMinerNotFound);
        fsm.ConfigureTransition(searchState, attackState, onMinerFound);
        fsm.ConfigureTransition(attackState, wanderingState, onMinerScape);
        fsm.ConfigureTransition(attackState, wanderingState, onMinerDie);
    }

    protected override void OnShutdown()
    {
        EventBus.Instance.Unsubscribe<MinerDieEvent>(OnMinerDie);
    }

    protected override void OnUpdate()
    {
        fsm.Update(Time.deltaTime);
    }

    private void OnMinerDie(in MinerDieEvent minerEvent)
    {
        if (TargetMiner != null && minerEvent.Id == TargetMiner.Id)
        {
            TargetMiner = null;
            onMinerDie?.Invoke();
        }
    }
}
