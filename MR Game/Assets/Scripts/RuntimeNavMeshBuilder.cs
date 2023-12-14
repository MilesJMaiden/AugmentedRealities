using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RuntimeNavMeshBuilder : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    void Awake()
    {
        // Assuming navMeshSurface is already assigned
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }
}