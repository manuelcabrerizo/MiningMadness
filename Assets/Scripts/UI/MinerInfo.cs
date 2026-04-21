using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinerInfo : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    public void SetColor(Color color)
    {
        image.color = color;
    }
    public void SetText(string value)
    {
        text.text = value;
    }
}
