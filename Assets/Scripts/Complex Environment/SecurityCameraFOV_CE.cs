using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraFOV_CE : MonoBehaviour
{
    public bool cameraOn = true;
    public bool[] detectRobberArr;
    public bool detectRobber;
    private GroupController_CE controller;
    private int numRobbers;
    public int timeDetectRobber = 0;
    private Transform robbersTeam;
    public int room;
    private int maxTime;
    public SecurityCameraController_CE cameraController;
    public LayerMask checkLayerMask;
    public void Awake()
    {
        controller = this.transform.root.gameObject.GetComponent<GroupController_CE>();
        robbersTeam = transform.root.GetChild(2);
        if (robbersTeam)
        {
            numRobbers = robbersTeam.childCount;// controller.TeamRobbers.Count;
                                                //  Debug.Log(numRobbers);
            detectRobberArr = new bool[numRobbers];
            for (int i = 0; i < detectRobberArr.Length; i++)
            {
                detectRobberArr[i] = false;
            }

        }
        else
        {
            detectRobberArr = new bool[1];
            detectRobberArr[0] = false;
        }
        room = (int)(0.5f*transform.parent.parent.GetSiblingIndex());
        maxTime = controller.maxStepsInFOVCamera;
        cameraController = controller.cameraController;
       
         
    }
    public void Restart()

    {
        MeshRenderer render = GetComponent<MeshRenderer>();
        Color color = new Color(0.4134924f, 0.8679245f, 0.8453907f, 0.6156863f);
        render.material.SetColor("_Color", color);
        for (int i = 0; i < detectRobberArr.Length; i++)
        {
            detectRobberArr[i] = false;
        }
        detectRobber = false;
        timeDetectRobber = 0;
        cameraOn = true;
    }
    void FixedUpdate()
    {
        if (cameraOn)
        {
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<MeshCollider>().enabled = true;
            MeshRenderer render = GetComponent<MeshRenderer>();
            Color color = new Color(0.4134924f, 0.8679245f, 0.8453907f, 0.6156863f);
            render.material.SetColor("_Color", color);
            for (int i = 0; i < detectRobberArr.Length; i++)
            {
                detectRobberArr[i] = false;
            }
            detectRobber = false;

        }
        else
        {
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

        }


        if (cameraController.totalTimeDetectRobber >= controller.maxStepsInFOVCamera && cameraController.lastCameraToDetect == this)
        {
            controller.museumMap.alarms[room].GetComponent<Alarm>().SetAlarmOff();
            controller.dataInt[3] = controller.m_ResetTimer;
        }


    }
    public bool meshDete = false;
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "RobberMesh")
        {
           
            int idxRob = other.transform.parent.GetSiblingIndex();
            Vector3 dir = other.transform.position - transform.parent.position + new Vector3(0f, 0.5f, 0f) - new Vector3(0f, 0.9f, 0f);
            RaycastHit hit;

            // if (Physics.Raycast(transform.parent.position, dir.normalized, out hit, 1000, 11))
            //{
            if (Physics.Raycast(transform.parent.position + new Vector3(0f,-0.5f,0f), dir.normalized, out hit, 1000, checkLayerMask)) 
            { 

            if (hit.collider.gameObject.tag == "RobberMesh")
                {
                    //Debug.DrawRay(transform.parent.position + new Vector3(0f, -0.5f, 0f), dir, Color.black);
                   

                    MeshRenderer render = GetComponent<MeshRenderer>();
                    Color color = new Color(1f, 0.12f, 0.15f, 0.5f);
                    render.material.SetColor("_Color", color);
                    detectRobber = true;// Debug.Log(detectRobber);
                    detectRobberArr[idxRob] = true;
                    timeDetectRobber++;
                    if (controller)
                    {
                        
                            controller.RewardInCameraFOV();
                    }
                    cameraController.lastCameraToDetect = this;
                }
            }



        }
    }
    
    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "RobberMesh")
        {
            int idxRob = other.transform.parent.GetSiblingIndex();
            Vector3 dir = other.transform.position - transform.parent.position + new Vector3(0f, 0.5f, 0f) - new Vector3(0f, 0.9f, 0f);
            RaycastHit hit;
           
           // if (Physics.Raycast(transform.parent.position, dir.normalized, out hit, 1000, 11))
            //{
                if (Physics.Raycast(transform.parent.position - new Vector3(0f, 0.5f, 0f), dir.normalized, out hit, 1000, checkLayerMask))
                {

                    if (hit.collider.gameObject.tag == "RobberMesh")
                { //Debug.DrawRay(transform.parent.position + new Vector3(0f, -0.5f, 0f), dir, Color.black);
                   
                    MeshRenderer render = GetComponent<MeshRenderer>();
                    Color color = new Color(1f, 0.12f, 0.15f, 0.5f);
                    render.material.SetColor("_Color", color);
                    detectRobber = true;// Debug.Log(detectRobber);
                    detectRobberArr[idxRob] = true;
                    timeDetectRobber++;
                    if (controller)
                    {
                       
                            controller.RewardInCameraFOV();
                    }
                    cameraController.lastCameraToDetect = this;
                }
            }



        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RobberMesh")
        {
            int idxRob = other.transform.parent.GetSiblingIndex();
            MeshRenderer render = GetComponent<MeshRenderer>();
            Color color = new Color(0.4134924f, 0.8679245f, 0.8453907f, 0.6156863f);
            render.material.SetColor("_Color", color);
            detectRobber = false;
            detectRobberArr[idxRob] = false;
            //Debug.Log(detectRobber);
        }
    }
}
