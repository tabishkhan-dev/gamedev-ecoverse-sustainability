using System.Collections;
using UnityEngine;

public class IcebergBehavior : MonoBehaviour
{
    public float meltDelay = 3f; // Default delay before melting
    public float meltDuration = 3f; // Default duration for melting

    private bool hasMelted = false;
    private bool isMeltingPaused = false;
    private float currentMeltDuration; // Tracks the remaining melt duration
    private Coroutine meltingCoroutine;

    void Start()
    {
        currentMeltDuration = meltDuration;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasMelted)
        {
            meltingCoroutine = StartCoroutine(MeltIceberg());
        }
    }

    private IEnumerator MeltIceberg()
    {
        yield return new WaitForSeconds(meltDelay);

        if (!hasMelted)
        {
            hasMelted = true;
            Vector3 originalScale = transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < currentMeltDuration)
            {
                if (isMeltingPaused)
                {
                    yield return null;
                    continue;
                }

                float progress = elapsedTime / meltDuration;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = Vector3.zero;
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject, 2f);
        }
    }

    public void PauseMelting(float duration)
    {
        if (meltingCoroutine != null)
        {
            isMeltingPaused = true;
            StopCoroutine(ResumeMeltingAfterDelay(duration));
            StartCoroutine(ResumeMeltingAfterDelay(duration));
        }
    }

    private IEnumerator ResumeMeltingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isMeltingPaused = false;
    }
}
