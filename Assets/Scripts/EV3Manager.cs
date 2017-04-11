﻿// <copyright file="EV3Manager.cs" company="dyadica.co.uk">
// Copyright (c) 2010, 2016 All Right Reserved, http://www.dyadica.co.uk

// This source is subject to the dyadica.co.uk Permissive License.
// Please see the http://www.dyadica.co.uk/permissive-license file for more information.
// All other rights reserved.

// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>

// <author>SJB</author>
// <email>info@dyadica.co.uk</email>
// <date>16.01.2016</date>

using UnityEngine;
using System.Collections;

// Bespoke using declarations

using UnityEngine.UI;
using System.Collections.Generic;

public class EV3Manager : MonoBehaviour
{
    #region Properties

    public static EV3Manager Instance;
    private string scriptLocation;

    public static AndroidJavaClass UnityPlayer;
    public static AndroidJavaObject Activity;

    private AndroidJavaClass activityClass = null;
    private AndroidJavaObject activityContext = null;

    private AndroidJavaObject ev3Plugin = null;

    public string EV3Name
    {
        get { return PlayerPrefs.GetString("Name", "EV3"); }
        set { PlayerPrefs.SetString("Name", value); }
    }

    public bool DynamicScriptLocation = false;
    public bool InitialiseOnStart = false;
    public bool autoEnableBT = false;

    public bool IsConnected = false;

    public OutputPort LMotorPort;
    public OutputPort RMotorPort;
    public OutputPort SMotorPort;

