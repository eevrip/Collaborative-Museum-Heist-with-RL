using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class LocksmithAgent_CE : Agent
{
   
   
    public int timeSinceReachGoal = 0;
    private Transform tAgent;
    private Rigidbody rbAgent;

    private Vector3 goalTempPoint;
    //Get lights, gates, cameras, lasers state
    private Transform museum;


    
  
    private GameObject skillsGo;
    
    //  private bool enterMuseum = false;
    public float agentTerminalVel = 12f;
    public float turnSpeed = 150f;
    public float regMoveSpeed = 7f;
    //Action variables
    private int rotateAxis;
    private float hmove;
    private float vmove;
    private int pickDropVal;

    private int openGate;
    private GameObject robberGo;
    [HideInInspector]
    public GameObject meshGo;

   
    private MuseumMap museumMap;
    public Collider spawnAreaAgents;
   
    
   
    
    private bool m_FirstInitialize = true;
    private bool m_IsDecisionStep;
    private int m_AgentStepCount; //current agent step
    Vector3 initialPos;


    private float m_LocationNormalizationFactor = 72.0f;//52.0f; // About the size of a reasonable stage
    //Initialize
    
    
    [HideInInspector]
    public bool detectRobber = false;
    [HideInInspector]
    public BufferSensorComponent otherAgentsBuffer;
    

   

    private GroupController_CE controller;
    public int idxRob = -1;
    [HideInInspector]
    public float maxStepsInFOV = 20;
    // [HideInInspector]
    public bool reachGoal = false;
   
    public int room = 0;


    public override void Initialize()
    {

        // Debug.Log("Init");
        controller = transform.root.gameObject.GetComponent<GroupController_CE>();
        if (m_FirstInitialize)
        {
            tAgent = this.transform.parent;
            rbAgent = tAgent.gameObject.GetComponent<Rigidbody>();
            museumMap = controller.museumMap;
       
           

            var bufferSensors = GetComponentsInChildren<BufferSensorComponent>();
         otherAgentsBuffer = bufferSensors[0];
           initialPos = tAgent.localPosition;
            robberGo = tAgent.GetChild(2).gameObject;
            meshGo = tAgent.GetChild(1).gameObject;
            idxRob = tAgent.GetSiblingIndex();
            int numChild = tAgent.childCount;

            if (numChild >= 4) //take1 3, take2 4
            {
                //starts from 0, the forth child if exists it's the extra skills of the agent


                skillsGo = tAgent.GetChild(numChild - 1).gameObject;
                //Debug.Log(skillsGo.name);
            }
            m_FirstInitialize = false;
        }

        //On Episode begin equivalent
        
        spawnAreaAgents = controller.museumMap.spawnAreaAgents;
        reachGoal = meshGo.GetComponent<RobberMesh>().reachGoal;
       
    }

   
    public void ResetAgent()
    {
        
        room = 0;
        
        spawnAreaAgents = controller.museumMap.spawnAreaAgents;
        reachGoal = meshGo.GetComponent<RobberMesh>().reachGoal;

        rbAgent.velocity = Vector3.zero;//Initializes to zero the  velocity
        float randRot = Random.Range(0f, 359f);

        tAgent.localRotation = Quaternion.Euler(0f, randRot, 0f);//Quaternion.identity;
     
        //InitializeAgentPos();
       
        skillsGo.GetComponent<LocksmithRobber>().Restart();

        robberGo.GetComponent<Robber_CE>().Restart();
        meshGo.GetComponent<RobberMesh>().Restart();
        //skillsGo.SetActive(true);
        SpawnAgent();
        //Get info from controller

        timeSinceReachGoal = 0;
    }

    public void SpawnAgent()
    {

        Vector3 newPos = new Vector3(Random.Range(-spawnAreaAgents.bounds.extents.x + 1.5f, spawnAreaAgents.bounds.extents.x - 1.5f), initialPos.y, Random.Range(-spawnAreaAgents.bounds.extents.z + 1.5f, spawnAreaAgents.bounds.extents.z - 1.5f));
        
        tAgent.localPosition = newPos + spawnAreaAgents.transform.localPosition;
        


    }
    public int nextRoom;
    public float[] vel = new float[2];
    public float[] valuablePos;
    public float[] goalPos;
    public bool valPickedUp;
    public float hasVal;
    public float[] gateCoord;
    public float[] gateDir;
    public float[] cameraOn_1;
    public float[] detectRobber_1;
    public float[] cameraCoord1;
    public float[] cameraDir1;
    public float[] cameraOn_2;
    public float[] detectRobber_2;
    public float[] cameraCoord2;
    public float[] cameraDir2;
    public bool[] canUnlockGate = new bool[3];
    public float[] canDisCam_2;

    public override void CollectObservations(VectorSensor sensor)
    {
      /*  //Path that the agent would take
        foreach (int roomIdx in controller.museumMap.pathToTake)
        {
            sensor.AddObservation(roomIdx);//4
        }*/
        //Agent velocity

        sensor.AddObservation(Vector3.Dot(rbAgent.velocity, tAgent.forward));//1
        sensor.AddObservation(Vector3.Dot(rbAgent.velocity, tAgent.right));//1

        //Current room that the agent is in
        room = meshGo.GetComponent<RobberMesh>().currRoom;

        sensor.AddObservation(room);//1
        int k = 0;
        //  int k = 0;
        //Path that the agent would take
        
        //Find next room to visit -> instead of the room, how progress we've made. 
        //If visited 2 rooms, then observe 2.
      /*  nextRoom = -1;
        foreach (int roomIdx in controller.museumMap.pathToTake)
        {
            if (room == roomIdx)
            {
                //nextRoom = controller.museumMap.pathToTake[k + 1];
                nextRoom = k;

                break;
            }
            else
                k++;
        }
        sensor.AddObservation(nextRoom);//1
      */
        k = 0;
        //Gates are closed/open
        foreach (int gate in controller.museumMap.gatePath)
        {
            sensor.AddObservation(controller.museumMap.gates[gate].GetComponent<Gate>().permanentlyLocked);//3
            sensor.AddObservation(controller.museumMap.gates[gate].GetComponent<Gate>().gateOpen);//3
            sensor.AddObservation(relativeCoordinates(controller.museumMap.gates[gate].transform.position));//2*3=6
         //sensor.AddObservation(relativeDirDotProduct(controller.museumMap.gates[controller.museumMap.gatePath[i]].transform)); 
           //Locksmith can unlock a gate
            sensor.AddObservation(controller.museumMap.gates[gate].GetComponent<Gate>().canUnlockGate);//3
            canUnlockGate[k] = controller.museumMap.gates[gate].GetComponent<Gate>().canUnlockGate;
            k++;
        }
        //Alarm
         foreach (GameObject alarm in controller.museumMap.alarms)
        {

            sensor.AddObservation(alarm.GetComponent<Alarm>().alarm);//4
        }

       
        //Position of goal
        sensor.AddObservation(relativeCoordinates(controller.museumMap.goalPos));//2 


        //number of collected Valuables
        sensor.AddObservation(controller.numCollectedValuables);//1
        //agent has reach goal
        sensor.AddObservation(reachGoal);//1
        int roomNow = room;
        if (room == 0)
        {
            roomNow = controller.museumMap.pathToTake[0];
        }
        //2 Cameras in the room that the agent is currently into
       
           
            sensor.AddObservation(controller.cameraController.camerasFov[2*roomNow-2].cameraOn);//1
            sensor.AddObservation(controller.cameraController.camerasFov[2*roomNow - 2].detectRobberArr[idxRob]);//1
                                                                                                                //sensor.AddObservation(relativeDirDotProduct(controller.cameraController.camerasRotation[room * 2 - 1].transform));//2
            sensor.AddObservation(GetFOV(controller.cameraController.camerasRotation[2*roomNow - 2].transform));
            sensor.AddObservation(relativeCoordinates(controller.cameraController.camerasTr[2*roomNow - 2].position));//2
         //   sensor.AddObservation(relativeCoordinates(controller.cameraController.camerasRotation[2*roomNow - 2].angVelocity));

            sensor.AddObservation(controller.cameraController.camerasFov[roomNow *2-1].cameraOn);//1
            sensor.AddObservation(controller.cameraController.camerasFov[roomNow *2-1].detectRobberArr[idxRob]);//1
                                                                                                            // sensor.AddObservation(relativeDirDotProduct(controller.cameraController.camerasRotation[room * 2].transform));//2
            sensor.AddObservation(GetFOV(controller.cameraController.camerasRotation[roomNow * 2-1].transform));
            sensor.AddObservation(relativeCoordinates(controller.cameraController.camerasTr[roomNow  * 2-1].position));//2
        //    sensor.AddObservation(relativeCoordinates(controller.cameraController.camerasRotation[roomNow * 2-1].angVelocity));
            sensor.AddObservation(GetGuardData(controller.guardController.guards[roomNow - 1]));//7

        


        //Position of Valuables
        foreach (GameObject val in controller.museumMap.valuables)
        {
            sensor.AddObservation(relativeCoordinates(val.transform.position));//4*2=8
            sensor.AddObservation(val.GetComponent<Valuable>().pickedUp);//4

        }


        

        List <GroupController_CE.RobberInfo> teamList;


        teamList = controller.TeamRobbers;



        foreach (var info in teamList)
        {
            if (info.robber != this.transform.parent.gameObject && info.robber.activeInHierarchy)
            {
                
                    otherAgentsBuffer.AppendObservation(GetOtherAgentData(info));
            }

        }






    }
    public float[] GetFOV(Transform other)
    {
        var fow = new float[2];
        Vector3 vectorDiff = new Vector3(tAgent.position.x - other.position.x, 0f, tAgent.position.z - other.position.z);
        fow[0] = vectorDiff.magnitude/m_LocationNormalizationFactor;
        fow[1] = Vector3.Dot(other.forward, vectorDiff.normalized);

        return fow;
    }

    public float[] relativeCoordinates(Vector3 pos)
    {
        var relativeCoordinate = new float[2];
        Vector3 relativePosition = transform.InverseTransformPoint(pos);
        relativeCoordinate[0] = relativePosition.x / m_LocationNormalizationFactor;
        relativeCoordinate[1] = relativePosition.z / m_LocationNormalizationFactor;
        return relativeCoordinate;

    }
    public float[] relativeDirection(Vector3 frwd)
    {
        var relativeDirection = new float[2];
        Vector3 relativeDir = transform.InverseTransformDirection(frwd);
        relativeDirection[0] = relativeDir.x;
        relativeDirection[1] = relativeDir.z;
        return relativeDirection;
    }
    public float[] relativeDirDotProduct(Transform other)
    {
        var relativeDirection = new float[2];
        relativeDirection[0] = Vector3.Dot(rbAgent.velocity.normalized, other.forward);
        Vector3 vectorDiff = new Vector3(-tAgent.position.x + other.position.x, 0f, -tAgent.position.z + other.position.z);
        relativeDirection[1] = Vector3.Dot(rbAgent.velocity.normalized, vectorDiff.normalized);
        return relativeDirection;
    }
   
    private float[] GetOtherAgentData(GroupController_CE.RobberInfo info)
    {
        var otherAgentdata = new float[7];
        otherAgentdata[0] = info.skills; //skills of other agents
        var relativePosition = transform.InverseTransformPoint(info.robber.transform.position);
        otherAgentdata[1] = relativePosition.x / m_LocationNormalizationFactor;
        otherAgentdata[2] = relativePosition.z / m_LocationNormalizationFactor;

        otherAgentdata[3] = (float)info.robber.transform.GetChild(1).GetComponent<RobberMesh>().currRoom;
        otherAgentdata[4] = info.robber.transform.GetChild(1).GetComponent<RobberMesh>().reachGoal ? 1.0f : 0.0f;
        var relativeVelocity = transform.InverseTransformDirection(info.robber.GetComponent<Rigidbody>().velocity);
        otherAgentdata[5] = relativeVelocity.x / 5.78f;
        otherAgentdata[6] = relativeVelocity.z / 5.78f;
        return otherAgentdata;

    }
    private float[] GetGuardData(GuardFSM guard)
    {
        var guardData = new float[7];
        var relativePosition = transform.InverseTransformPoint(guard.transform.position); //skills of other agents

        guardData[0] = relativePosition.x / m_LocationNormalizationFactor;
        guardData[1] = relativePosition.x / m_LocationNormalizationFactor;

        guardData[2] = guard.GetComponent<GuardFSM>().detectRobber[idxRob] ? 1.0f : 0.0f;
        var relativeVelocity = transform.InverseTransformDirection(guard.agent.velocity);
        guardData[3] = relativeVelocity.x / 3.5f;
        guardData[4] = relativeVelocity.z / 3.5f;

        var fow = GetFOV(guard.transform);
        guardData[5] = fow[0];
        guardData[6] = fow[1];
        return guardData;

    }
    private float[] GetGateInfo(GameObject gate)
    {
        var gateData = new float[6];
        gateData[0] = gate.GetComponent<Gate>().gateOpen ? 1.0f : 0.0f;
        gateData[1] = skillsGo.GetComponent<LocksmithRobber>().canUnlockGate ? 1.0f : 0.0f;
        if (gate.GetComponent<Gate>().gateOpen)
        {
            var gatePos = relativeCoordinates(gate.transform.position);
            gateData[2] = gatePos[0];
            gateData[3] = gatePos[1];
            var gateDir = relativeDirDotProduct(gate.transform);
            gateData[4] = gateDir[0];
            gateData[5] = gateDir[1];
        }
        else
        {

            gateData[2] = 0f;
            gateData[3] = 0f;

            gateData[4] = 0f;
            gateData[5] = 0f;
        }

        return gateData;
    }
   /* private float[] GetCameraInfo(int cameraIdx)
    {
        var cameraData = new float[6];

        cameraData[0] = secCamFOV[cameraIdx].cameraOn ? 1.0f : 0.0f;//1-#2
        cameraData[1] = secCamFOV[cameraIdx].detectRobberArr[idxRob] ? 1.0f : 0.0f;

        if (secCamFOV[cameraIdx].cameraOn)
        {
            var camPos = relativeCoordinates(activeCam[cameraIdx].transform.position);
            var camDir = relativeDirDotProduct(activeCam[cameraIdx].transform.GetChild(0).GetChild(0));
            cameraData[2] = camPos[0];
            cameraData[3] = camPos[1];
            cameraData[4] = camDir[0];
            cameraData[5] = camDir[1];
        }
        else
        {
            cameraData[2] = 0f;
            cameraData[3] = 0f;
            cameraData[4] = 0f;
            cameraData[5] = 0f;
        }


        return cameraData;
    }*/
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        reachGoal = meshGo.GetComponent<RobberMesh>().reachGoal;





        if (!reachGoal)
        {
           // AddReward(-1f / (float)controller.MaxEnvironmentSteps);
            MoveAgent(actionBuffers);
        }
        else
        {
            timeSinceReachGoal++;
        }


    }
    //Actions to take that are best due to current observations
    public void MoveAgent(ActionBuffers actionBuffers)
    {

        var continuousActions = actionBuffers.ContinuousActions;
        var discreteActions = actionBuffers.DiscreteActions;

        Vector3 rotateDir = Vector3.zero;


        hmove = Mathf.Clamp(continuousActions[0], -1f, 1f);
        vmove = Mathf.Clamp(continuousActions[1], -1f, 1f);
        rotateAxis = (int)discreteActions[0];

        pickDropVal = (int)discreteActions[1];

        openGate = (int)discreteActions[2];

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }
        //Rotation
        if (rotateDir.magnitude != 0)
            tAgent.Rotate(rotateDir, Time.fixedDeltaTime * this.turnSpeed);
        //HANDLE XZ MOVEMENT
        Vector3 move = hmove * tAgent.right + vmove * tAgent.forward;
        if (move.magnitude != 0)
        {
            var vel = rbAgent.velocity.magnitude;
            float adjustedSpeed = Mathf.Clamp(regMoveSpeed - vel, 0, agentTerminalVel);
            rbAgent.AddForce(move.normalized * adjustedSpeed, ForceMode.VelocityChange);
            //  Debug.Log(rbAgent.velocity.magnitude);
        }


        if (m_IsDecisionStep)
        {


            switch (pickDropVal)
            {
                case 1:
                    robberGo.GetComponent<Robber_CE>().actPickUpValuable();
                    break;
               
            }
            if (openGate > 0)
            {
                // Debug.Log("openGate= " + openGate);
                skillsGo.GetComponent<LocksmithRobber>().actUnlockGateDelayed();
            }
            m_IsDecisionStep = false;
        }




    }


   

   
    public override void Heuristic(in ActionBuffers actionsOut)
    {






        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal2");
        continuousActionsOut[1] = Input.GetAxis("Vertical2");

        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.T))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.Y))
        {
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[2] = 1;
            // Debug.Log("pressed Q");
        }
        if (Input.GetKey(KeyCode.R))
        {
            discreteActionsOut[1] = 1;
            // Debug.Log("pressed Q");
        }
        




    }

    void FixedUpdate()
    {
        Debug.DrawRay(tAgent.position, tAgent.forward * 2f, Color.blue);

        // m_DashCoolDownReady = m_CubeMovement.dashCoolDownTimer > m_CubeMovement.dashCoolDownDuration;
        if (StepCount % 5 == 0)
        {
            m_IsDecisionStep = true;
            m_AgentStepCount++;
        }

    }






}
