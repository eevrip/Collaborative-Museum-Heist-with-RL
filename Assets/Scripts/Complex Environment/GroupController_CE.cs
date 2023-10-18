using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System.IO;
using TMPro;

public class GroupController_CE : MonoBehaviour
{
    [Serializable]
    public class RobberInfo
    {

        public GameObject robber;
        [HideInInspector]
        public int skills;
        [HideInInspector]
        public int teamID; //guards = 0, robbers = 1

        [HideInInspector]
        public int id;

    }

    public int maxTimeAlarm = 100;
    public bool firstEd;
    //Robbers
    public List<RobberInfo> TeamRobbers; //Enter manually in Inspector
  //  public List<GuardFSM> guards;
    [HideInInspector]
    public SimpleMultiAgentGroup agentRobbersGroup;

    public int MaxEnvironmentSteps = 5000;
    private bool m_Initialized;
    public int m_ResetTimer;

    private bool m_GameEnded;

    [HideInInspector]
    public SecurityCameraController_CE cameraController;
    [HideInInspector]
    public GuardsController_CE guardController;

    private GameObject[] cameras;

    


    public bool restart = false;

    [HideInInspector]
    public MuseumMap museumMap;
    public int m_completedEpisodes;






    [HideInInspector]
    public Vector3 goalPos;






 private float maxStepsGuardInFOV = 100f;
    int dtInCamFOV = 0;

   int dtInGuardFOV = 0;
   
    [Tooltip("Maximum steps that a robber appears in the fov of either that of a camera's or a guard's before the alarm is triggered")]

    public int maxStepsInFOV = 600;
    public int maxStepsInFOVCamera = 200;
    public float maxStepsInFOV_LS =600f;
    public float maxStepsInFOV_TG = 600f;
    public int numCollectedValuables = 0;


    public bool[] dataBool = new bool[6];
    public int[] dataInt = new int[5];
    void Awake()
    {
        

        museumMap = transform.GetChild(1).gameObject.GetComponent<MuseumMap>();
        cameraController = transform.GetChild(1).GetChild(1).gameObject.GetComponent<SecurityCameraController_CE>();
        guardController = transform.GetChild(3).gameObject.GetComponent<GuardsController_CE>();
        SetMaxStepsInFOV();
        Initialize();
       
    }
    void Initialize()
    {

        //INITIALIZE AGENTS
        int new_id = 0;
        foreach (var item in TeamRobbers)
        {



            if (item.robber.name.Contains("TechGuy"))
            {
                // Debug.Log("Techguy");
                TechRobberAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<TechRobberAgent_CE>();
                temp.Initialize();
                item.skills = 1;
                //   item.robAgent.m_BehaviorParameters.TeamId = 1;
                item.teamID = 1;
                // agentRobbersGroup.RegisterAgent(temp);

                item.id = new_id;
            }
            else if (item.robber.name.Contains("Locksmith"))
            {
                LocksmithAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<LocksmithAgent_CE>();
                temp.Initialize();
                item.skills = 0;
                //   item.robAgent.m_BehaviorParameters.TeamId = 1;
                item.teamID = 1;
                // agentRobbersGroup.RegisterAgent(temp);

                item.id = new_id;
            }
            //depending on skills get Agent script. Needs thinking
            new_id++;
        }
       /* guards = new List<GuardFSM>();
        Transform guardParent = transform.GetChild(3);
        for (int i = 0; i < guardParent.childCount; i++)
        {
            guards.Add(guardParent.GetChild(i).GetChild(0).gameObject.GetComponent<GuardFSM>());

        }
       */

        m_Initialized = true;

        
    ResetScene();
    }
    private void SetMaxStepsInFOV()
    {
      //  Debug.Log(maxStepsInFOV  + " ls =" + maxStepsInFOV_LS + " tg = " + maxStepsInFOV_TG);
      //  maxStepsInFOV_LS = Academy.Instance.EnvironmentParameters.GetWithDefault("max_steps_in_fov_ls", maxStepsInFOV);
       // maxStepsInFOV_TG = Academy.Instance.EnvironmentParameters.GetWithDefault("max_steps_in_fov_tg", maxStepsInFOV);
        maxStepsInFOV_LS = Academy.Instance.EnvironmentParameters.GetWithDefault("max_steps_in_fov_ls", maxTimeAlarm);
        maxStepsInFOV_TG = Academy.Instance.EnvironmentParameters.GetWithDefault("max_steps_in_fov_tg", maxTimeAlarm);
        if (maxStepsInFOV_LS == maxStepsInFOV_TG)
           maxTimeAlarm = (int)maxStepsInFOV_LS;
        else if (maxStepsInFOV_LS < maxStepsInFOV_TG)
            maxTimeAlarm = (int)maxStepsInFOV_TG;
        else
            maxTimeAlarm = (int)maxStepsInFOV_LS;
      //  if (maxStepsInFOV > 200)
      //      maxStepsInFOVCamera = maxStepsInFOV;
       // else
         //   maxStepsInFOVCamera = 200;

       // Debug.Log(maxStepsInFOV + " ls =" + maxStepsInFOV_LS + " tg = " + maxStepsInFOV_TG);
    }
    void FixedUpdate()
    {
        // CheckAllAgentsInSameRoom();
        if (!m_Initialized) return;
        museumMap.BlockPath();



        //RESET SCENE IF WE MaxEnvironmentSteps
        m_ResetTimer += 1;
        //If an alarm is off for a certain time, endepisode
        foreach (GameObject alarm in museumMap.alarms)
        {
           
            if (alarm.GetComponent<Alarm>().alarm & alarm.GetComponent<Alarm>().timeSinceAlarmIsOff >= maxTimeAlarm)
            {
                
                AssignRobbersReward(-1f);
                AssignNoEnteringMuseumPunishment();
                AssignGiveUpPunishment();
                EndRobbersEpisode();
                ResetScene();

            }
        }
        if (m_ResetTimer >= MaxEnvironmentSteps)
        {
            //AssignRobbersReward(-2f);
            AssignNoEnteringMuseumPunishment();
            AssignGiveUpPunishment();
            EndRobbersEpisode();
            
            ResetScene();

        }





    }


   


