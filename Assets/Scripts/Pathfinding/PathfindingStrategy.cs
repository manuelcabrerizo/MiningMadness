using System.Collections.Generic;
using System.Linq;

public abstract class PathfindingStrategy
{
    public abstract PathNode GetNextNode(List<PathNode> nodes, PathNode targetNode);
    public virtual void UpdatePathNode(PathNode pathNode, PathNode parentNode) { }
    protected static void CalculateAccumulatedCost(PathNode pathNode, PathNode parentNode)
    {
        float sqrDistance = (parentNode.Position - pathNode.Position).sqrMagnitude;
        pathNode.AccumulatedCost = parentNode.AccumulatedCost + sqrDistance * pathNode.CostMultiplier;
    }
    protected static float EstimateCost(PathNode a, PathNode b)
    {
        return (a.Position - b.Position).sqrMagnitude;
    }
}

public class DepthFirst : PathfindingStrategy
{
    public override PathNode GetNextNode(List<PathNode> nodes, PathNode targetNode)
    {
        return nodes[^1];
    }
}

public class BreadthFirst : PathfindingStrategy
{
    public override PathNode GetNextNode(List<PathNode> nodes, PathNode targetNode)
    {
        return nodes[0];
    }
}

public class Dijkstra : PathfindingStrategy
{
    public override PathNode GetNextNode(List<PathNode> nodes, PathNode targetNode)
    {
        return nodes.OrderBy(n => n.AccumulatedCost).First();
    }
    public override void UpdatePathNode(PathNode pathNode, PathNode parentNode)
    {
        CalculateAccumulatedCost(pathNode, parentNode);
    }
}

public class AStart : PathfindingStrategy
{
    public override PathNode GetNextNode(List<PathNode> nodes, PathNode targetNode)
    {
        return nodes.OrderBy(n => n.AccumulatedCost + EstimateCost(n, targetNode)).First();
    }
    public override void UpdatePathNode(PathNode pathNode, PathNode parentNode)
    {
        CalculateAccumulatedCost(pathNode, parentNode);
    }
}