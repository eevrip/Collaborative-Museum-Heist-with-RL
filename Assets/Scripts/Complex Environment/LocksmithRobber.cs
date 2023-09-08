using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocksmithRobber : MonoBehaviour
{
    GameObject gate;
    public bool canUnlockGate = false;
    private GroupController_CE controller;

    int id;
   // private TestNavmesh t1;
  //  private TestNormal t2;
  //  private Training_take_1 t3;
    // Start is called before the first frame update
    void Awake()
    {
        controller = transform.root.gameObject.GetComponent<GroupController_CE>();
        //  t1 = this.transform.parent.GetChild(0).GetComponent<TestNavmesh>();
        //  t2 = this.transform.parent.GetChild(0).GetComponent<TestNormal>();
        //   t3 = this.transform.parent.GetChild(0).GetComponent<Training_take_1>();
        // t3 = this.transform.parent.parent.GetComponent<Training_take_1>(); Take 2
    }
    public void Restart()
    {   
        StopAllCoroutines();
        if (gate)
        {
            gate.GetComponent<Gate>().Restart();
            gate = null;
        }
        canUnlockGate = false;
     
    }
    // Update is called once per frame
    /* void Update()
     {
         if(Input.GetKeyDown(KeyCode.Keypad1))
             id = 1;
         else if (Input.GetKeyDown(KeyCode.Keypad2))
             id = 2;

         if (Input.GetKeyDown(KeyCode.Q) && id==2 && gate!=null)
         {//If space is pressed then alarm sets off

             StartCoroutine(UnlockGate(gate));
         }
     }*/
    public void actUnlockGateDelayed()
    {

        if (gate != null)
        {
            if (gate.GetComponent<Gate>().canUnlockGate)
            {
                if (!gate.GetComponent<Gate>().permanentlyLocked)
                {

                    //controller.AssignRobbersReward(0.5f / (float)controller.MaxEnvironmentSteps);
                    // StartCoroutine(UnlockGate(gate)); 
                    gate.GetComponent<Gate>().canUnlockGate = false;
                    gate.GetComponent<Gate>().UnlockGateCoroutine();
                    canUnlockGate = false;
                   
                }
            }
        }
    }
    
    public void actUnlockGate()
    {

        if (gate != null)
        {
            gate.GetComponent<Gate>().UnlockGate();
           
            canUnlockGate = false;
            
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gate")
        {
            if (!other.gameObject.GetComponent<Gate>().permanentlyLocked )
            {
                gate = other.gameObject;
                gate.GetComponent<Gate>().canUnlockGate = true;
            }
            // Debug.Log("GAte");
        }

        /*if (other.tag == "Valuable") //The goal area didn't exist in this training process. The goal was to reach the valuable
        {
            if (t1)
            {
                t1.AddReward(1f);
                t1.EndEpisode();

            }
            else if (t2)
            {
                t2.AddReward(1f);
                t2.EndEpisode();
            }
            else if (t3)
            {
                t3.AddReward(1f);
                t3.EndEpisode();
            }
        */
            //Debug.Log("Val");
        //}
        

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Gate")
        {
            if (!other.gameObject.GetComponent<Gate>().permanentlyLocked && !other.gameObject.GetComponent<Gate>().gateOpen)
            {
                gate = other.gameObject;
                gate.GetComponent<Gate>().canUnlockGate = true;
            }
            // Debug.Log("GAte");
        }
        if ( other.tag == "GateClosed")
        {

            other.gameObject.GetComponent<Gate>().canUnlockGate = false;
            gate = null;

        }
    }
        private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Gate" || other.tag == "GateClosed")
        {
          
            other.gameObject.GetComponent<Gate>().canUnlockGate = false;
            gate = null;

        }

    }



    private IEnumerator UnlockGate(GameObject other)
    {
        //Needs 2 seconds to open the gate.
        yield return new WaitForSeconds(1f);
        //Gate is opened
        other.GetComponent<Gate>().UnlockGate();
        //Remains open for 7 seconds
        yield return new WaitForSeconds(14f);
        //Gate is closed
        if(!other.GetComponent<Gate>().permanentlyLocked)
            other.GetComponent<Gate>().LockGate();
        else
            other.GetComponent<Gate>().PermanentlyLocked();

    }

}
