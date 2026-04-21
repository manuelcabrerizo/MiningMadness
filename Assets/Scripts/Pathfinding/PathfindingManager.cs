using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviourSingleton<PathfindingManager>
{

    [SerializeField] private PathGenerator pathGenerator = new PathGenerator();
    private PathfindingStrategy strategy;
    private List<PathNode> pathNodes;
    private List<PathNode> openNodes;
    private List<PathNode> closeNodes;

    private void Awake()
    {
        strategy = new AStart();
        if (pathNodes == null)
        {
            GeneratePath();
        }
        openNodes = new List<PathNode>();
        closeNodes = new List<PathNode>();

        EventBus.Instance.Subscribe<ChangePathfindingStrategyEvent>(OnPathfindingStrategyChange);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<ChangePathfindingStrategyEvent>(OnPathfindingStrategyChange);
    }

    private void OnDrawGizmos()
    {
        if (pathNodes == null)
        {
            return;
        }
        foreach (PathNode node in pathNodes)
        {
            foreach (PathNode adjacentNode in node.AdjacentNodes)
            {
                Gizmos.DrawLine(node.Position, adjacentNode.Position);
            }
        }
    }

    private void OnPathfindingStrategyChange(in ChangePathfindingStrategyEvent changePathfindingStrategyEvent)
    {
        switch(changePathfindingStrategyEvent.StrategyType)
        {
            case PathfindingStrategyType.DepthFirst:
                strategy = new DepthFirst(); 
                break;
            case PathfindingStrategyType.BreadthFirst:
                strategy = new BreadthFirst();
                break;
            case PathfindingStrategyType.Dijkstra:
                strategy = new Dijkstra();
                break;
            case PathfindingStrategyType.AStart:
                strategy = new AStart();
                break;
        }
    }

    [ContextMenu("Generate Path")]
    private void GeneratePath()
    {
        pathNodes = pathGenerator.GenerateNodes();
    }

    private PathNode FindClosestNode(Vector3 position)
    {
        PathNode closest = null;
        float closestSqrDistance = float.MaxValue;
        foreach (PathNode node in pathNodes)
        {
            float sqrDistance = (node.Position - position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closest = node;
            }
        }
        return closest;
    }

    private PathNode GetNextOpenNode(PathNode destinationNode)
    {
        if (openNodes.Count == 0)
        {
            return null;
        }
        PathNode openNode = strategy.GetNextNode(openNodes, destinationNode);
        return openNode;
    }

    private void OpenNode(PathNode node)
    {
        if (openNodes.Contains(node))
        {
            return;
        }
        node.CurrentState = NodeState.Open;
        openNodes.Add(node);
    }

    private void CloseNode(PathNode node)
    {
        if (!openNodes.Contains(node))
        {
            return;
        }
        node.CurrentState = NodeState.Closed;
        openNodes.Remove(node);
        closeNodes.Add(node);
    }

    private void OpenAdjacentNodes(PathNode parentNode)
    {
        foreach (PathNode pathNode in parentNode.AdjacentNodes)
        {
            if (pathNode.CurrentState != NodeState.Pending)
            {
                continue;
            }
            pathNode.Parent = parentNode;
            strategy.UpdatePathNode(pathNode, parentNode);
            OpenNode(pathNode);
        }
    }

    private void ResetNodes()
    {
        foreach (PathNode pathNode in pathNodes)
        {
            pathNode.CurrentState = NodeState.Pending;
            pathNode.Parent = null;
            pathNode.AccumulatedCost = 0;
        }
        openNodes.Clear();
        closeNodes.Clear();
    }

    private Stack<PathNode> GeneratePath(PathNode destinationNode)
    {
        Stack<PathNode> path = new Stack<PathNode>();
        PathNode currentNode = destinationNode;
        while (currentNode != null)
        {
            path.Push(currentNode);
            currentNode = currentNode.Parent;
        }
        return path;
    }

    public Stack<PathNode> CreatePath(Vector3 origin, Vector3 destination)
    {
        Stack<PathNode> path = null;

        PathNode originNode = FindClosestNode(origin);
        PathNode destinationNode = FindClosestNode(destination);

        OpenNode(originNode);

        while (openNodes.Count > 0 && path == null)
        {
            PathNode openNode = GetNextOpenNode(destinationNode);
            if (openNode == destinationNode)
            {
                path = GeneratePath(destinationNode);
            }
            else 
            {
                OpenAdjacentNodes(openNode);
            }
            CloseNode(openNode);
        }

        ResetNodes();

        return path;
    }
}