    public void AssignRobbersReward(float reward)
    {
        foreach (var item in TeamRobbers)
        {
            if (item.skills == 1)
            {
                TechRobberAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<TechRobberAgent_CE>();
                temp.AddReward(reward);
                  //  Debug.Log(m_ResetTimer + " " + temp + " "+temp.GetCumulativeReward());

            }
            else if (item.skills == 0)
            {
                LocksmithAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<LocksmithAgent_CE>();
                temp.AddReward(reward);
                // Debug.Log(m_ResetTimer + " " + temp + " " +temp.GetCumulativeReward());
            }


        }
    }

    public void RewardInCameraFOV()
    {
       dtInCamFOV = dtInCamFOV + 1;
        if (dtInCamFOV <= maxStepsInFOVCamera)
        {// Debug.Log("In cam FOV " + dtInCamFOV);
          AssignRobbersReward(-0.5f / (maxStepsInFOVCamera));
         

        }


        /*  if (dtInCamFOV <= maxStepsInFOV)
          {

              //  AssignRobbersReward(-0.5f);
              EndRobbersEpisode();

          }*/


    }

    public void RewardInGuardFOV()
    {
     
      dtInGuardFOV = dtInGuardFOV + 1;

         if (dtInGuardFOV <= maxStepsInFOV)
          {

            //    AssignRobbersReward(-0.5f / (maxStepsInFOV));
            AssignRobbersReward(-1.5f / (maxStepsInFOV));

        }
       

    }
    float tot = 0f;
    public void RewardInGuardFOV(float dt)
    {
       
        tot = tot + dt;
        if (guardController.totalTimeDetectRobber <= maxStepsGuardInFOV)
        { 
         //   Debug.Log("In guard FOV " + (-0.5f * dt / maxStepsGuardInFOV) + "dt " + dt + "max=" + maxStepsGuardInFOV);
            AssignRobbersReward(-0.5f *dt / maxStepsGuardInFOV);
            TechRobberAgent_CE temp = TeamRobbers[0].robber.transform.GetChild(0).gameObject.GetComponent<TechRobberAgent_CE>();
          //  Debug.Log(" Guard reward" + " " + temp + " " + temp.GetCumulativeReward() +"total time" + guardController.totalTimeDetectRobber + "total = " + tot);


        }
        //dtInGuardFOV = dtInGuardFOV + 1;

        /*  if (dtInGuardFOV >= maxStepsInFOV)
          {

               AssignRobbersReward(-0.5f / (maxStepsInFOV));

          }*/


    }

