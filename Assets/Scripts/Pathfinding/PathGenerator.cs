using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathGenerator
{
    [Serializable]
    class TerrainLayer
    {
        public string Tag;
        public float CostMultiplier;
    }

    [SerializeField] private Transform lowerLeftLimit;
    [SerializeField] private Transform upperRightLimit;
    [SerializeField, Range(0.1f, 10.0f)] private float nodeSeparation;
    [SerializeField] private TerrainLayer[] terrainLayers;
    [SerializeField] private LayerMask ignoreLayers;

    public List<PathNode> GenerateNodes()
    {
        List<PathNode> nodes = new List<PathNode>();
        float startX = lowerLeftLimit.position.x;
        float endX = upperRightLimit.position.x;
        float startZ = lowerLeftLimit.position.z;
        float endZ = upperRightLimit.position.z;
        const float rayHeight = 100.0f;
        for (float x = startX; x <= endX; x += nodeSeparation)
        {
            for (float z = startZ; z <= endZ; z += nodeSeparation)
            {
                RaycastHit hitInfo;
                Vector3 rayOrigin = new Vector3(x, rayHeight, z);
                if (Physics.Raycast(rayOrigin, Vector3.down, out hitInfo, rayHeight * 2, ~ignoreLayers))
                {
                    foreach (TerrainLayer terrainLayer in terrainLayers)
                    {
                        if (hitInfo.collider.CompareTag(terrainLayer.Tag))
                        {
                            nodes.Add(GenerateNewNode(hitInfo, terrainLayer.CostMultiplier));
                            break;
                        }
                    }
                }
            }
        }
        float separationSqr = nodeSeparation * nodeSeparation;
        float maxSqrDistance = (separationSqr + separationSqr) + 0.01f;
        foreach (PathNode node in nodes)
        {
            node.AdjacentNodes = nodes.FindAll(n => AreNodeAdjacent(node, n, maxSqrDistance));
        }
        return nodes;
    }

    private bool AreNodeAdjacent(PathNode first, PathNode second, float maxSqrDistance)
    { 
        Vector3 diff = first.Position - second.Position;
        return first != second && diff.sqrMagnitude <= maxSqrDistance;
    }

    private PathNode GenerateNewNode(RaycastHit hitInfo, float costMultiplier)
    {
        return new PathNode { Position = hitInfo.point, CostMultiplier = costMultiplier };
    }
}
