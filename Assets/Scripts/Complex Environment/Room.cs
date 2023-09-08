using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    //    [HideInInspector]
    public int numberOfAgents = 0;
    public int maxNumberOfAgents = 2;

    public int room;
    public GroupController_CE controller;
   //If a robber has visited the room for the first time
    public bool[] firstTime = new bool[2];

   void Awake()
    {
        controller = transform.root.GetComponent<GroupController_CE>();
        numberOfAgents = 0;
        for (int i = 0; i < firstTime.Length; i++)
        {

            firstTime[i] = true;
        }
    }

    public void Restart()
    {
        controller = transform.root.GetComponent<GroupController_CE>();
        numberOfAgents = 0;
        for (int i = 0; i <firstTime.Length; i++) {
           
            firstTime[i] = true;
         }

    }
    //Increase number of agents that exist in the room
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RobberMesh")
        {
            if (numberOfAgents != maxNumberOfAgents)
                numberOfAgents++;
           
                other.gameObject.GetComponent<RobberMesh>().currRoom = room;

            //If exit a room for the first time get a bonus. (entering the next room)
            if (firstTime[other.gameObject.GetComponent<RobberMesh>().idx])
            {
             //   Debug.Log("Enter Room");
                if (room != controller.museumMap.startRoom)
                {
                    controller.AssignIndividualReward(0.3f, other.gameObject.GetComponent<RobberMesh>().idx);
                    firstTime[other.gameObject.GetComponent<RobberMesh>().idx] = false;
                   
                }
            }



        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "RobberMesh")
        {
            if (numberOfAgents != 0)
                numberOfAgents--;
            
        }
    }
}
