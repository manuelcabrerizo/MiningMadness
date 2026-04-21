using System.Collections.Generic;

public class ChangePathfindingStrategyEvent : Event
{
    public PathfindingStrategyType StrategyType;
    public override void Initialize(params object[] parameters)
    {
        StrategyType = (PathfindingStrategyType)parameters[0];
    }
}
