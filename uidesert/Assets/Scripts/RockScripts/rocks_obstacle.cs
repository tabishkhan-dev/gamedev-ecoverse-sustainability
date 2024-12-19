using UnityEngine;

public class rocks_obstacle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
      void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player jumped over the obstacle!");
        }
    }
}
