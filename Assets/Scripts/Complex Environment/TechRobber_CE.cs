using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechRobber_CE : MonoBehaviour
{
    public bool[] canDisableCam;
   
   
    public List<GameObject> listCameras = new List<GameObject>();//list of cameras that can disable at the same time
    private List<int> cameraIdx = new List<int>();
   public bool canDisableAlarm;
    public GameObject alarm;
    
    private GameObject[] gates;
    private GameObject[] cameras;
  
    private GroupController_CE controller;
    private TechRobberAgent_CE agent;
    public int disableCamera;
    public bool hasFOV = true;

   
    int id;

    void Awake()
    {
        controller = transform.root.gameObject.GetComponent<GroupController_CE>();
        canDisableCam = new bool[8];
        for(int i = 0; i < canDisableCam.Length; i++)
        {
            canDisableCam[i] = false;
        }
        

        agent = transform.parent.GetChild(0).GetComponent<TechRobberAgent_CE>();
        }

    public void Restart()
    {
        StopAllCoroutines();
        canDisableCam = new bool[8];
        for (int i = 0; i < canDisableCam.Length; i++)
        {
            canDisableCam[i] = false;
        }
       
       
        listCameras.Clear();
        if (alarm)
        {
           
            alarm = null;
        }
    }
    
    public void actDisableCamera()
    {

        

        if (listCameras.Count > 0)
        {

            for (int i=0; i< listCameras.Count;i++)
            {
                if (canDisableCam[cameraIdx[i]])
                {
                    // listCameras[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SecurityCameraFOV_CE>().cameraOn = false;
               //     controller.AssignRobbersReward(0.5f/(float)controller.MaxEnvironmentSteps);
                    StartCoroutine(DisableCamera(listCameras[i].transform.GetChild(0).GetChild(0).gameObject));
                }


            }



        }
       

    }

    public void actDisableAlarm()
    {
        if (canDisableAlarm)
        {
            alarm.GetComponent<Alarm>().alarmEnabled = false;
        }
    }
       


   
    public int cameraRoom;
    public int thiroom;
    private void OnTriggerEnter(Collider other)
    {
        /*  if ( other.tag == "Security Camera")
          {
              canDisableCam = true;
             // Debug.Log("Can disable Camera Press C");
              secCamera = other.gameObject;
          }*/
        if (other.tag == "Security Camera")
        {
            int idx = other.transform.parent.GetSiblingIndex();
            cameraRoom = (int)(0.5f * idx) + 1;

            if (agent)
            {
                //   if (other.gameObject.name.Equals(agent.activeCam[idx].transform.GetChild(0).gameObject.name))
                //{
                thiroom = transform.parent.GetChild(1).GetComponent<RobberMesh>().currRoom;
                if (cameraRoom == transform.parent.GetChild(1).GetComponent<RobberMesh>().currRoom)
                {

                    canDisableCam[idx] = true;
                    listCameras.Add(other.gameObject);
                    cameraIdx.Add(idx);
                }
               // }
                
            }



        }
       
        if(other.gameObject.tag == "Alarm")
        {
            alarm = other.gameObject;
            canDisableAlarm = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Security Camera")
        {
            int idx = other.transform.parent.GetSiblingIndex();
            if (agent)
            {
               // if (other.gameObject.name.Equals(agent.activeCam[idx].transform.GetChild(0).gameObject.name))
                //{

                    canDisableCam[idx] = false;
                    cameraIdx.Remove(idx);

               // }
                
                listCameras.Clear();
                cameraIdx.Clear();
            }
           

        }
       
        if (other.gameObject.tag == "Alarm")
        {
            alarm = null;
            canDisableAlarm = false;
        }

    }

    private IEnumerator DisableCamera(GameObject cam)
    {
        cam.GetComponent<SecurityCameraFOV_CE>().cameraOn = false;


        yield return new WaitForSeconds(30f);
        cam.GetComponent<SecurityCameraFOV_CE>().cameraOn = true;


    }
    
}
