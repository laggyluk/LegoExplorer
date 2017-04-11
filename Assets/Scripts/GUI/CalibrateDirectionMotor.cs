using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MoveDirection { none, left, right };

public class CalibrateDirectionMotor : MonoBehaviour {

    
    public InputField limitField;
    //how long small engine can rotate (s)
    public static float timeLimit;
    static float time;

	// Use this for initialization
	void Start ()
    {
        timeLimit = PlayerPrefs.GetFloat("timeLimit", 1f);
        limitField.text = timeLimit.ToString();
    }
	
	public void SetZero()
    {
        time = 0;
    }

    public void SetLimit()
    {
        timeLimit = float.Parse(limitField.text);
        PlayerPrefs.SetFloat("timeLimit", timeLimit);
    }

    //stops direction motor if it runs for too long in same direction
    void Update()
    {
        if (!active) return;
        bool stop = false;
        if(direction==MoveDirection.left)
        {
            time -= Time.deltaTime;
            if (time <= timeLimit)
            {
                stop = true;
                time = -timeLimit;
            }
        }
        if (direction == MoveDirection.right)
        {
            time += Time.deltaTime;
            if (time >= timeLimit)
            {
                stop = true;
                time = timeLimit;
            }
        }
        if (stop)
        {
            print("Direction Motor Limit ");
            active = false;
            EV3Manager.Instance.StopDirectionMotor();
        }
    }

    static MoveDirection direction;
    static bool active;

    public static void SetDirection(MoveDirection dir)
    {
        print("SetDirection " + dir.ToString());
        direction = dir;
        if (dir == MoveDirection.none) active = false;
        else active = true;
    }
}
