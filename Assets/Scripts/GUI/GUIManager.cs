// <copyright file="GUIManager.cs" company="dyadica.co.uk">
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

public class GUIManager : MonoBehaviour
{
    #region Properties

    public static GUIManager Instance;

    public bool Debug = true;

    public Text DebugWindow;

    public PlayProgram[] ProgramList;
    public PlayAudio[] AudioList;

    public Toggle InvertMotors;

    public List<GameObject> Panels;

    public GameObject SplashPanel;
    public GameObject ConnectionPanel;
    public GameObject GameplayPanel;
    public GameObject VideoConfigPanel;
    public GameObject MenuPanel;
    public GameObject MotorsPanel;
    //power sliders
    public Slider slider1, slider2;
    //menu buttons
    public Button motorConfigBtn, videoConfigBtn, connectionBtn;
    #endregion Properties

    #region Unity Loop

    void Awake()
    {
        Instance = this;
    }

	void Start ()
    {
        if (InvertMotors != null)
            InvertMotors.isOn = EV3Manager.Instance.InvertMotors;
        //load value
        slider1.value = PlayerPrefs.GetFloat("slider1",50);
        slider2.value = PlayerPrefs.GetFloat("slider2", 50);

        LoadConnectionScreen();
    }

    #endregion Unity Loop

    #region Program Updates

    public void UpdateProgram(int program, string fileName)
    {
        if (ProgramList == null || ProgramList[program] == null)
            return;

        ProgramList[program].FileName = fileName;
    }

    #endregion Program Updates

    #region Audio Updates

    public void UpdateAudio(int audio, int volume, string fileName)
    {
        if (AudioList == null || AudioList[audio] == null)
            return;

        AudioList[audio].Volume = volume;
        AudioList[audio].FileName = fileName;
    }

    public void UpdateAudio(int audio, string fileName)
    {
        if (AudioList == null || AudioList[audio] == null)
            return;

        AudioList[audio].FileName = fileName;
    }

    public void UpdateAudio(int audio, int volume)
    {
        if (AudioList == null || AudioList[audio] == null)
            return;

        AudioList[audio].Volume = volume;
    }

    #endregion Audio Updates

    public void UpdateRobotName(string name)
    {
        EV3Manager.Instance.EV3Name = name;
    }

    public void UpdateInvertMotors()
    {
        if (InvertMotors != null)
            EV3Manager.Instance.InvertMotors = InvertMotors.isOn;
    }

    public void LoadConnectionScreen()
    {
        StartCoroutine(loopAndLoad());
    }

    IEnumerator loopAndLoad()
    {
        foreach (GameObject panel in Panels)
        {
            panel.SetActive(false);
        }
        SplashPanel.SetActive(true);
        yield return new WaitForSeconds(2);        
        SplashPanel.SetActive(false);

#if UNITY_ANDROID && !UNITY_EDITOR
        
        if(StreamingManager.roleServer) ConnectionPanel.SetActive(true);
        else LoadGameplayScreen();
#else
        MenuPanel.SetActive(true);
#endif
    }

    public void LoadGameplayScreen()
    {
        foreach(GameObject panel in Panels)
        {
            panel.SetActive(false);
        }

        GameplayPanel.SetActive(true);
    }

    public void LoadVideoConfigScreen()
    {
        foreach (GameObject panel in Panels)
        {
            panel.SetActive(false);
        }
        VideoConfigPanel.SetActive(true);
    }

    public void LoadMenuScreen()
    {
        foreach (GameObject panel in Panels)
        {
            panel.SetActive(false);
        }

        MenuPanel.SetActive(true);
        //disable buttons basing on current role
        motorConfigBtn.interactable = videoConfigBtn.interactable = connectionBtn.interactable = StreamingManager.roleServer;        
    }

    public void LoadMotorsScreen()
    {
        foreach (GameObject panel in Panels)
        {
            panel.SetActive(false);
        }

        MotorsPanel.SetActive(true);
    }

    public void QuitApp()
    {
        PlayerPrefs.SetFloat("slider1", slider1.value);
        PlayerPrefs.SetFloat("slider2", slider2.value);
        Application.Quit();
    }

}
