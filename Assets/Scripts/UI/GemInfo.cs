using TMPro;
using UnityEngine;

public class GemInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public void SetText(string value)
    {
        text.text = value;
    }
}
