using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class MicLightEffect : MonoBehaviour
{
    private uint NUMBEROFPIXELS = 6;

    [Header("The main player object or object which will trigger lights.")]
    [SerializeField] private GameObject MainCharacter;
    [Header("The Group of the Cilia you want to activate.")]
    [SerializeField] private SurroundPosition CiliaGroup = (SurroundPosition)0;
    [Header("Base Color.")]
    [SerializeField] private byte Red = 0;
    [SerializeField] private byte Green = 0;
    [SerializeField] private byte Blue = 0;
    [Header("Use default Microphone.")]
    [SerializeField] private bool UseMic0 = false;
    [SerializeField] private string MicChoice = "";

    private float mSpeedDivider = 0.5f;

    private float mTimer = 0.0f;

    AudioSource mAudioSource;
    bool MicFailed = false;
    /**
     * Sets up microphone
     */
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        string myMicrophone = "";
        if (UseMic0)
            myMicrophone = Microphone.devices[0];
        else
            myMicrophone = MicChoice;
        //foreach(string mic in Microphone.devices)
        //{
        //    Debug.Log("mic:" + mic);
        //}
        try
        {
            mAudioSource.clip = Microphone.Start(myMicrophone, true, 1, 44100);
        }
        catch
        {
            MicFailed = true;
        }
    }

    /**
     * While the player is collided generate and play breath effect.
     * @param collision object that triggers effect.
     */
    void OnTriggerStay(Collider collision)
    {
        if (!MicFailed)
        {
            float[] data = new float[44100];
            mAudioSource.clip.GetData(data, 0);
            float intensity = 0;
            for (int i = 0; i < 44100; i++)
                data[i] = Mathf.Abs(data[i]);
            intensity = Mathf.Max(data);
            //intensity = Mathf.Abs(Mathf.Max(data));
            Debug.Log("intensity" + intensity);
            // mAudioSource.volume;
            if (MainCharacter.Equals(collision.gameObject))
            {
                mTimer += Time.deltaTime;
                if (mTimer > mSpeedDivider)
                {
                    mTimer = 0.0f;
                    for (byte LightNumber = 1; LightNumber < 7; LightNumber++)
                    {
                        byte redValue = (byte)((float)Red * intensity);
                        byte greenValue = (byte)((float)Green * intensity);
                        byte blueValue = (byte)((float)Blue * intensity);
                        Cilia.setLight(CiliaGroup, LightNumber, redValue, greenValue, blueValue);
                    }
                }
            }
        }
    }

    /**
     * Used to modify SmellsList file to have a new enumerated type with the smells for a game.
     */
    public void AddMicList()
    {
        string myString = "public class MicList { public static string[] MicStringsList = { ";
        if (Microphone.devices.Length >= 1)
        {
            myString += "\"" + Microphone.devices[0] + "\"";
            for (int i = 1; i < Microphone.devices.Length; i++)
            {
                myString += ", \"" + Microphone.devices[i] + "\"";
            }
            myString += "}; }";
            string[] mystrings = { myString, "" };
            File.WriteAllLines("Assets/CiliaPlugin/Scripts/MicList.cs", mystrings);
        }
    }
    public void SetMicChoice(string aMicChoice)
    {
        MicChoice = aMicChoice;
    }
    public string GetMicChoice()
    {
        return MicChoice;
    }
}
