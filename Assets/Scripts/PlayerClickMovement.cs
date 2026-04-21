
using UnityEngine;

public class PlayerClickMovement : MonoBehaviour
{
    private Agent agent;
    private Camera mainCamera;

    private void Awake()
    {
        agent = GetComponent<Agent>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, maxDistance: float.MaxValue))
            {
                agent.SetDestination(raycastHit.point);
            }
        }
    }
}

