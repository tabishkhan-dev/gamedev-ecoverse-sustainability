using UnityEngine;

public class AutoScrollText : MonoBehaviour
{
    public RectTransform content;  // Drag the Content GameObject here
    public float scrollSpeed = 50f; // Adjust the speed of scrolling
    private bool isScrolling = false;

    private void Update()
    {
        if (isScrolling)
        {
            // Move the content upwards
            content.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // Stop scrolling when the content reaches the top
            if (content.anchoredPosition.y >= content.sizeDelta.y)
            {
                isScrolling = false; // Stop scrolling
            }
        }
    }

    // Public function to start scrolling
    public void StartScrolling()
    {
        isScrolling = true;
    }
}