    public void EndRobbersEpisode()
    {
       
        restart = true;
        m_completedEpisodes += 1;
        // Debug.Log("Endepisode");
        foreach (var item in TeamRobbers)
        {
            if (item.skills == 1)
            {
                TechRobberAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<TechRobberAgent_CE>();
           //     Debug.Log(" EndEpisode" + " " + temp + " " + temp.GetCumulativeReward());
                temp.EndEpisode();
                dataInt[1] = temp.room;
            }
            else if (item.skills == 0)
            {
                LocksmithAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<LocksmithAgent_CE>();
              // Debug.Log(" EndEpisode" + " " + temp + " " + temp.GetCumulativeReward());
                temp.EndEpisode();
                dataInt[0] = temp.room;
            }


        }
       // PrintData(dataBool, dataInt);
        ResetScene();
    }
    public void EndRobberEpisode()
    {

      //  Debug.Log("EndEpisode");
        foreach (var item in TeamRobbers)
        {

            if (item.robber.transform.GetChild(1).GetComponent<RobberMesh>().reachGoal)
            {
                if (item.skills == 1)
                {
                    dataBool[5] = true;
                }
                else
                {
                    dataBool[4] = true;
                }
                Debug.Log(item.robber.name);
                continue;
                
            }
            else
                return;

        }
        dataBool[1] = true;
       
        //get a bonus reward for reaching goal early
        //   AssignRobbersReward((MaxEnvironmentSteps-m_ResetTimer)/(float)MaxEnvironmentSteps);
        AssignGiveUpPunishment();
        EndRobbersEpisode();//End episode for everyone, if they all reached the goal

    }
   
        public void AssignIndividualReward(float reward, int robberId)
    {
        if (robberId == 0)
        {
            TechRobberAgent_CE temp = TeamRobbers[robberId].robber.transform.GetChild(0).gameObject.GetComponent<TechRobberAgent_CE>();
            temp.AddReward(reward);
           // Debug.Log(m_ResetTimer + " Individual " + temp + " " + temp.GetCumulativeReward());

        }
        else if (robberId == 1)
        {
            LocksmithAgent_CE temp = TeamRobbers[robberId].robber.transform.GetChild(0).gameObject.GetComponent<LocksmithAgent_CE>();
            temp.AddReward(reward);
            //  Debug.Log(m_ResetTimer + " Individual " + temp + " " +  temp.GetCumulativeReward());

        }
    }
    public void AssignNoEnteringMuseumPunishment()
    {

        foreach (var item in TeamRobbers)
        {

           /* if (item.skills == 1)
            {
                TechRobberAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<TechRobberAgent_CE>();
                if (!museumMap.outOfSR)//one of them have not exit the Start region
                {
                    temp.AddReward(-3f);
                      //  Debug.Log(m_ResetTimer + " No Entering museum" + temp.timeSinceReachGoal+" " + temp + " " + temp.GetCumulativeReward());
                }
            }
            else if (item.skills == 0)
            {
                LocksmithAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<LocksmithAgent_CE>();
                if (!museumMap.outOfSR)//agent has not enter any room
                {
                    temp.AddReward(-3f);
                   //  Debug.Log(m_ResetTimer + " No Entering museum" + temp.timeSinceReachGoal + " " + temp + " " + temp.GetCumulativeReward());
                }

            }
         
            if (item.skills == 1)
            {
                TechRobberAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<TechRobberAgent_CE>();
                if (temp.meshGo.GetComponent<RobberMesh>().currRoom == 0)//agent has not enter any room
                {
                    temp.AddReward(-5f);
                    //   Debug.Log(m_ResetTimer + " No Entering museum " + temp.GetCumulativeReward());
                }
            }
            else if (item.skills == 0)
            {
                LocksmithAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<LocksmithAgent_CE>();
                if (temp.meshGo.GetComponent<RobberMesh>().currRoom == 0)//agent has not enter any room
                {
                    temp.AddReward(-5f);
                  //  Debug.Log(m_ResetTimer + " No Entering museum " + temp.GetCumulativeReward());
                }

            }*/

 	if (item.skills == 1)
            {
                TechRobberAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<TechRobberAgent_CE>();
                if (!temp.meshGo.GetComponent<RobberMesh>().outOfSR)//agent has not enter any room
                {
                    temp.AddReward(-3f);
                    //   Debug.Log(m_ResetTimer + " No Entering museum " + temp.GetCumulativeReward());
                }
            }
            else if (item.skills == 0)
            {
                LocksmithAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<LocksmithAgent_CE>();
                if (!temp.meshGo.GetComponent<RobberMesh>().outOfSR)//agent has not enter any room
                {
                    temp.AddReward(-3f);
                  //  Debug.Log(m_ResetTimer + " No Entering museum " + temp.GetCumulativeReward());
                }

            }
        }
    }