    public int invert
    {
        get
        {
            if (InvertMotors)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    public bool InvertMotors
    {
        get
        {
            bool v = bool.Parse(PlayerPrefs.GetString("Invert", "False"));
            return v;
        }
        set
        {
            PlayerPrefs.SetString("Invert", value.ToString());
        }
    }

    #region Events

    public delegate void throwPortDeviceUpdateEventHandler(InputPort port, Device device);
    public static event throwPortDeviceUpdateEventHandler throwPortDeviceUpdateEvent;

    public delegate void throwPortDataUpdateEventHandler(PortState state);
    public static event throwPortDataUpdateEventHandler throwPortDataUpdateEvent;

    // Sensor events

    public delegate void InfraredSensorUpdateEventHandler(PortState state);
    public static event InfraredSensorUpdateEventHandler InfraredSensorUpdateEvent;

    public delegate void ColourSensorUpdateEventHandler(PortState state);
    public static event ColourSensorUpdateEventHandler ColourSensorUpdateEvent;

    public delegate void TouchSensorUpdateEventHandler(PortState state);
    public static event TouchSensorUpdateEventHandler TouchSensorUpdateEvent;

    public delegate void UltrasonicSensorUpdateEventHandler(PortState state);
    public static event UltrasonicSensorUpdateEventHandler UltrasonicSensorUpdateEvent;

    public delegate void GyroscopeSensorUpdateEventHandler(PortState state);
    public static event GyroscopeSensorUpdateEventHandler GyroscopeSensorUpdateEvent;

    // Bluetooth

    public delegate void BluetoothUpdateEventHandler(bool state);
    public static event BluetoothUpdateEventHandler BluetoothUpdateEvent;

    // Buttons

    public delegate void ButtonUpdateEventHandler(BrickButton button, bool state);
    public static event ButtonUpdateEventHandler ButtonUpdateEvent;

    #endregion Events

    #endregion Properties

    #region Loop

    void Awake()
    {
        Instance = this;
    }

	void Start ()
    {
        scriptLocation = gameObject.name;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_ANDROID
        activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");

        // Register events

        throwPortDeviceUpdateEvent += 
            EV3Manager_throwPortDeviceUpdateEvent;

        throwPortDataUpdateEvent += 
            EV3Manager_throwPortDataUpdateEvent;

        // Sensor Events

        InfraredSensorUpdateEvent += 
            EV3Manager_InfraredSensorUpdateEvent;

        ColourSensorUpdateEvent += 
            EV3Manager_ColourSensorUpdateEvent;

        TouchSensorUpdateEvent += 
            EV3Manager_TouchSensorUpdateEvent;

        UltrasonicSensorUpdateEvent += 
            EV3Manager_UltrasonicSensorUpdateEvent;

        GyroscopeSensorUpdateEvent += 
            EV3Manager_GyroscopeSensorUpdateEvent;

        BluetoothUpdateEvent += 
            EV3Manager_BluetoothUpdateEvent;

        // Buttons

        ButtonUpdateEvent += 
            EV3Manager_ButtonUpdateEvent;


        if (GUIManager.Instance.DebugWindow != null)
            GUIManager.Instance.DebugWindow.text = "Found unity plugin!";

        using (AndroidJavaClass pluginClass = new AndroidJavaClass("uk.co.dyadica.ev3api.EV3Manager"))
        {
            if (pluginClass != null)
            {
                if (GUIManager.Instance.DebugWindow != null)
                    GUIManager.Instance.DebugWindow.text = "Found ev3 plugin!";

                ev3Plugin = pluginClass.CallStatic<AndroidJavaObject>("ev3Manager");
                ev3Plugin.Call("setContext", activityContext);
            }
        }

        if (DynamicScriptLocation)
            ev3Plugin.CallStatic<string>("scriptLocation", scriptLocation);

        if (autoEnableBT)
            ev3Plugin.CallStatic<bool>("autoEnableBT", true);

        if(InitialiseOnStart)
            ev3Plugin.Call("initialisePlugin");
#else
        GUIManager.Instance.LoadConnectionScreen();
#endif
    }


    void OnDestroy()
    {
        #region Events

        throwPortDeviceUpdateEvent -=
             EV3Manager_throwPortDeviceUpdateEvent;

        throwPortDataUpdateEvent -=
            EV3Manager_throwPortDataUpdateEvent;

        // Sensors

        InfraredSensorUpdateEvent -=
            EV3Manager_InfraredSensorUpdateEvent;

        ColourSensorUpdateEvent -=
            EV3Manager_ColourSensorUpdateEvent;

        TouchSensorUpdateEvent -=
            EV3Manager_TouchSensorUpdateEvent;

        UltrasonicSensorUpdateEvent -=
            EV3Manager_UltrasonicSensorUpdateEvent;

        GyroscopeSensorUpdateEvent -=
            EV3Manager_GyroscopeSensorUpdateEvent;

        BluetoothUpdateEvent -=
           EV3Manager_BluetoothUpdateEvent;

        // Buttons

        ButtonUpdateEvent -=
            EV3Manager_ButtonUpdateEvent;

        #endregion Events
    }

    #endregion Loop

    #region Events

    private void EV3Manager_BluetoothUpdateEvent(bool state)
    {
        // throw new System.NotImplementedException();
    }

    private void EV3Manager_GyroscopeSensorUpdateEvent(PortState state)
    {
        // throw new System.NotImplementedException();
    }

    private void EV3Manager_UltrasonicSensorUpdateEvent(PortState state)
    {
        // throw new System.NotImplementedException();
    }

    private void EV3Manager_TouchSensorUpdateEvent(PortState state)
    {
        // throw new System.NotImplementedException();
    }

    private void EV3Manager_ColourSensorUpdateEvent(PortState state)
    {
        // throw new System.NotImplementedException();
    }

    private void EV3Manager_InfraredSensorUpdateEvent(PortState state)
    {
        // throw new System.NotImplementedException();
    }

    private void EV3Manager_throwPortDataUpdateEvent(PortState state)
    {
        // throw new System.NotImplementedException();
    }

    private void EV3Manager_throwPortDeviceUpdateEvent(InputPort port, Device device)
    {
        // throw new System.NotImplementedException();
    }

    private void EV3Manager_ButtonUpdateEvent(BrickButton button, bool state)
    {
        // throw new System.NotImplementedException();
    }

    #endregion Events

    #region Command Methods

    public void InitialisePlugin()
    {
        if (ev3Plugin == null)
            return;

        ev3Plugin.SetStatic<string>("deviceName", EV3Name);
        ev3Plugin.Call("initialisePlugin");
    }

    public void InitialisePlugin(string ev3Name)
    {
        if (ev3Plugin == null)
            return;

        EV3Name = ev3Name;
        InitialisePlugin();
    }

    public void PlayProgram(string fileName)
    {
        ev3Plugin.Call("playProgram", fileName);
    }

    public void PlayAudio(int volume, string fileName)
    {
        ev3Plugin.Call("playAudio", volume, fileName);
    }

    public void CallLedPattern(LedPattern pattern)
    {
        ev3Plugin.Call("setLedPattern", pattern.ToString());
    }

    public void StopDriveMotors()
    {
        if (ev3Plugin == null) return;
        ev3Plugin.Call("stopDriveMotors", LMotorPort.ToString(), LMotorPort.ToString());        
    }

    public void StopDirectionMotor()
    {
        if (ev3Plugin == null) return;
        ev3Plugin.Call("stopDriveMotors", RMotorPort.ToString(), RMotorPort.ToString());
    }

    public void StartDirectionMotor(Direction direction, int power = 100)
    {
        if (ev3Plugin == null) return;
        power = Mathf.Clamp(power, 0, 100);
        switch (direction)
        {
            case Direction.Left:
                ev3Plugin.Call("startDriveMotors", RMotorPort.ToString(), RMotorPort.ToString(), power * invert, power * invert);
                break;
            case Direction.Right:
                ev3Plugin.Call("startDriveMotors", RMotorPort.ToString(), RMotorPort.ToString(), -power * invert, -power * invert);
                break;
        }
    }

    public void StartDriveMotors(Direction direction, int power = 100)
    {
        if (ev3Plugin == null) return;
        power = Mathf.Clamp(power, 0, 100);
        switch(direction)
        {
            case Direction.Forward:
                ev3Plugin.Call("startDriveMotors", LMotorPort.ToString(), LMotorPort.ToString(), power * invert, power * invert);
                break;

            case Direction.Backwards:
                ev3Plugin.Call("startDriveMotors", LMotorPort.ToString(), LMotorPort.ToString(), -power * invert, -power * invert);
                break;

            case Direction.Left:
                ev3Plugin.Call("startDriveMotors", LMotorPort.ToString(), LMotorPort.ToString(), power * invert, -power * invert);
                break;

            case Direction.Right:
                ev3Plugin.Call("startDriveMotors", LMotorPort.ToString(), LMotorPort.ToString(), -power * invert, power * invert);
                break;
        }
    }

    #endregion Command Methods

    #region Input Triggers

    void throwAlertUpdate(string data)
    {
        if (GUIManager.Instance.DebugWindow != null)
            GUIManager.Instance.DebugWindow.text = data;
    }

    void throwBluetoothConnection(string data)
    {
        GUIManager.Instance.LoadGameplayScreen();

        switch (data)
        {
            case "true": IsConnected = true; break;
            case "false": IsConnected = false; break;
        }

        if (GUIManager.Instance.DebugWindow != null)
            GUIManager.Instance.DebugWindow.text = "Bluetooth Connected; " + IsConnected.ToString();

        if (BluetoothUpdateEvent != null)
            BluetoothUpdateEvent(IsConnected);
    }

    void throwBluetoothAlert(string data)
    {
        string[] input = data.Split(',');
    }

    void throwButtonUpdate(string data)
    {
        string[] input = data.Split(',');

        if (input.Length == 2)
        {
            string buttonType = input[0];
            string buttonState = input[1];

            BrickButton bb = (BrickButton)System.Enum.Parse(typeof(BrickButton), buttonType);
            bool state = bool.Parse(buttonState);

            if (ButtonUpdateEvent != null)
                ButtonUpdateEvent(bb, state);

            // print("Button: " + bb.ToString() + "," + state.ToString());
        }
    }

    void throwPortDeviceUpdate(string data)
    {
        string[] input = data.Split(',');

        if(input.Length == 2)
        {
            string deviceType = input[1];
            string devicePort = input[0];

            Device d = (Device)System.Enum.Parse(typeof(Device), deviceType);
            InputPort p = (InputPort)System.Enum.Parse(typeof(InputPort), devicePort);

            if (throwPortDeviceUpdateEvent != null)
                throwPortDeviceUpdateEvent(p, d);

            print("PortDeviceUpdate: " + p + "," + d);
        }
    }

    void throwPortSensorUpdate(string data)
    {
        string[] input = data.Split(',');

        if (input.Length == 6)
        {
            string devicePort = input[0];
            string deviceType = input[1];
            string deviceMode = input[2];

            string ValueR = input[3];
            string ValueS = input[4];
            string ValueP = input[5];

            // print("PortSensorUpdate: " + devicePort + "," + deviceType + "," + deviceMode + "," + ValueR);

            PortState portState = new PortState();

            portState.InputPort = (InputPort)System.Enum.Parse(typeof(InputPort), devicePort);
            portState.Device = (Device)System.Enum.Parse(typeof(Device), deviceType);
            portState.Mode = int.Parse(deviceMode);
            portState.Raw = double.Parse(ValueR);
            portState.SI = double.Parse(ValueS);
            portState.Percent = double.Parse(ValueP);

            if (throwPortDataUpdateEvent != null)
                throwPortDataUpdateEvent(portState);

            // Throw sensor events

            if(portState.Device == Device.Color)
            {
                if (ColourSensorUpdateEvent != null)
                    ColourSensorUpdateEvent(portState);
            }

            if(portState.Device == Device.Infrared)
            {
                if (InfraredSensorUpdateEvent != null)
                    InfraredSensorUpdateEvent(portState);
            }

            if(portState.Device == Device.Touch)
            {
                if (TouchSensorUpdateEvent != null)
                    TouchSensorUpdateEvent(portState);
            }

            if(portState.Device == Device.Gyroscope)
            {
                if (GyroscopeSensorUpdateEvent != null)
                    GyroscopeSensorUpdateEvent(portState);
            }

            if(portState.Device == Device.Ultrasonic)
            {
                if (UltrasonicSensorUpdateEvent != null)
                    UltrasonicSensorUpdateEvent(portState);
            }
        }
    }

    #endregion Input Triggers
}
