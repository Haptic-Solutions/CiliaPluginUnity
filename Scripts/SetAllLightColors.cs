using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAllLightColors : MonoBehaviour
{
    [Header("The main player object or object which will trigger lights.")]
    [SerializeField] private GameObject MainCharacter;
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
    [Header("Which group of Cilia you want to illuminate.")]
    [SerializeField] private SurroundPosition CiliaGroup = (SurroundPosition)0;
    /**
     * Changes the lights on the Cilias in a surroundPosition at a specified neopixel to a RGB color.
     * If AllLights is checked then all lights change to the specified color
     * @param aCollision object we are colliding with that is triggering this event.
     */
    void OnTriggerEnter(Collider aCollision)
    {
        if (MainCharacter.Equals(aCollision.gameObject))
        {
            Cilia.setLight(CiliaGroup, 1, Light1Red, Light1Green, Light1Blue);
            Cilia.setLight(CiliaGroup, 2, Light2Red, Light2Green, Light2Blue);
            Cilia.setLight(CiliaGroup, 3, Light3Red, Light3Green, Light3Blue);
            Cilia.setLight(CiliaGroup, 4, Light4Red, Light4Green, Light4Blue);
            Cilia.setLight(CiliaGroup, 5, Light5Red, Light5Green, Light5Blue);
            Cilia.setLight(CiliaGroup, 6, Light6Red, Light6Green, Light6Blue);
        }
    }
}
