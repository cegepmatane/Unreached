using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    public Transform destination;
    public float speed;

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial position of the platform
        initialPosition = transform.position;

        // Set the target position to the destination position
        targetPosition = destination.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the platform towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // If the platform reaches the target position, swap the initial position and destination
        if (transform.position == targetPosition)
        {
            Vector3 temp = initialPosition;
            initialPosition = destination.position;
            destination.position = temp;

            // Update the target position
            targetPosition = destination.position;
        }
    }
}