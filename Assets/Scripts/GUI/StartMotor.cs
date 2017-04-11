﻿// <copyright file="StartMotor.cs" company="dyadica.co.uk">
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

using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class StartMotor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Slider powerSlider;
    public bool triggered = false;
    public Direction direction;
    public bool smallMotor = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (triggered == false)
        {
            triggered = true;
            if (smallMotor)
            {
                EV3Manager.Instance.StartDirectionMotor(direction, (int)powerSlider.value);
                if (direction == Direction.Left) CalibrateDirectionMotor.SetDirection(MoveDirection.left);
                if (direction == Direction.Right) CalibrateDirectionMotor.SetDirection(MoveDirection.right);
            }
            else
                EV3Manager.Instance.StartDriveMotors(direction, (int)powerSlider.value);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        triggered = false;
        if (smallMotor)
        {
            EV3Manager.Instance.StopDirectionMotor();
            CalibrateDirectionMotor.SetDirection(MoveDirection.none);
        }
        else
            EV3Manager.Instance.StopDriveMotors();
    }

    // Use this for initialization
    void Start ()
    {
        
	}
	
}
