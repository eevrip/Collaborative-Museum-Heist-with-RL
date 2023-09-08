using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberMesh : MonoBehaviour
{
    public int currRoom=0;
    public bool reachGoal = false;
    public int idx = -1;
public bool outOfSR = false;
    public void Restart()
    {
        currRoom = 0;
        reachGoal = false;
outOfSR = false;
    }
    
    void Awake()
    {
        idx = transform.parent.GetSiblingIndex(); //0 techguy, 1 locksmith
    }

}
