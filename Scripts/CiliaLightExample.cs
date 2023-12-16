using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CiliaLightExample : MonoBehaviour
{
    [Header("The main player object or object which will trigger lights.")]
    [SerializeField] private GameObject MainCharacter;
    [Header("The light number 1-6 you would like to illuminate.")]
    [SerializeField] private uint LightNumber = 1;
    [Header("Check this if you would like all lights to be set to the color.")]
    [SerializeField] private bool AllLights = false;
    [Header("Lights Red Green Blue values from 0-255.")]
    [SerializeField] private byte LightRed = 255;
    [SerializeField] private byte LightGreen = 255;
    [SerializeField] private byte LightBlue = 255;
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
            if (AllLights == true)
            {
                for(uint i = 1; i < 7; i++)
                {
                    Cilia.setLight(CiliaGroup, i, LightRed, LightGreen, LightBlue);
                }
            }
            else
                Cilia.setLight(CiliaGroup, LightNumber, LightRed, LightGreen, LightBlue);
        }
    }
}
