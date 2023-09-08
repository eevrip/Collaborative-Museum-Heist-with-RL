using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardsController_CE : MonoBehaviour
{
    public GuardFSM[] guards;
  //  public float totalTimeDetectRobber = 0f;
    public int totalTimeDetectRobber = 0;
    public GuardFSM lastGuardToDetect;
    // Start is called before the first frame update
    void Awake()
    {
        guards = GetComponentsInChildren<GuardFSM>();
    }

    public void Restart()
    {
        foreach (GuardFSM guard in guards)
        {
            guard.Restart();
        }

        //totalTimeDetectRobber = 0f;
        totalTimeDetectRobber = 0;
        lastGuardToDetect = null;
    }
    void FixedUpdate()
    {
        //totalTimeDetectRobber = 0f;
        totalTimeDetectRobber = 0;
        //Cameras talk to each other
        foreach (GuardFSM guard in guards)
        {
            totalTimeDetectRobber = totalTimeDetectRobber + guard.dtDetectRobbers;
        }
    }
}
