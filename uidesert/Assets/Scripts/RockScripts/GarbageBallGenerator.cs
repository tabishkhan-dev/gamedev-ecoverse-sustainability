using UnityEngine;

public class GarbageBallGenerator : MonoBehaviour
{
    public GameObject[] garbagePrefabs; // Array of garbage objects
    public int garbageCount = 100; // Number of garbage items
    public float sphereRadius = 2f; // Sphere radius for placement

    void Start()
    {
        for (int i = 0; i < garbageCount; i++)
        {
            // Pick a random prefab
            GameObject garbage = Instantiate(garbagePrefabs[Random.Range(0, garbagePrefabs.Length)]);

            // Generate a random point on the sphere
            Vector3 randomDirection = Random.onUnitSphere * sphereRadius;

            // Position the garbage on the sphere
            garbage.transform.position = transform.position + randomDirection;

            // Random rotation for realism
            garbage.transform.rotation = Random.rotation;

            // Parent the garbage to the sphere
            garbage.transform.SetParent(transform);
        }
    }
}

