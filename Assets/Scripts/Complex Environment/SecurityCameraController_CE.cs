using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraController_CE : MonoBehaviour
{
    public Transform[] camerasTr;
    public SecurityCameraFOV_CE[] camerasFov;
    public CameraRotation_CE [] camerasRotation;
    public int totalTimeDetectRobber=0;
    public SecurityCameraFOV_CE lastCameraToDetect;
    void Awake()
    {
        camerasTr = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            camerasTr[i] = transform.GetChild(i);

        camerasFov = GetComponentsInChildren<SecurityCameraFOV_CE>();
        camerasRotation = GetComponentsInChildren<CameraRotation_CE>();

    }
    public void Restart()
    {
        foreach (SecurityCameraFOV_CE cam in camerasFov)
        {
            cam.Restart();
        }
        foreach (CameraRotation_CE cam in camerasRotation)
        {
            cam.Restart();
        }
        totalTimeDetectRobber = 0;
        lastCameraToDetect = null;
    }
    void FixedUpdate()
    {
        totalTimeDetectRobber = 0;
        //Cameras talk to each other
        foreach (SecurityCameraFOV_CE cam in camerasFov)
        {
            totalTimeDetectRobber = totalTimeDetectRobber + cam.timeDetectRobber;
        }
    }
}
