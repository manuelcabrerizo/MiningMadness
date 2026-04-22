using UnityEngine;
using UnityEngine.UI;

public class Gem : MonoBehaviour
{
    public bool IsOccupy { get; private set; }

    [SerializeField] private Image lifebar;
    [SerializeField] private int maxAmount = 100;
    private int amount = 0;

    private void Awake()
    {
        amount = maxAmount;
    }

    public void Reseve()
    {
        IsOccupy = true;
    }

    public void Release()
    {
        IsOccupy = false;
    }

    public void Delete()
    { 
        Destroy(gameObject);
    }

    public int GetGems(int value)
    {
        int result = value;
        if (amount < value)
        {
            result = amount;
        }
        amount -= result;
        lifebar.fillAmount = (float)amount / (float)maxAmount;
        return result;
    }

    public bool IsEmpty()
    {
        return amount == 0;
    }
}