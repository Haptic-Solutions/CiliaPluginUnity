using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableLight : MonoBehaviour
{
    [Header("The main player object or object which will trigger lights.")]
    [SerializeField] private GameObject MainCharacter;
    [Header("Which group of Cilias you want the fans to spin in.")]
    [SerializeField] private SurroundPosition CiliaGroup = (SurroundPosition)0;
    [Header("Lights Red Green Blue values from 0-255.")]
    [SerializeField] private byte Light1Red = 255;
    [SerializeField] private byte Light1Green = 255;
    [SerializeField] private byte Light1Blue = 255;

    [SerializeField] private byte Light2Red = 255;
    [SerializeField] private byte Light2Green = 255;
    [SerializeField] private byte Light2Blue = 255;

    [SerializeField] private byte Light3Red = 255;
    [SerializeField] private byte Light3Green = 255;
    [SerializeField] private byte Light3Blue = 255;

    [SerializeField] private byte Light4Red = 255;
    [SerializeField] private byte Light4Green = 255;
    [SerializeField] private byte Light4Blue = 255;

    [SerializeField] private byte Light5Red = 255;
    [SerializeField] private byte Light5Green = 255;
    [SerializeField] private byte Light5Blue = 255;

    [SerializeField] private byte Light6Red = 255;
    [SerializeField] private byte Light6Green = 255;
    [SerializeField] private byte Light6Blue = 255;
    [Header("Will you be modifying the size of the colider on this object?")]
    [SerializeField] private bool SizeIsDynamic = false;
    private float LightRadius = 5;
    private BoxCollider mBoxCollider;
    float LightIntensity = 255;
    static float oldDistance = 0;
    /**
     * Run at startup, Determines the largest dimension of the collider and divides by two for the radius of the smell.
     */
    void Start()
    {
        mBoxCollider = GetComponent<BoxCollider>();
        //the radius is the euclidean distance to the corner of the box colider from the center.
        Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
        LightRadius = Vector3.Distance(mBoxCollider.bounds.size / 2, center);
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
        if (SizeIsDynamic)
        {
            Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
            LightRadius = Vector3.Distance(mBoxCollider.bounds.size / 2, center);
        }
        GameObject colissionObject = aCollision.gameObject;
        float newDistance = Vector3.Distance(transform.position, colissionObject.transform.position);
        if (newDistance != oldDistance)
        {
            oldDistance = newDistance;
            if (MainCharacter.Equals(colissionObject))
            {
                //Debug.Log("FanRadius" + FanRadius + "Distance" + newDistance);
                LightIntensity = ((LightRadius - newDistance) / LightRadius);
                //Debug.Log("FanSpeed" + FanSpeed);
                Cilia.setLight(CiliaGroup, 1, (byte)(Light1Red * LightIntensity), (byte)(Light1Green * LightIntensity), (byte)(Light1Blue * LightIntensity));
                Cilia.setLight(CiliaGroup, 2, (byte)(Light2Red * LightIntensity), (byte)(Light2Green * LightIntensity), (byte)(Light2Blue * LightIntensity));
                Cilia.setLight(CiliaGroup, 3, (byte)(Light3Red * LightIntensity), (byte)(Light3Green * LightIntensity), (byte)(Light3Blue * LightIntensity));
                Cilia.setLight(CiliaGroup, 4, (byte)(Light4Red * LightIntensity), (byte)(Light4Green * LightIntensity), (byte)(Light4Blue * LightIntensity));
                Cilia.setLight(CiliaGroup, 5, (byte)(Light5Red * LightIntensity), (byte)(Light5Green * LightIntensity), (byte)(Light5Blue * LightIntensity));
                Cilia.setLight(CiliaGroup, 6, (byte)(Light6Red * LightIntensity), (byte)(Light6Green * LightIntensity), (byte)(Light6Blue * LightIntensity));
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
            Cilia.setLight(CiliaGroup, 1, 0, 0, 0);
            Cilia.setLight(CiliaGroup, 2, 0, 0, 0);
            Cilia.setLight(CiliaGroup, 3, 0, 0, 0);
            Cilia.setLight(CiliaGroup, 4, 0, 0, 0);
            Cilia.setLight(CiliaGroup, 5, 0, 0, 0);
            Cilia.setLight(CiliaGroup, 6, 0, 0, 0);
        }
    }
}