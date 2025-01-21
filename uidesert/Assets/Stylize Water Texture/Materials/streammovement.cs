using UnityEngine;

public class WaterFlow : MonoBehaviour
{
    public Material waterMaterial; // Assign your water material here
    public Vector2 scrollSpeed = new Vector2(0.1f, 0.1f); // Adjust speed (X, Y)

    void Update()
    {
        if (waterMaterial != null)
        {
            Vector2 offset = scrollSpeed * Time.time;
            waterMaterial.mainTextureOffset = offset;
        }
    }
}

