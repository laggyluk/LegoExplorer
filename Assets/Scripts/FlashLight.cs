using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour {
    /*
    static AndroidJavaObject camera = null;
    static AndroidJavaObject cameraParameters = null;

    public static void ToggleAndroidFlashlight()
    {
        if (camera == null)
        {
            AndroidJavaClass cameraClass = new AndroidJavaClass("android.hardware.Camera");
            camera = cameraClass.CallStatic<AndroidJavaObject>("open", 0);
            if (camera != null)
            {
                cameraParameters = camera.Call<AndroidJavaObject>("getParameters");
                cameraParameters.Call("setFlashMode", "torch");
                camera.Call("setParameters", cameraParameters);
            }
        }
        else
        {
            cameraParameters = camera.Call<AndroidJavaObject>("getParameters");
            string flashmode = cameraParameters.Call<string>("getFlashMode");
            if (flashmode != "torch")
                cameraParameters.Call("setFlashMode", "torch");
            else
                cameraParameters.Call("setFlashMode", "off");

            camera.Call("setParameters", cameraParameters);
        }
    }

    void ReleaseAndroidJavaObjects()
    {
        if (camera != null)
        {
            camera.Call("release");
            camera = null;
        }
    }

    private void OnApplicationQuit()
    {
        ReleaseAndroidJavaObjects();
    }
    */

    private static bool Active;
    private static AndroidJavaObject camera1;

    public static void ToggleAndroidFlashlight()
    {
        if (Active) FL_Stop(); else FL_Start();
    }

    static void FL_Start()
    {
        AndroidJavaClass cameraClass = new AndroidJavaClass("android.hardware.Camera");
        WebCamDevice[] devices = WebCamTexture.devices;

        int camID = 0;
        camera1 = cameraClass.CallStatic<AndroidJavaObject>("open", camID);

        if (camera1 != null)
        {
            AndroidJavaObject cameraParameters = camera1.Call<AndroidJavaObject>("getParameters");
            cameraParameters.Call("setFlashMode", "torch");
            camera1.Call("setParameters", cameraParameters);
            camera1.Call("startPreview");
            Active = true;
        }
        else
        {
            Debug.LogError("[CameraParametersAndroid] Camera not available");
        }

    }

    void OnDestroy()
    {
        FL_Stop();
    }

    static void FL_Stop()
    {

        if (camera1 != null)
        {
            camera1.Call("stopPreview");
            camera1.Call("release");
            Active = false;
        }
        else
        {
            Debug.LogError("[CameraParametersAndroid] Camera not available");
        }

    }

}
