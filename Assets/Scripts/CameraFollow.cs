using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private float m_PlayerSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 2D Screen
        // Make the camera follow the player
        // Get the player position
        Vector3 t_PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        // Set the camera position
        transform.position = new Vector3(t_PlayerPosition.x, t_PlayerPosition.y, transform.position.z);

        //The more the speed of the player is, the more the camera will be far from the player
        //float m_PlayerSpeed = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity.magnitude;
        // Set the camera size
        //GetComponent<Camera>().orthographicSize = 5 + m_PlayerSpeed / 2;

    }
}
