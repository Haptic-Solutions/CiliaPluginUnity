using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CiliaFanExample : MonoBehaviour
{
    [Header("The main player object or object which will trigger lights.")]
    [SerializeField] private GameObject MainCharacter;
    [Header("What fan speed should the fan be spinning at.")]
    [SerializeField] private byte FanSpeed = 255;
    [Header("Which group of Cilias you want the fans to spin in.")]
    [SerializeField] private SurroundPosition CiliaGroup = (SurroundPosition)0;
    [Header("Which smell should be turned on.")]
    [SerializeField] private SmellList Scent;
    [Header("Should the fan turn off when you leave the collision box?")]
    [SerializeField] private bool StayOn = false;
    /**
     * Turns on a smell.
     * When an object with the right MainCharacterName colides with an object that has this class and a trigger collider,
     * turns on all fans on the Cilia in the currect SurroundPosition group with the right Smell. Intensity is set to FanSpeed.
     * @param object that triggered a collision with us.
     */
    void OnTriggerEnter(Collider aCollision)
    {
        GameObject colissionObject = aCollision.gameObject;
        //Debug.Log(colissionObject.name.ToString());
        if (MainCharacter.Equals(colissionObject))
        {
            Cilia.setFan(CiliaGroup, Scent, FanSpeed);
        }
    }
    /**
     * Turns off smell.
     * When an object with the right MainCharacterName discontinues colideing with an object that has this class and a trigger collider,
     * turns off all fans on the Cilia in the currect SurroundPosition group with the right Smell. Intensity is set to FanSpeed.
     * @param object that triggered stopped colliding with us.
     */
    void OnTriggerExit(Collider aCollision)
    {
        if (StayOn == false)
        {
            GameObject colissionObject = aCollision.gameObject;
            //Debug.Log(colissionObject.name.ToString());
            if (MainCharacter.Equals(colissionObject))
            {
                Cilia.setFan(CiliaGroup, Scent, 0);
            }
        }
    }
}
