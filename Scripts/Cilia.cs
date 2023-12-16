/**
* This class MUST BE INCLUDED on an object in Unity 3D for the plugin to be able to communicate with the SDK.
* <pre>
* This class handles connecting and disconnecting from the SDK.
* This class allows a developer to setup networking if the SDK is on another computer.
* This class allows a developer to define a game profile for their game.
*  - defining smells
*  - defining groups of cilias
*  - defining default color scheme
* This class allows a developer to send messages to the SDK controlling fan speed and light colors.
* </pre>
* @author Peter Sassaman
* @version 0.2.7
* MIT License
* Copyright (c) 2019 Haptic Solutions Incorporated
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.IO;
using System.Net.WebSockets;
using System.Net;
using System.Threading;
using Assets.CiliaPlugin.Scripts;
using Newtonsoft.Json;
using System.Threading.Tasks;

[System.Serializable]
public struct Neopixel
{
    public byte redValue;
    public byte greenValue;
    public byte blueValue;
};
public class Cilia : MonoBehaviour
{
    [Header("If You Use Fan Numbers Instead Of Scents Uncheck")]
    [SerializeField] private bool CreateProfile = true;
    [Header("------------Networking Section------------")]
    [Header("Most of the time leave this alone")]
    [SerializeField] private int CiliaPort = 1995;
    [SerializeField] private string CiliaIP = "localhost";
    [Header("-----------Game Profile Section-----------")]
    [SerializeField] private string GameProfileName = "Game";
    [SerializeField] private SurroundPosition DefaultSurroundGroup = (SurroundPosition) 0;
    /*Smells to add to sdk smell library. Also for setting default game profile. First six will be used*/
    [Header("Adds the following smells to the smell library")]
    [Header("Sets the Cilias to the 1st six smells if the profile didn't exist")]
    [SerializeField] private string[] SmellsToAddToSmellLibrary = { "Apple", "BahamaBreeze", "CleanCotton", "Leather", "Lemon", "Rose" };
    private static string[] privateLibrary = { "Apple", "BahamaBreeze", "CleanCotton", "Leather", "Lemon", "Rose" };
    [Header("Game Profile Default Lighting Values: 0-255")]
    [SerializeField] private Neopixel Light1;
    [SerializeField] private Neopixel Light2;
    [SerializeField] private Neopixel Light3;
    [SerializeField] private Neopixel Light4;
    [SerializeField] private Neopixel Light5;
    [SerializeField] private Neopixel Light6;
    List<Neopixel> Lights;
    [SerializeField] private string[] surroundGroupStrings = { "FrontCenter", "FrontLeft", "SideLeft", "RearLeft", "RearCenter", "RearRight", "SideRight", "FrontRight" };
    private string[] privateSurroundGroups = { "FrontCenter", "FrontLeft", "SideLeft", "RearLeft", "RearCenter", "RearRight", "SideRight", "FrontRight" };
    [Header("If you placed the Cilia scripts elsewhere please change this")]
    [SerializeField] private string PathToCiliaPluginScripts = "Assets/CiliaPlugin/Scripts/";
    [Header("Force clean game profile update. Do not leave checked. Sets all Cilias back to default smells next time you press play")]
    [SerializeField] private bool forceCleanUpdate = false;
    static ClientWebSocket CiliaClient;// = new ClientWebSocket();
    static byte[] message;
    const int SmellNameLength = 20;
    static bool mIsConnected = false;
    static bool busySending;
    public static Cilia instance;
    [SerializeField]
    private bool connectAutomatically = true;

    static Mutex aSendProtector = new Mutex();

    private class CommandResult
    {
        int errorCode;
        bool success;
        string message;
    }

    public void Update()
    {
        if(mIsConnected == false)
        {
            mIsConnected = true;
            Connect();
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Destroying duplicate cilia");
            Destroy(this.gameObject);
            return;
        }
        
    }

    /**
     * Connects Plugin to the SDK, adds our games smells to the SDK's smell library, and sends our games profile to the SDK.
     */
    void Start()
    {
        Lights = new List<Neopixel> { Light1, Light2, Light3, Light4, Light5, Light6 };
        busySending = false;
        //we are already connected so don't need setup
        if (mIsConnected == true)
        {
            return;
        }
        if (connectAutomatically)
        {
            Connect();
            
        }
        
        //if this is our first time starting check if there is another Cilia if there is destroy ourselves
        
        //setFan(SurroundPosition.All, SmellList.Apple, 255);
        //setLight(SurroundPosition.All, 1, 255, 0, 0);
    }

    public async void Connect()
    {
        CiliaClient = new ClientWebSocket();
        //If we are the first Cilia Make ourselves persistent for the entire game
        //Debug.Log("Attempting connection");
        DontDestroyOnLoad(this.gameObject);
        /*Connect Networking*/
        try
        {
            String address = "ws://" + CiliaIP.ToString() + ":" + CiliaPort.ToString();
            Uri uri = new Uri(address);
            Task task = CiliaClient.ConnectAsync(uri, CancellationToken.None);
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                Debug.Log("failed to connect to Cilia SDK");
                Debug.Log(ex.ToString());
            }
            
            if (CiliaClient.State == WebSocketState.Open)
            {
                Debug.Log("Cilia SDK Connected");
                mIsConnected = true;
                SetGameProfile();
            } else
            {
                mIsConnected = false;
                Debug.Log("Could not connect. " + task.Status);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to connect to Cilia SDK");
            Debug.Log(e.StackTrace);
            mIsConnected = false;
            return;
        }
    }

    private void SetGameProfile()
    {
        if (!CreateProfile)
        {
            return;
        }
        LoadProfileJSON LoadProfile = new LoadProfileJSON();
        LoadProfile.LoadProfile.ProfileName = GameProfileName;
        //var JsonToSend = JsonUtility.ToJson(LoadProfile);
        /*Start Creating Messages for setting up library and prfiles*/
        for (int i = 0; i < SmellsToAddToSmellLibrary.Length; i++)
        {
            LoadProfile.LoadProfile.Scents.Add(new List<object> { i, SmellsToAddToSmellLibrary[i] });
        }

        for (int i = 0; i < surroundGroupStrings.Length; i++)
        {
            LoadProfile.LoadProfile.Groups.Add( surroundGroupStrings[i] );
        }

        LoadProfile.LoadProfile.Effect = new Effect();
        var effect = LoadProfile.LoadProfile.Effect;
        effect.EffectID = 0;
        effect.EffectColors = new List<List<uint>>();
        uint j = 1;
        foreach (var light in Lights)
        {
            RGB rgb = new RGB(light.redValue, light.greenValue, light.blueValue);
            effect.EffectColors.Add(new List<uint> { j, rgb.GetHex()});
            j++;
        }
        sendMessageToCilia(JsonConvert.SerializeObject(LoadProfile));
        
    }

    /**
     * Sends a message to the SDK to set a surround position's specific light nuber to a RGB color
     * @param aSurroundPosition enumerated value indicating which group of Cilias we want to change the lighting on.
     * @param aLightNumber between 1-6 of which light we want to change the color of.
     * @param aRedValue between 0-255 of how red the light will be.
     * @param aGreenValue between 0-255 of how green the light will be
     * @param aBlueValue between 0-255 of how blue the light will be
     */
    public static void setLight(SurroundPosition aSurroundPosition, uint aLightNumber, byte aRedValue, byte aGreenValue, byte aBlueValue)
    {
        SetLights setLights = new SetLights();
        setLights.SetLight = new List<List<object>>();
        setLights.SetLight.Add(new List<object> { aLightNumber, aRedValue, aGreenValue, aBlueValue });

        if (aSurroundPosition == SurroundPosition.All)
        {
            sendMessageToCilia(JsonConvert.SerializeObject(setLights));
        }
        else
        {
            GroupIDs groupIDs = new GroupIDs();
            groupIDs.GroupID.Add((byte)aSurroundPosition);
            groupIDs.Message = setLights;
            sendMessageToCilia(JsonConvert.SerializeObject(groupIDs));
        }
    }
    /**
     * Sends a message to the SDK asking it to set all the fans with a specific smell in a specific surroundPosition to a specified speed.
     * @param aSurroundPosition group of Cilias we want this to apply to.
     * @param aSmell we want the user to smell the SDK uses this to find fans with the smell.
     * @param aFanSpeed a value between 0-255 specifying how fast we want the fant to spin.
     */
    public static void setFan(SurroundPosition aSurroundPosition, SmellList aSmell, byte aFanSpeed)
    {
        
        SetScents setScents = new SetScents();
        setScents.SetScent.Add( new List<object>{ aSmell.ToString() , aFanSpeed});

        if (aSurroundPosition == SurroundPosition.All)
        {
            sendMessageToCilia(JsonConvert.SerializeObject(setScents));
        }
        else
        {
            GroupIDs groupIDs = new GroupIDs();
            groupIDs.GroupID.Add((byte)aSurroundPosition);
            groupIDs.Message = setScents;
            string serializedMessage = JsonConvert.SerializeObject(groupIDs);
            //Debug.Log(serializedMessage);
            sendMessageToCilia(serializedMessage);
        }
    }

    /// <summary>
    /// Set a fan without worrying about surround position
    /// </summary>
    /// <param name="aFan"></param>
    /// <param name="aFanSpeed"> from 0 to 255</param>
    public static void setFan(int aFan, byte aFanSpeed)
    {

        setFan(SurroundPosition.All, aFan, aFanSpeed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aSurroundPosition"></param>
    /// <param name="aFan"></param>
    /// <param name="aFanSpeed">From 0 to 255</param>
    public static void setFan(SurroundPosition aSurroundPosition, int aFan, byte aFanSpeed)
    {

        SetFans setFans = new SetFans();
        setFans.SetFan.Add(new List<object> { aFan, aFanSpeed });

        if (aSurroundPosition == SurroundPosition.All)
        {
            sendMessageToCilia(JsonConvert.SerializeObject(setFans));
        }
        else
        {
            GroupIDs groupIDs = new GroupIDs();
            groupIDs.GroupID.Add((byte)aSurroundPosition);
            groupIDs.Message = setFans;
            string serializedMessage = JsonConvert.SerializeObject(groupIDs);
            //Debug.Log(serializedMessage);
            sendMessageToCilia(serializedMessage);
        }
    }
    /**
     * Returns a string of what value a neopixel is currently set to with three deimal places for red green and blue
     * @param aNeopixel structure to retrieve the string value from
     */
    static string getLightString(Neopixel aNeopixel)
    {
        return aNeopixel.redValue.ToString("D3") + aNeopixel.greenValue.ToString("D3") + aNeopixel.blueValue.ToString("D3");
    }

    /**
     * Sends a string message to the SDK.
     * @param aMessageToSend string message to send to the SDK.
     */
    static void sendMessageToCilia(string aMessageToSend)
    {
        if (mIsConnected)
        {
            //Debug.Log(aMessageToSend);
            message = System.Text.Encoding.ASCII.GetBytes(aMessageToSend);
            ArraySegment<byte> messageBytes = new ArraySegment<byte>(message);
            //while (busySending) { }
            //busySending = true;
            aSendProtector.WaitOne();
            try
            {
                Task sentMessage = CiliaClient.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
                sentMessage.Wait();
            }
            catch(Exception e)
            {
                Cilia.Disconnect();
            }
            aSendProtector.ReleaseMutex();
            //busySending = false;
        }
    }

    public static void Disconnect()
    {
        try
        {
            if (mIsConnected)
            {
                mIsConnected = false;
                CiliaClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
        }
        catch(Exception e)
        {

        }
    }
    /**
     * Closes TCP/IP stream and client that were connected to the SDK.
     */
    void OnApplicationQuit()
    {
        Disconnect();
    }

    /* Code for validating inputs and modifying files. Please only touch if you are very comfortable with programming!!! */

    /**
     * Validates the surround groups and smells added by the user
     */
    private void OnValidate()
    {
        if (SmellsToAddToSmellLibrary.Length == privateLibrary.Length)
        {
            for (int i = 0; i < SmellsToAddToSmellLibrary.Length; i++)
            {
                if (SmellsToAddToSmellLibrary[i].Length == 0)
                {
                    SmellsToAddToSmellLibrary[i] = privateLibrary[i];

                }
                else
                {
                    string SmellsToAddCopy = "";
                    if (SmellsToAddToSmellLibrary[i].Length > SmellNameLength)
                        SmellsToAddToSmellLibrary[i] = SmellsToAddToSmellLibrary[i].Substring(0, SmellNameLength);
                    foreach (char c in SmellsToAddToSmellLibrary[i])
                    {
                        if (char.IsLetterOrDigit(c))
                            SmellsToAddCopy += c;
                    }
                    if (SmellsToAddCopy.Length != 0)
                    {
                        SmellsToAddToSmellLibrary[i] = SmellsToAddCopy;
                        privateLibrary[i] = SmellsToAddToSmellLibrary[i];
                    }
                    else
                    {
                        SmellsToAddToSmellLibrary[i] = privateLibrary[i];
                    }
                }
            }
        }
        else if (SmellsToAddToSmellLibrary.Length < 6)
        {
            SmellsToAddToSmellLibrary = privateLibrary;
        }
        else
        {
            privateLibrary = SmellsToAddToSmellLibrary;
        }

        if (surroundGroupStrings.Length == privateSurroundGroups.Length)
        {
            for (int i = 0; i < surroundGroupStrings.Length; i++)
            {
                if (surroundGroupStrings[i].Length == 0)
                {
                    surroundGroupStrings[i] = privateSurroundGroups[i];

                }
                else
                {
                    string GroupsToAddCopy = "";
                    if (surroundGroupStrings[i].Length > SmellNameLength)
                        surroundGroupStrings[i] = surroundGroupStrings[i].Substring(0, SmellNameLength);
                    foreach (char c in surroundGroupStrings[i])
                        if (char.IsLetterOrDigit(c))
                            GroupsToAddCopy += c;
                    if (GroupsToAddCopy.Length != 0)
                    {
                        surroundGroupStrings[i] = GroupsToAddCopy;
                        privateSurroundGroups[i] = surroundGroupStrings[i];
                    }
                    else
                    {
                        surroundGroupStrings[i] = privateSurroundGroups[i];
                    }
                }
            }
        }
        else if (surroundGroupStrings.Length > 256)
        {
            surroundGroupStrings = privateSurroundGroups;
        }
        else if (surroundGroupStrings.Length < 1)
        {
            surroundGroupStrings = privateSurroundGroups;
        }
        else
        {
            privateSurroundGroups = surroundGroupStrings;
        }
    }
    /**
     * Used to modify SurroundPositions file to have a new enumerated type with the surround groups for a game.
     */
    public void AddSurroundGroups()
    {
        string myString = "public enum SurroundPosition { ";
        if (surroundGroupStrings.Length >= 1)
        {
            myString += "" + surroundGroupStrings[0];
            for (int i = 1; i < surroundGroupStrings.Length; i++)
            {
                myString += ", " + surroundGroupStrings[i];
            }
            myString += ", All};";
            string[] mystrings = { myString, "" };
            File.WriteAllLines(PathToCiliaPluginScripts + "SurroundPosition.cs", mystrings);
        }
    }
    /**
     * Used to modify SmellsList file to have a new enumerated type with the smells for a game.
     */
    public void AddSmellsList()
    {
        string myString = "public enum SmellList { ";
        if (SmellsToAddToSmellLibrary.Length >= 1)
        {
            myString += "" + SmellsToAddToSmellLibrary[0];
            for (int i = 1; i < SmellsToAddToSmellLibrary.Length; i++)
            {
                myString += ", " + SmellsToAddToSmellLibrary[i];
            }
            myString += "};";
            string[] mystrings = { myString, "" };
            File.WriteAllLines(PathToCiliaPluginScripts + "SmellsList.cs", mystrings);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Destroying cilia");
    }
}
