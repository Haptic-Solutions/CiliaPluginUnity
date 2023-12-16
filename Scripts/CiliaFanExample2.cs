using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CiliaFanExample2 : MonoBehaviour
{
    [Header("The main player object or object which will trigger lights.")]
    [SerializeField] private GameObject MainCharacter;
    [Header("Which group of Cilias you want the fans to spin in.")]
    [SerializeField] private SurroundPosition CiliaGroup = (SurroundPosition)0;
    [Header("Which smell should be turned on.")]
    [SerializeField] private SmellList Scent;
    [SerializeField] private bool UseFanNumber = false;
    [SerializeField] private int FanNumber = 1;
    [Header("Will you be modifying the size of the colider on this object?")]
    [SerializeField] private bool SizeIsDynamic = false;
    private float FanRadius = 5;
    private BoxCollider mBoxCollider;
    byte FanSpeed = 255;
    byte OldFanSpeed = 255;
    static float oldDistance = 0;
    /**
     * Run at startup, Determines the largest dimension of the collider and divides by two for the radius of the smell.
     */
    void Start()
    {
        mBoxCollider = GetComponent<BoxCollider>();
        //the radius is the euclidean distance to the corner of the box colider from the center.
        Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
        FanRadius = Vector3.Distance( mBoxCollider.bounds.size /2,center);
        //Debug.Log(mBoxCollider.bounds.size);
    }
    /**
     * Turns on a smell to an intensity based on how close the player is to the center of the collider.
     * While an object with the right MainCharacterName colides with an object that has this class and a trigger collider,
     * turns on all fans on the Cilia in the currect SurroundPosition group with the right Smell. Intensity is determined by
     * how close the character is to the middle of the collider.
     * @param object that triggered a collision with us.
     */
    void OnTriggerStay(Collider aCollision)
    {
        if(SizeIsDynamic)
        {
            Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
            FanRadius = Vector3.Distance(mBoxCollider.bounds.size / 2, center);
        }
        GameObject colissionObject = aCollision.gameObject;
        float newDistance = Vector3.Distance(transform.position, colissionObject.transform.position);
        if (newDistance != oldDistance)
        {
            oldDistance = newDistance;
            if (MainCharacter.Equals(colissionObject))
            {
                //Debug.Log("FanRadius" + FanRadius + "Distance" + newDistance);
                FanSpeed = (byte)((FanRadius - newDistance) / FanRadius * 255);
                if (FanSpeed != OldFanSpeed)
                {
                    //Debug.Log("FanSpeed" + FanSpeed);
                    if (UseFanNumber)
                    {
                        Cilia.setFan(CiliaGroup, FanNumber, FanSpeed);
                    }
                    else
                    {
                        Cilia.setFan(CiliaGroup, Scent, FanSpeed);
                    }
                }
                OldFanSpeed = FanSpeed;
            }
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
        GameObject colissionObject = aCollision.gameObject;
        //Debug.Log(colissionObject.name.ToString());
        if (MainCharacter.Equals(colissionObject))
        {
            if (UseFanNumber)
            {
                Cilia.setFan(CiliaGroup, FanNumber, 0);
            }
            else
            {
                Cilia.setFan(CiliaGroup, Scent, 0);
            }
        }
    }
}