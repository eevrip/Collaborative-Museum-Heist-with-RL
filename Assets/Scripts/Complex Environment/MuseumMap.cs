using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumMap : MonoBehaviour
{
    public bool randomStartGoal;
    [Range (0 , 3)]
    public int gateToBeClosed = -1;
    public bool clockwisePath;
    public int[] pathToTake = new int[4];
    public int[] gatePath = new int[3];
    public int[] rooms = new int[4] { 1, 2, 3, 4 };
    public int[,] gateIdx = new int[4, 2] { { 1, 2 }, { 2, 3 }, { 3, 4 }, { 1, 4 } };
    public bool[] roomVisited = new bool[4] { false, false, false, false };
    public int startRoom;
    public int goalRoom;
    public bool restart;
    public GameObject[] gates;
    public GameObject[] valuables = new GameObject[4];//valuables are in order of Room 1, Room 2, Room 3, Room 4
    public Room[] roomsSc = new Room[4];
    private Transform wallsOpen;
    private Transform wallsClosed;
    private Transform gateParent;
    public GameObject prefabStartRegion;
    public GameObject prefabGoalRegion;
    public GameObject prefabValuable;
    
    public GameObject prefabWall;
    [HideInInspector]
    public GameObject tempStart;
    [HideInInspector]
    public GameObject tempGoal;
    [HideInInspector]
    public Collider spawnAreaAgents;
    private GameObject tempWall;
    private GroupController_CE controller;
    private int currGateIdx = -1;
    private int currRoom = -1;
    public GameObject[] LockGateT = new GameObject[3];
    public GameObject currGate;
    public GameObject currValuable;
    public int curRoomIdx = 0;
    [HideInInspector]
    public Vector3 startPos;
    [HideInInspector]
    public Vector3 goalPos;
    bool first_Initialize = true;
    public GameObject[] alarms = new GameObject[4];

    public bool insifdeIf = false;
    public int num = -1;
    public bool outOfSR = false;
    // Start is called before the first frame update
    void Awake()
    {
        gateParent = transform.GetChild(0).GetChild(0);

        wallsOpen = transform.GetChild(0).GetChild(5).GetChild(11);
        wallsClosed = transform.GetChild(0).GetChild(5).GetChild(12);
        gates = new GameObject[gateParent.childCount];

        controller = transform.root.GetComponent<GroupController_CE>();

        for (int i = 0; i < gates.Length; i++)
        {
            gates[i] = gateParent.GetChild(i).gameObject;
        }

        for (int i = 0; i < alarms.Length; i++)
        {
            alarms[i] = transform.GetChild(0).GetChild(i+1).GetChild(1).GetChild(0).gameObject;
        }
      //  CreateMap();
      //  currGate = gates[gatePath[0]];
      //  currValuable = valuables[pathToTake[0] - 1];
       
    }


    // Update is called once per frame
    void Update()
    {
        if (restart)
        {
            CreateMap();
           
            restart = false;
        }


    }
   
   
    public void Restart()
    {
       
        curRoomIdx = 0;
        for (int i = 0; i < gateParent.childCount; i++)
        {
            gates[i].SetActive(true);
            gates[i].GetComponent<Gate>().Restart();
            
        }
        for (int i = 0; i < wallsOpen.childCount; i++)
        {
            wallsOpen.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < wallsClosed.childCount; i++)
        {
            wallsClosed.GetChild(i).gameObject.SetActive(true);
        }

        if (tempGoal)
        {
            tempGoal.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            tempGoal.gameObject.GetComponent<Goal>().Restart();
        }
        if (tempStart)
        {
            tempStart.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        //    tempStart.gameObject.GetComponent<StartRegion>().numberOfAgents = 2;
            tempStart.gameObject.GetComponent<StartRegion>().Restart();
        }
        if (tempWall)
        {
            tempWall.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        
        for (int i = 0; i < alarms.Length; i++)
        {
            alarms[i].GetComponent<Alarm>().Restart();
        }
       
        for (int i = 0; i < roomVisited.Length; i++)
        {
            roomVisited[i] = false;
        }
           
            disableAllValuables();

        outOfSR = false;
    }
    public void CreateMap()
    { 
        
        Restart();
        GetStartGoalPosition();
        createRoomPath();
        createGatePath();
        for (int i = 0; i < rooms.Length; i++)
        {
            SpawnValuable(transform.GetChild(0).GetChild(rooms[i]).GetChild(0), i);
        }
        currGate = gates[gatePath[0]];
        currValuable = valuables[pathToTake[0] - 1];

    }
    //Choose random rooms to connect with either the start or goal region
    public void GetStartGoalPosition()
    {
        if (randomStartGoal)
        {
            int gateRnd = UnityEngine.Random.Range(0, 4);
            float rndNum = UnityEngine.Random.Range(0f, 1f);

            if (rndNum < 0.5f)
            {
                startRoom = gateIdx[gateRnd, 0];
                goalRoom = gateIdx[gateRnd, 1];
            }
            else
            {
                startRoom = gateIdx[gateRnd, 1];
                goalRoom = gateIdx[gateRnd, 0];
            }

            //gateRnd is replaced with a Wall
            replacedWithWall(gateRnd);
        }
        else
        {
            if (gateToBeClosed != -1 )
            {
                if (!clockwisePath)
                {
                    if (gateToBeClosed != 3)
                    {
                        startRoom = gateIdx[gateToBeClosed, 0];
                        goalRoom = gateIdx[gateToBeClosed, 1];
                    }
                    else {
                        startRoom = gateIdx[gateToBeClosed, 1];
                        goalRoom = gateIdx[gateToBeClosed, 0];
                    }
                }
                else
                {
                    if (gateToBeClosed != 3)
                    {
                        startRoom = gateIdx[gateToBeClosed, 1];
                        goalRoom = gateIdx[gateToBeClosed, 0];
                    }
                    else
                    {
                        startRoom = gateIdx[gateToBeClosed, 0];
                        goalRoom = gateIdx[gateToBeClosed, 1];
                    }
                }
                replacedWithWall(gateToBeClosed);
            }
        }
        //----------------------------------------------------
        //Spawn Start and Goal region in the environment
        //----------------------------------------------------
        wallsOpen.GetChild(startRoom - 1).gameObject.SetActive(true);
        wallsOpen.GetChild(goalRoom - 1).gameObject.SetActive(true);
        wallsClosed.GetChild(startRoom - 1).gameObject.SetActive(false);
        wallsClosed.GetChild(goalRoom - 1).gameObject.SetActive(false);


        startPos = SRGRposition(startRoom) + this.transform.position;
        goalPos = SRGRposition(goalRoom) + this.transform.position;
       
        if (!tempStart)
        {
            tempStart = Instantiate(prefabStartRegion, startPos, Quaternion.identity);
        }
        else
        {
            tempStart.transform.position = startPos;
        }
        if (!tempGoal)
        {
            tempGoal = Instantiate(prefabGoalRegion, goalPos, Quaternion.identity);
        }
        else
        {
            tempGoal.transform.position = goalPos;
        }
        float rotStart = SRGRrotation(startRoom);
        float rotGoal = SRGRrotation(goalRoom);


        tempStart.transform.Rotate(0f, rotStart, 0f, Space.Self);
        tempGoal.transform.Rotate(0f, rotGoal, 0f, Space.Self);
        tempStart.transform.SetParent(this.transform);
        tempGoal.transform.SetParent(this.transform);
        spawnAreaAgents = tempStart.GetComponent<Collider>();
        tempStart.gameObject.GetComponent<StartRegion>().Restart();
        tempGoal.gameObject.GetComponent<Goal>().Restart();

    }

    //The gate cannot be opened again. Permanetely locked.
    public void lockGate(GameObject gateToLock)
    {
        // gateToLock.GetComponent<Gate>().permanentlyLocked = true;
        gateToLock.GetComponent<Gate>().InteruptUnlockGateCoroutine();
        gateToLock.GetComponent<Gate>().PermanentlyLocked();
    }
    //Replace gate with a wall
    public void replacedWithWall(int gateNum)
    {
        if (!tempWall)
            tempWall = Instantiate(prefabWall, gates[gateNum].transform.position, Quaternion.identity);
        else
        {
            tempWall.transform.position = gates[gateNum].transform.position;
        }
        if (gateNum == 1 || gateNum == 3)
        {
            tempWall.transform.Rotate(0f, 90f, 0f, Space.Self);
        }
        tempWall.transform.SetParent(transform.GetChild(0).GetChild(5));
        gates[gateNum].SetActive(false);

    }
  
    public void BlockPath()
    {
        //Block path backwards when all agents arrive to a room.
        if (!roomVisited[curRoomIdx])
        {
          
                if (curRoomIdx == 0)
                {
                    num = tempStart.GetComponent<StartRegion>().numberOfAgents;
                    if (tempStart.GetComponent<StartRegion>().numberOfAgents == 0)
                    {
                        //createWall at starting point
                        wallsOpen.GetChild(startRoom - 1).gameObject.SetActive(false);

                        wallsClosed.GetChild(startRoom - 1).gameObject.SetActive(true);
                        roomVisited[curRoomIdx] = true;
                    outOfSR = true;
                 //   Debug.Log("Enter new room");
                  //  controller.AssignRobbersReward(0.2f); //both agents move to the next room
                      
                    }
                    

                }
                else if(roomsSc[curRoomIdx].numberOfAgents == 2 && roomsSc[curRoomIdx-1].numberOfAgents == 0)// && !gates[gatePath[curRoomIdx - 1]].GetComponent<Gate>().gateOpen)//only if gate is closed then move to the next room
                {
                    lockGate(gates[gatePath[curRoomIdx - 1]]);
                    LockGateT[curRoomIdx - 1] = gates[gatePath[curRoomIdx - 1]];
                    roomVisited[curRoomIdx] = true;
                    if(curRoomIdx<=2)
                        currGate = gates[gatePath[curRoomIdx]];
               // Debug.Log("Enter new room");
               // controller.AssignRobbersReward(0.2f);//both agents move to the next room
            }  
                
                currValuable = valuables[pathToTake[curRoomIdx]-1];
              
               if (roomVisited[curRoomIdx] && curRoomIdx < 3)
                    curRoomIdx++;
               
            
        
        }
    }


  

    //Finds rotation for the start region (SR) and the goal region (GR)
    public float SRGRrotation(int roomIdx)
    {
        float rot = 0f;
        if (roomIdx == 1 || roomIdx == 2)
        {
            rot = 180f;
        }

        return rot;
    }

    //Finds local position to spawn goal/start region
    public Vector3 SRGRposition(int roomIdx)
    {
        float spawnPosX = 0f;//13.5
        float spawnPosZ = 0f;//32
        if (roomIdx == 1 || roomIdx == 4)
        {
            spawnPosX = -13.5f;
        }
        else if (roomIdx == 2 || roomIdx == 3)
        {
            spawnPosX = 13.5f;
        }

        if (roomIdx == 1 || roomIdx == 2)
        {
            spawnPosZ = 32f;
        }
        else if (roomIdx == 3 || roomIdx == 4)
        {
            spawnPosZ = -32f;
        }
        return new Vector3(spawnPosX, 0f, spawnPosZ);
    }
    public void createRoomPath()
    {
        if ((startRoom == 1 && goalRoom == 4) || (startRoom == 4 && goalRoom == 1))
        {
            if (startRoom < goalRoom)
            {
                clockwise();
            }
            else
            {
                anticlockwise();
            }
        }
        else
        {

            if (startRoom > goalRoom)
            {
                clockwise();
            }
            else
            {
                anticlockwise();
            }
        }

        for (int i = 0; i < 4; i++)
        {
            //pathToTake =[1,4] coincides with the index of children
            roomsSc[i] = transform.GetChild(0).GetChild(pathToTake[i]).gameObject.GetComponent<Room>();

        }
    }
    public void clockwise()
    {//0 represents the 4th room
        int temp1 = (startRoom + 1) % 4;
        int temp2 = (startRoom + 2) % 4;
        if (temp1 == 0)
            temp1 = 4;
        if (temp2 == 0)
            temp2 = 4;
        pathToTake[0] = startRoom;
        pathToTake[1] = temp1;
        pathToTake[2] = temp2;
        pathToTake[3] = goalRoom;
    }
    public void anticlockwise()
    {
        int temp1 = (goalRoom + 2) % 4;
        int temp2 = (goalRoom + 1) % 4;
        if (temp1 == 0)
            temp1 = 4;
        if (temp2 == 0)
            temp2 = 4;
        pathToTake[0] = startRoom;
        pathToTake[1] = temp1;
        pathToTake[2] = temp2;
        pathToTake[3] = goalRoom;
    }
    public void createGatePath()
    {
        int k = 0;
        for (int i = 0; i < pathToTake.Length - 1; i++)
        {
            int[] asc = new int[2];
            if (pathToTake[i] < pathToTake[i + 1])
            {
                asc[0] = pathToTake[i];
                asc[1] = pathToTake[i + 1];
            }
            else
            {
                asc[0] = pathToTake[i + 1];
                asc[1] = pathToTake[i];
            }
            for (int j = 0; j < gateIdx.GetLength(0); j++)
            {
                if (gateIdx[j, 0] == asc[0] && gateIdx[j, 1] == asc[1])
                {
                    gatePath[k] = j;
                    break;
                }
            }
            k++;
        }
        currRoom = startRoom;
        currGateIdx = gatePath[0];
    }
    public int[] randPosVal = new int[4];

    public void SpawnValuable(Transform valSpawnArea, int valuableIdx)
    {
        randPosVal[valuableIdx] = UnityEngine.Random.Range(0, 4);
        valSpawnArea.GetChild(randPosVal[valuableIdx]).GetChild(0).gameObject.SetActive(true);
        valuables[valuableIdx] = valSpawnArea.GetChild(randPosVal[valuableIdx]).GetChild(0).gameObject;
    }
    //needs solution when episode ended and valuables are in the possesion of agent
    public void disableAllValuables()
    {
       for (int i = 0; i < valuables.Length; i++)
        {
         //   valuables[i].GetComponent<Valuable>().ResetPosition();
          //  valuables[i].SetActive(false);   
            transform.GetChild(0).GetChild(rooms[i]).GetChild(0).GetChild(randPosVal[i]).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(rooms[i]).GetChild(0).GetChild(randPosVal[i]).GetChild(0).gameObject.GetComponent<Valuable>().ResetColliderAndMesh();
        }
    }
   
}
