using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{

	/*
     * Very basic movement script for the example player.
     * Just moves the player based on WASD and arrow keys.
     */
	void Update ()
    {
        transform.Translate((Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f), 0, (Input.GetAxis("Vertical") * Time.deltaTime * 3.0f));
    }
}
