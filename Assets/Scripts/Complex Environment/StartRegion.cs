using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRegion : MonoBehaviour
{
    public int numberOfAgents = 2;

    public bool[] firstTime = new bool[2];
    public bool[] secondTime = new bool[2];
    public GroupController_CE controller;
   

    public void Restart()
    {
        controller = transform.root.GetComponent<GroupController_CE>();
        int LayerSR = LayerMask.NameToLayer("Default");
        gameObject.layer = LayerSR;

        numberOfAgents = 2;
        for (int i = 0; i < firstTime.Length; i++)
        {

            firstTime[i] = true;
            secondTime[i] = true;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "RobberMesh")
        {
            numberOfAgents--;
            
            if ( firstTime[other.gameObject.GetComponent<RobberMesh>().idx])
            {
               // Debug.Log("Exit SR");
                controller.AssignIndividualReward(0.3f, other.gameObject.GetComponent<RobberMesh>().idx);
                firstTime[other.gameObject.GetComponent<RobberMesh>().idx] = false;
                int LayerSR= LayerMask.NameToLayer("Ignore Raycast");
                gameObject.layer = LayerSR;
               
                other.gameObject.GetComponent<RobberMesh>().outOfSR = true;
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RobberMesh")
        {
           
            if (numberOfAgents != 2)
                numberOfAgents++;
            if (!firstTime[other.gameObject.GetComponent<RobberMesh>().idx] && secondTime[other.gameObject.GetComponent<RobberMesh>().idx])
            {
                // Debug.Log("Enter SR");
               // controller.AssignIndividualReward(-3.3f, other.gameObject.GetComponent<RobberMesh>().idx);
                secondTime[other.gameObject.GetComponent<RobberMesh>().idx] = false;
            }
              //  other.gameObject.GetComponent<RobberMesh>().currRoom = 0;

        }
    }

  

}
