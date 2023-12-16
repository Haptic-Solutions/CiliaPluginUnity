using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathLightEffect : MonoBehaviour
{
    private uint NUMBEROFPIXELS = 6;

    [Header("The main player object or object which will trigger lights.")]
    [SerializeField] private GameObject MainCharacter;
    [Header("The Group of the Cilia you want to activate.")]
    [SerializeField] private SurroundPosition CiliaGroup = (SurroundPosition)0;
    [Header("How much time has to pass before the colors will change.")]
    [SerializeField] private float SpeedDivider = 1;
    [Header("Base Color.")]
    [SerializeField] private byte Red = 0;
    [SerializeField] private byte Green = 0;
    [SerializeField] private byte Blue = 0;
    [Header("Breath Color.")]
    [SerializeField] private byte Red2 = 0;
    [SerializeField] private byte Green2;
    [SerializeField] private byte Blue2;
    [Header("How long should the transition be between colors?")]
    [SerializeField] private int TransitionLength = 10;

    private float mTimer = 0.0f;

    private int mBreathCounter;

    /**
     * While the player is collided generate and play Breath effect.
     * @param collision object that triggers effect.
     */
    void OnTriggerStay(Collider collision)
    {
        if (MainCharacter.Equals(collision.gameObject))
        {
            mTimer += Time.deltaTime;
            if (mTimer > SpeedDivider)
            {
                mTimer = 0.0f;
                int breathCounterDouble;
                if (mBreathCounter < TransitionLength)
                    breathCounterDouble = mBreathCounter++;
                else
                    breathCounterDouble = TransitionLength - (mBreathCounter++ - TransitionLength);
                float transitionFloat = (float)breathCounterDouble / (float)(TransitionLength);
                byte redValue = (byte)(Red * transitionFloat + Red2 * (1 - transitionFloat));
                byte blueValue = (byte)(Blue * transitionFloat + Blue2 * (1 - transitionFloat));
                byte greenValue = (byte)(Green * transitionFloat + Green2 * (1 - transitionFloat));
                for (uint LightNumber = 1; LightNumber < 7; LightNumber++)
                {
                    Cilia.setLight(CiliaGroup, LightNumber, redValue, greenValue, blueValue);
                }
                if (mBreathCounter > (TransitionLength * 2))
                    mBreathCounter = 0;
            }
        }
    }
}
