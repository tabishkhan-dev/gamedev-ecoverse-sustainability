using UnityEngine;
using System.Collections.Generic;

public class CrystalManager : MonoBehaviour
{
    public List<GameObject> crystals; // List to hold all the crystal GameObjects
    public float activationDistance = 20f; // Distance at which the crystals will appear
    public Transform player; // Reference to the player's Transform

    private void Start()
    {
        // Ensure all crystals are initially hidden
        foreach (var crystal in crystals)
        {
            crystal.SetActive(false); // Hide all crystals at the start
        }
    }

    private void Update()
    {
        // Loop through all crystals and check their distance from the player
        foreach (var crystal in crystals)
        {
            float distance = Vector3.Distance(player.position, crystal.transform.position);

            // If the player is within the activation distance, show the crystal
            if (distance <= activationDistance)
            {
                crystal.SetActive(true); // Make the crystal visible
            }
            else
            {
                crystal.SetActive(false); // Hide the crystal
            }
        }
    }
}
