using System.Collections;
using UnityEngine;

public class IcebergBehavior : MonoBehaviour
{
    public float meltDelay = 3f; // Delay before the iceberg starts melting
    public float meltDuration = 3f; // Duration over which the iceberg melts completely
    private bool hasMelted = false; // Prevents the iceberg from melting multiple times

    private void OnCollisionEnter(Collision collision)
    {
        // Start melting if the player steps on the iceberg and it hasn't already melted
        if (collision.gameObject.CompareTag("Player") && !hasMelted)
        {
            Debug.Log("Player stepped on the iceberg. Melting process will start.");
            StartCoroutine(MeltIceberg()); // Start the melting process
        }
    }

    private IEnumerator MeltIceberg()
    {
        // Wait for the specified delay before starting the melting
        yield return new WaitForSeconds(meltDelay);

        // Ensure the iceberg hasn't already melted
        if (!hasMelted)
        {
            Debug.Log("Iceberg is melting gradually!");
            hasMelted = true; // Mark as melted to prevent re-melting

            Vector3 originalScale = transform.localScale; // Store the original scale of the iceberg
            float elapsedTime = 0f; // Timer for melting

            while (elapsedTime < meltDuration)
            {
                // Calculate the new scale based on the elapsed time
                float progress = elapsedTime / meltDuration;
                float shrinkFactor = 1f - progress; // Shrinks from 100% to 0%
                transform.localScale = new Vector3(
                    originalScale.x * shrinkFactor,
                    originalScale.y,
                    originalScale.z * shrinkFactor
                );

                elapsedTime += Time.deltaTime; // Increment elapsed time
                yield return null; // Wait for the next frame
            }

            // Ensure the iceberg fully disappears at the end of the melting duration
            transform.localScale = Vector3.zero;

            // Disable interactions and optionally destroy the iceberg
            GetComponent<Collider>().enabled = false; // Disable the collider
            Destroy(gameObject, 2f); // Destroy the iceberg after a short delay
        }
    }
}
