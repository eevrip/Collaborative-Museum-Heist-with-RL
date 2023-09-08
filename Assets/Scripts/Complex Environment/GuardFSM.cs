using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;
public class GuardFSM : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Vector3 goalDest;
    [HideInInspector]
    public Transform alarm;
    private int maxTime;
    public int dtDetectRobbers;
   // public float dtDetectRobbers;
    public bool[] detectRobber = new bool[2];
    private int room;
    private int[] path;
    private bool generateNewPoint = true;
    private Transform checkPoints;
    bool[] visited;
    public float radiusFOV = 13f;
    [Range(0, 360)]
    public float fullAngleFOV = 90f;
    private GroupController_CE controller;
    public int currChPtIdx = -1;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private bool firstInitialize = true;
    public int timerStop = 0;
    Vector3 worlDest;
    public bool setAlarmOff = false;
    public bool pause = false;
    public LayerMask checkLayerMask;
    public GuardsController_CE guardController;
    void Awake()
    {
        controller = transform.root.GetComponent<GroupController_CE>();
        agent = GetComponent<NavMeshAgent>();
        checkPoints = transform.parent.GetChild(1);
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
        worlDest = this.transform.position;
        room = transform.parent.GetSiblingIndex() + 1;
        maxTime = controller.maxStepsInFOV;
        guardController = transform.parent.parent.gameObject.GetComponent<GuardsController_CE>();
        switch (room)
        {
            case 1:
                //  path = new int[22] { 0, 2, 4, 6, 0, 1, 8, 3, 4, 5, 8, 7, 6, 4, 3, 7, 6, 4, 2, 1, 5, 6 };
                //visited = new bool[22];
                path = new int[10] { 2, 3, 7, 6, 4, 2, 1, 5, 6, 0 };
                visited = new bool[10];
                break;
            case 2:
                path = new int[12] { 2, 3, 4, 1, 0, 5, 3, 2, 1, 4, 5, 0 };
                visited = new bool[12];
                break;
            case 3:
                path = new int[12] { 2, 3, 4, 1, 0, 5, 3, 2, 1, 4, 5, 0 };
                visited = new bool[12];
                break;
            case 4:
               // path = new int[8] {0, 1, 2, 3, 0, 3, 2, 1 };
                // path = new int[8] { 3, 2, 1, 0, 3, 2, 1,0};
                 path = new int[8] { 1, 2, 3, 0, 3, 2, 1, 0 };
                visited = new bool[8];
                break;
            default:
                path = new int[1];
                visited = new bool[1];
                break;
        }
        // alarm = transform.root.GetChild(1).GetChild(1).GetChild(room).GetChild(1);

    }
    public void Restart()
    {


        if (!firstInitialize)
        {
            agent = GetComponent<NavMeshAgent>();
            transform.localPosition = initialPos;
            transform.localRotation = initialRot;
            currChPtIdx = -1;

            worlDest = transform.position;
            if (agent)
            {
                MoveToPoint(worlDest);
                agent.speed = 3.5f;
            }
          //  dtDetectRobbers = 0f;
            dtDetectRobbers = 0;
            for (int i = 0; i < detectRobber.Length; i++)
                detectRobber[i] = false;
            for (int i = 0; i < visited.Length; i++)
                visited[i] = false;
            timerStop = 0;
            setAlarmOff = false;
            pause = false;
        }
        firstInitialize = false;

    }
    void MoveToPoint(Vector3 point)
    {
        agent.isStopped = false;
        agent.SetDestination(point);
    }

    public bool DetectRobber(GameObject rob)
    {
        Vector3 robPos = rob.transform.position;
        Vector3 difference = robPos - this.transform.position;
        Vector3 direction = difference.normalized;
        float dist = difference.magnitude;
        // Obstacle check
        RaycastHit hit;

        if (radiusFOV >= dist)
        {
            if (Physics.Raycast(transform.position, difference, out hit, radiusFOV, checkLayerMask))
            {


                if (hit.collider.tag == "RobberMesh")
                {

                    if (Vector3.Angle(direction, this.transform.forward) <= fullAngleFOV / 2f)
                    {
                        float currdt = 0f;
                        if (dist > 0.5f)
                            currdt = 2f * Vector3.Dot(direction, this.transform.forward) / dist;
                        else
                            currdt = 2f * Vector3.Dot(direction, this.transform.forward) / 0.5f;
                      //  Debug.Log(currdt + " D = " + dist);
                       // dtDetectRobbers = dtDetectRobbers + currdt;

                        Debug.DrawRay(transform.position, hit.collider.transform.position - transform.position, Color.red);

                        controller.RewardInGuardFOV();


                        return true;
                    }

                }


            }
        }




        return false;

    }


    void FixedUpdate()
    {


        Debug.DrawRay(transform.position, transform.forward * 2f, Color.blue);
        if (!pause)
        {
            if (agent)
            {



                //if (dtDetectRobbers >= maxTime)
                if (guardController.totalTimeDetectRobber >= controller.maxStepsInFOV && guardController.lastGuardToDetect == this) //unless no robbers are detected
                {
                    //MoveToPoint(alarm.position);
                    worlDest = checkPoints.GetChild(checkPoints.childCount - 1).position;
                    agent.speed = 7.5f;
                    MoveToPoint(worlDest);
                    setAlarmOff = true;
                 //   dtDetectRobbers = 0f;
                    dtDetectRobbers = 0;
                    currChPtIdx = -1;

                }
                else if (Vector3.Distance(this.transform.position, worlDest) <= 2f)
                {
                    timerStop++;
                    if (timerStop >= 60)
                    {



                        currChPtIdx++;
                        if (currChPtIdx < path.Length && !visited[currChPtIdx])
                        {

                            worlDest = checkPoints.GetChild(path[currChPtIdx]).position;
                            MoveToPoint(worlDest);

                        }
                        /* if (Vector3.Distance(this.transform.position, worlDest) <= 2f)
                         {
                             currChPtIdx++;
                         }*/
                        if (currChPtIdx == path.Length)
                            currChPtIdx = -1;
                        timerStop = 0;
                    }
                }

            }


            List<GroupController_CE.RobberInfo> teamList;


            teamList = controller.TeamRobbers;

            foreach (var info in teamList)
            {
                if (info.robber.activeInHierarchy)
                {
                    if (DetectRobber(info.robber.transform.GetChild(1).gameObject))
                    {
                         dtDetectRobbers++;
                        detectRobber[info.id] = true;
                        guardController.lastGuardToDetect = this;
                    }
                    else
                        detectRobber[info.id] = false;
                }

            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Alarm" && setAlarmOff)
        {
            controller.dataInt[4] = controller.m_ResetTimer; 
           
            other.gameObject.GetComponent<Alarm>().SetAlarmOff();
            setAlarmOff = false;
         //   dtDetectRobbers = 0f;
            dtDetectRobbers = 0;
            agent.isStopped = true;
            //agent.Stop();
            pause = true;
        }

    }
}

