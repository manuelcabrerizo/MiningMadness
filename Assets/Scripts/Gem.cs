using UnityEngine;

public class Gem : MonoBehaviour
{
    public float Duration { get; private set; }
    public bool IsOccupy { get; private set; }

    public void Reseve()
    {
        IsOccupy = true;
    }

    public void Release()
    {
        IsOccupy = false;
    }
}
