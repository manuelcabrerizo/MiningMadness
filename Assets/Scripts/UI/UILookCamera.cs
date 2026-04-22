using UnityEngine;
using UnityEngine.UI;

public class UILookCamera : MonoBehaviour
{
    [SerializeField] private GameObject colorUI;
    [SerializeField] private Image color;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        Miner miner = null;
        if (TryGetComponent<Miner>(out miner))
        {
            color.color = miner.Color;
        }
    }

    void LateUpdate()
    {
        colorUI.transform.position = transform.position + offset;
        colorUI.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
