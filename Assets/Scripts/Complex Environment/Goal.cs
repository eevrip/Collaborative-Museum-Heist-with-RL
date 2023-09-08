using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int numberOfAgents = 0;
    public int maxNumberOfAgents = 2;
    public bool[] firstTime = new bool[2];
    public GroupController_CE controller;
    // Start is called before the first frame update

    public void Restart()
    {
        controller = transform.root.GetComponent<GroupController_CE>();

        numberOfAgents = 0;
        for (int i = 0; i < firstTime.Length; i++)
        {

            firstTime[i] = true;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RobberMesh")
        {
            if (numberOfAgents != maxNumberOfAgents)
                numberOfAgents++;

            if (firstTime[other.gameObject.GetComponent<RobberMesh>().idx])
            {
              //  Debug.Log("enter goal room");
                other.gameObject.GetComponent<RobberMesh>().reachGoal = true;
                controller.AssignIndividualReward(0.3f, other.gameObject.GetComponent<RobberMesh>().idx);
                //Debug.Log("goal");
               
                controller.SetGoalPosition(transform.GetChild(transform.childCount - 1).GetChild(other.gameObject.GetComponent<RobberMesh>().idx), other.gameObject.GetComponent<RobberMesh>().idx);
             controller.EndRobberEpisode();
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