    public void AssignGiveUpPunishment()
    {
        foreach (var item in TeamRobbers)
        {
            //Only agents that reach goal before the agent that has the valuable
            if (item.robber.transform.GetChild(1).GetComponent<RobberMesh>().reachGoal)
            {
                if (item.skills == 1)
                {
                    TechRobberAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<TechRobberAgent_CE>();
                    //  temp.AddReward(-3f * (float)temp.timeSinceReachGoal / (float)MaxEnvironmentSteps);
                    temp.AddReward(-3f * (float)temp.timeSinceReachGoal / (float)m_ResetTimer);
                    //   Debug.Log("Give Up"+ temp.timeSinceReachGoal+" " + temp + " " + temp.GetCumulativeReward());

                }
                else if (item.skills == 0)
                {
                    LocksmithAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<LocksmithAgent_CE>();
                    // temp.AddReward(-3f * (float)temp.timeSinceReachGoal / (float)MaxEnvironmentSteps);
                    temp.AddReward(-3f * (float)temp.timeSinceReachGoal / (float)m_ResetTimer);
                    //    Debug.Log("Give Up" + temp.timeSinceReachGoal+" " + temp + " " + temp.GetCumulativeReward());

                }

            }
        }
    }
    public void SetGoalPosition(Transform other, int robberId)
    {
        if (robberId == 0)
        {
           TeamRobbers[robberId].robber.transform.position = other.position;
            


        }
        else if (robberId == 1)
        {
            TeamRobbers[robberId].robber.transform.position = other.position;
            
        }
    }
    void ResetScene()
    {
        dataReset();
        SetMaxStepsInFOV();
       
       // Debug.Log("Reset");
        museumMap.CreateMap();
        cameraController.Restart();
      
       guardController.Restart();
        foreach (GameObject alarm in museumMap.alarms)
        {
            alarm.GetComponent<Alarm>().Restart();
        }
        m_GameEnded = false;
        m_ResetTimer = 0;
        numCollectedValuables = 0;
        dtInCamFOV = 0;
        dtInGuardFOV = 0;

        tot = 0;

        







        foreach (var item in TeamRobbers)
        {

            if (item.skills == 1)
            {
                TechRobberAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<TechRobberAgent_CE>();
                temp.ResetAgent();

            }

            else if (item.skills == 0)
            {
                LocksmithAgent_CE temp = item.robber.transform.GetChild(0).gameObject.GetComponent<LocksmithAgent_CE>();
                temp.ResetAgent();

            }


        }

        for (int i = 0; i < museumMap.roomsSc.Length; i++)
        {
            if (museumMap.roomsSc[i])
            {
                //  roomsSc[i].numberOfAgents = 0;
               museumMap.roomsSc[i].Restart();
            }

        }


        restart = false;

       

    }






    public void dataReset()
    {

        for (int i = 0; i < dataBool.Length; i++)
        {
            dataBool[i] = false;
        }
        for (int i = 0; i < dataInt.Length; i++)
        {
            dataInt[i] = -100;
        }
    }






void OnApplicationQuit()
    {
        m_completedEpisodes++;
       // PrintData(dataBool, dataInt);
    }




    public void PrintData( bool[] dataB, int[] dataI)
    {
        Debug.Log("Time= " + m_ResetTimer);
        foreach (GameObject alarm in museumMap.alarms)
        {
            if (alarm.GetComponent<Alarm>().alarm)
            {
                dataBool[0] = true;
            }
        }
            dataInt[2] = numCollectedValuables;
        foreach (var item in TeamRobbers)
        {

            if (item.robber.transform.GetChild(1).GetComponent<RobberMesh>().reachGoal)
            {
                if (item.skills == 1)
                {
                    dataBool[5] = true;
                }
                else
                {
                    dataBool[4] = true;
                }
            }
        }
                Debug.Log(dtInGuardFOV + " " + maxStepsInFOV);
        if (dtInGuardFOV >= maxStepsInFOV)
        {
           
            dataBool[3] = true;
        }
        if (dtInCamFOV >= maxStepsInFOVCamera)
        {
            dataBool[2] = true;
        }
        string fullNameDoc = "/Data/Vanilla_I_Time"  + ".csv";
        string path = Application.dataPath + fullNameDoc;
        StreamWriter sr = new StreamWriter(path, true); //append to existing files


        sr.WriteLine("Run " + m_completedEpisodes.ToString());
      //  sr.WriteLine("Alarm, Win, LossCamera, LossGuards, GoalAreaL, GoalAreaT, RoomL, RoomT, Valuables, CamerasTime, GuardsTime, Time");
        string all = "";

        for (int i = 0; i < dataB.Length; i++)
        {
            if(i==0)
                all = dataB[i].ToString();
            else
                all = all + ", " + dataB[i].ToString();
        }
        for (int i = 0; i < dataI.Length; i++)
        {
           
                all = all + ", " + dataI[i].ToString();
        }
        sr.WriteLine(all+ ", "+m_ResetTimer.ToString());
        sr.WriteLine("Path:");
        all = museumMap.pathToTake[0].ToString();
        for (int i = 1; i < museumMap.pathToTake.Length; i++)
        {
            all = all + ", " + museumMap.pathToTake[i].ToString();
        }
        sr.WriteLine(all);
        sr.Close();

    }

    


        // Update is called once per frame
        void Update()
    {
        if (!m_Initialized)
        {
            Initialize();
        }
    }
}
