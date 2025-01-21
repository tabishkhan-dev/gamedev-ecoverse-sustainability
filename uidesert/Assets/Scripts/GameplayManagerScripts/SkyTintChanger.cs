using UnityEngine;

public class SkyTintChanger : MonoBehaviour
{
    public Material skyboxMaterial; // Assign your skybox material here
    public float baseSpeed = 0.05f; // Base speed for hue changes
    public float randomVariance = 0.02f; // Variance for random adjustments

    private float hue = 0f; // Current hue value (0 to 1)

    void Update()
    {
        if (skyboxMaterial != null)
        {
            // Add a small base increment and a random variance for finer transitions
            float hueIncrement = baseSpeed + Random.Range(-randomVariance, randomVariance);
            hue += hueIncrement * Time.deltaTime;

            // Wrap hue if it exceeds 1 (to stay in the [0, 1] range)
            if (hue > 1f) hue -= 1f;
            if (hue < 0f) hue += 1f;


            // Convert hue to a Color using HSV to RGB
            Color dynamicColor = Color.HSVToRGB(hue, 1f, 1f);

            // Set the Tint Color property of the Skybox material
            skyboxMaterial.SetColor("_Tint", dynamicColor);
        }
    }
}