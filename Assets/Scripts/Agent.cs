using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Agent : MonoBehaviour
{
    private float speed;
    private float rotationSpeed;
    private float damping;
    private float reachRadius;

    private Vector3 direction;

    private Vector3 velocity;

    private Vector3 forceAccumulator;

    private Stack<PathNode> path;

    private PathNode currentDestination;

    private void Awake()
    {
        speed = 25;
        rotationSpeed = 250;
        damping = 0.01f;
        reachRadius = 2.0f;

        direction = Vector3.left;
        velocity = Vector3.zero;
        forceAccumulator = Vector3.zero;

        path = null;
        currentDestination = null;
    }

    private void Start()
    {  
    }

    private void Update()
    {
        IntegrateMovement();

        if (path == null || path.Count == 0)
        {
            return;
        }

        currentDestination = path.Peek();
        float sqrDistanceToDestination = (currentDestination.Position - transform.position).sqrMagnitude;
        if (sqrDistanceToDestination < reachRadius)
        {
            path.Pop();
        }

        Vector3 destination = new Vector3(
            currentDestination.Position.x,
            transform.position.y,
            currentDestination.Position.z);
        MoveTo(destination);
    }

    public void SetDestination(Vector3 destination)
    {
        path = PathfindingManager.Instance.CreatePath(transform.position,
            new Vector3(destination.x, transform.position.y, destination.z));
    }

    private void MoveTo(Vector3 position)
    {
        direction = (position - transform.position).normalized;
        forceAccumulator += direction * speed;
    }

    private void IntegrateMovement()
    {
        velocity += forceAccumulator * Time.deltaTime;
        velocity *= Mathf.Pow(damping, Time.deltaTime);
        transform.position += velocity * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction, transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        forceAccumulator = Vector3.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (currentDestination == null)
        {
            return;
        }
        float lineThickness = 5f;
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawLine(transform.position, currentDestination.Position, lineThickness);
        if (path != null && path.Count > 0)
        {
            PathNode[] nodes = path.ToArray();
            UnityEditor.Handles.DrawLine(currentDestination.Position, nodes[0].Position, lineThickness);
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                UnityEditor.Handles.DrawLine(nodes[i].Position, nodes[i + 1].Position, lineThickness);
            }
        }
    }
#endif
}
