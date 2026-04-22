using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject minerInfoPrefab;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private GameObject infoBar;
    [SerializeField] private GemInfo gemInfo;
    private Dictionary<int, MinerInfo> minerInfos;

    private void Awake()
    {
        minerInfos = new Dictionary<int, MinerInfo>();
        dropdown.onValueChanged.AddListener(OnDropDownValueChange);
        EventBus.Instance.Subscribe<MinerSpawnEvent>(OnMinerSpawn);
        EventBus.Instance.Subscribe<MinerModifyGemsEvent>(OnMinerModifyGems);
        EventBus.Instance.Subscribe<GemsAmountChangeEvent>(OnGemsAmountChange);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<GemsAmountChangeEvent>(OnGemsAmountChange);
        EventBus.Instance.Unsubscribe<MinerModifyGemsEvent>(OnMinerModifyGems);
        EventBus.Instance.Unsubscribe<MinerSpawnEvent>(OnMinerSpawn);
        dropdown.onValueChanged.RemoveListener(OnDropDownValueChange);
    }

    private void OnDropDownValueChange(int value)
    {
        if (value >= (int)PathfindingStrategyType.Count)
        {
            return;
        }
        PathfindingStrategyType strategyType = (PathfindingStrategyType)value;
        EventBus.Instance.Raise<ChangePathfindingStrategyEvent>(strategyType);
    }

    private void OnMinerSpawn(in MinerSpawnEvent minerSpawnEvent)
    {
        GameObject go = Instantiate(minerInfoPrefab, infoBar.transform);
        MinerInfo minerInfo = go.GetComponent<MinerInfo>();
        if (minerInfo != null)
        {
            minerInfo.SetColor(minerSpawnEvent.Color);
            minerInfos.Add(minerSpawnEvent.Id, minerInfo);
        }
    }

    private void OnMinerModifyGems(in MinerModifyGemsEvent minerCollectedGems)
    {
        minerInfos[minerCollectedGems.Id].SetText(minerCollectedGems.Amount.ToString());
    }

    private void OnGemsAmountChange(in GemsAmountChangeEvent gemsAmountChangeEvent)
    {
        gemInfo.SetText(gemsAmountChangeEvent.Amount.ToString());
    }
}
