using UnityEngine;

public class MinerSpawnEvent : Event
{
    public int Id;
    public Color Color;
    public override void Initialize(params object[] parameters)
    {
        Id = (int)parameters[0];
        Color = (Color)parameters[1];
    }
}