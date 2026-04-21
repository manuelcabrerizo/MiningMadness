using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    Pending,
    Open,
    Closed
}

public class PathNode
{
    public Vector3 Position { get; set; } = Vector3.zero;
    public NodeState CurrentState { get; set; } = NodeState.Pending;
    public float CostMultiplier { get; set; } = 1.0f;
    public float AccumulatedCost { get; set; } = 0.0f;
    public PathNode Parent { get; set; } = null;
    public List<PathNode> AdjacentNodes { get; set; } = new();
}