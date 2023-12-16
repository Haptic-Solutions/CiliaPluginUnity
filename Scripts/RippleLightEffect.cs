using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleLightEffect : MonoBehaviour
{
    private uint NUMBEROFPIXELS = 6;

    [Header("The main player object or object which will trigger lights.")]
    [SerializeField] private GameObject MainCharacter;
    [Header("The Group of the Cilia you want to activate.")]
    [SerializeField] private SurroundPosition CiliaGroup = (SurroundPosition)0;
    [Header("How much time has to pass before the colors will change.")]
    [SerializeField] private float SpeedDivider = 1;
    [Header("How many frames it takes for the effect to fade out.")]
    [SerializeField] private uint RippleFrames = 50;
    [Header("Base Color.")]
    [SerializeField] private byte Red = 0;
    [SerializeField] private byte Green = 0;
    [SerializeField] private byte Blue = 0;
    [Header("Ripple Color.")]
    [SerializeField] private byte RedRipple = 0;
    [SerializeField] private byte GreenRipple;
    [SerializeField] private byte BlueRipple;

    private float mTimer = 0.0f;
    private int mRippleProgression = 0;

    private int mLeftItterator = 1;
    private int mRightItterator = 1;

    private int mLeftDirection = 1;
    private int mRightDirection = 1;

    float mFallOff = 0.00f;
    
    float[][] mRippleFrames;
    
    bool mRippleInProgress = false;
    
    int mRippleSeed = 0;
    
    
    /**
     * Initializes rippleFrames array for holding frames of ripple effect.
     */
    void Start()
    {
        mRippleFrames = new float[RippleFrames][];
        for (int i = 0; i < mRippleFrames.Length; i++)
        {
            mRippleFrames[i] = new float[NUMBEROFPIXELS];
            for (int j = 0; j < mRippleFrames[i].Length; j++)
            {
                mRippleFrames[i][j] = 0.0f;
            }
        }
    }
    /**
     * Checks to make sure aValue is between 0 and 5
     * @param aValue to check
     * @return bool value of if the value is in the range.
     */
    bool CheckValue(int aValue)
    {
        if ((aValue < NUMBEROFPIXELS) && (aValue > -1))
            return true;
        else
            return false;
    }
    /**
     * Generates the ripple effect to be played by the Ripple function.
     */
    void GenerateRipple()
    {
        for (int cleari = 0; cleari < RippleFrames; cleari++)
            for (int clearj = 0; clearj < NUMBEROFPIXELS; clearj++)
                mRippleFrames[cleari][clearj] = 0.0f;
        mRippleSeed = Random.Range(0, 5);  // get seed 0-5
        mRippleFrames[0][mRippleSeed] = 1.0f; //set first frame to just have that one pixel darker

        for (int j = 2; j > -1; j--)
        {
            if (CheckValue(mRippleSeed + j))
                mRippleFrames[0][mRippleSeed + j] = (float)(3 - j) * 0.33f;
            if (CheckValue(mRippleSeed - j))
                mRippleFrames[0][mRippleSeed - j] = (float)(3 - j) * 0.33f;
        }

        for (int i = 1; i < RippleFrames; i++)
        {
            mRightItterator += mRightDirection;
            mLeftItterator += mLeftDirection;
            if(((mRippleSeed + mRightItterator) > 4) || ((mRippleSeed + mRightItterator) == (mRippleSeed - mLeftItterator)))
            {
                mRightDirection *= -1;
            }
            if (((mRippleSeed - mLeftItterator) < 1) || ((mRippleSeed - mLeftItterator) == (mRippleSeed + mRightItterator)))
            {
                mLeftDirection *= -1;
            }

            for (int j = 2; j > -1; j--)
            {
                float fallOff = ((float)RippleFrames - i) / (float)RippleFrames; //color fades depending on number of frames
                if (CheckValue(mRippleSeed + mRightItterator + j))
                    mRippleFrames[i][mRippleSeed + mRightItterator + j] = (float)(3 - j) * 0.33f * fallOff;
                if (CheckValue(mRippleSeed + mRightItterator - j))
                    mRippleFrames[i][mRippleSeed + mRightItterator - j] = (float)(3 - j) * 0.33f * fallOff;
                if (CheckValue(mRippleSeed - mLeftItterator + j))
                    mRippleFrames[i][mRippleSeed - mLeftItterator + j] = (float)(3 - j) * 0.33f * fallOff;
                if (CheckValue(mRippleSeed - mLeftItterator - j))
                    mRippleFrames[i][mRippleSeed - mLeftItterator - j] = (float)(3 - j) * 0.33f * fallOff;
            }
        }
        for (uint LightNumber = 1; LightNumber < 7; LightNumber++)
            Cilia.setLight(CiliaGroup, LightNumber, Red, Green, Blue);

        mRippleInProgress = true;
    }
    /**
     * Plays the ripple effect.
     */
    void Ripple()
    {
        if (mRippleProgression >= RippleFrames)
        {
            mRippleProgression = 0;
            mRippleInProgress = false;
            return;
        }
        byte LightNumber = 1;
        for (int i = 0; i < 6; i++)
        {
            byte redCombined = (byte)(((float)RedRipple * mRippleFrames[mRippleProgression][i]) + ((float)Red * (1.0f - mRippleFrames[mRippleProgression][i])));
            byte greenCombined = (byte)(((float)GreenRipple * mRippleFrames[mRippleProgression][i]) + ((float)Green * (1.0f - mRippleFrames[mRippleProgression][i])));
            byte blueCombined = (byte)(((float)BlueRipple * mRippleFrames[mRippleProgression][i]) + ((float)Blue * (1.0f - mRippleFrames[mRippleProgression][i])));
            Cilia.setLight(CiliaGroup, LightNumber++, redCombined, greenCombined, blueCombined);
        }
        mRippleProgression++;
    }

    /**
     * While the player is collided generate and play ripple effect.
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
                if (mRippleInProgress == false)
                {
                    GenerateRipple();
                }
                else
                {
                    Ripple();
                }
            }
        }
    }
}
