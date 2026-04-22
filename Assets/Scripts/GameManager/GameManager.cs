using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int gemsAmount;
    private void Awake()
    {
        EventBus.Instance.Subscribe<MinerDepositGemsEvent>(OnMinerDepositGems);
        gemsAmount = 0;
    }
    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<MinerDepositGemsEvent>(OnMinerDepositGems);   
    }
    private void OnMinerDepositGems(in MinerDepositGemsEvent minerDepositGemsEvent)
    {
        gemsAmount += minerDepositGemsEvent.Amount;
        EventBus.Instance.Raise<GemsAmountChangeEvent>(gemsAmount);
    }
}
