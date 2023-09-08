using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    MeshRenderer render;
    public bool alarm = false;
    public bool alarmEnabled = true;
    public int timeSinceAlarmIsOff = 0;
    void Awake()
    {
        render = transform.root.GetChild(4).GetChild(0).gameObject.GetComponent<MeshRenderer>();
    }
    public void Restart()
    {
        alarm = false;
        alarmEnabled = true;
        timeSinceAlarmIsOff = 0;
        render = transform.root.GetChild(4).GetChild(0).gameObject.GetComponent<MeshRenderer>();

        Color color = new Color(0.4134924f, 0.8679245f, 0.8453907f, 0.6156863f);
        render.material.SetColor("_Color", color);
    }
    public void SetAlarmOff()
    {
        if(alarmEnabled)
            alarm = true;
       
        Color color = new Color(1f, 0.12f, 0.15f, 0.5f);
        render.material.SetColor("_Color", color);
    }
    void FixedUpdate()
    {
        if(alarm)
            timeSinceAlarmIsOff++;
    }
    
}
