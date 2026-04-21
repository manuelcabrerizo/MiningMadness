using UnityEngine;
using UnityEngine.UI;

public class MinerColor : MonoBehaviour
{
    [SerializeField] private GameObject colorUI;
    [SerializeField] private Image color;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        Miner miner = GetComponent<Miner>();
        color.color = miner.Color;
    }

    void LateUpdate()
    {
        colorUI.transform.position = transform.position + offset;
        colorUI.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward * -1.0f);
    }
}
