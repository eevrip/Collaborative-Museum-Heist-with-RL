using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robber_CE : MonoBehaviour
{
    
    Valuable val;
    public int numberOfValuables;
   
    [HideInInspector]
    public bool canPickUp = false;
    bool canDrop = false;
 
    public bool hasValuable;
    public bool canGive = false;
    [HideInInspector]
    public bool reachGoal = false;
    // public enum robberID { Locksmith,Tech, Other  }
    // public robberID id;
    private int id = 0;
    public MuseumMap museumMap;
    private GroupController_CE controller;
    
    private GameObject fellowRobber;

    public GameObject[] valuables;
    // Start is called before the first frame update
    void Awake()
    {


        controller = transform.root.GetComponent<GroupController_CE>();
        museumMap = controller.museumMap;
        valuables = new GameObject[controller.museumMap.valuables.Length];
        for (int i = 0; i < museumMap.valuables.Length; i++)
        {
            valuables[i] = museumMap.valuables[i];
        }

        id = transform.parent.GetSiblingIndex();
        
       
    }
    public void Restart()
    {
        StopAllCoroutines();
        canPickUp = false;
        canDrop = false;
       
        hasValuable = false;
        reachGoal = false;
        int LayerDef = LayerMask.NameToLayer("RobberMesh");

        this.transform.parent.GetChild(1).gameObject.layer = LayerDef; //change RobberMesh layer to Ignore Raycast, to not be detected by cameras
        val = null;
        numberOfValuables = 0;
        
    }
    // Update is called once per frame
    /*void Update()
    {
        //Lights
        if (canSwitchLight && Input.GetKeyDown(KeyCode.E))
        {
            if (lights.lightsOn)
            {
                if (!switchlightsOff)
                {
                    lights.SwitchLightsOff();
                    StartCoroutine(CooldownLightsOff());
                }
            }

            else if(lights.powerOn)
                lights.SwitchLightsOn();
        }
        //Pickup valuable
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (canPickUp && !valuable.pickedUp)
            {

                valuable.PickUpValuable(this.transform);
                canDrop = true;
                canPickUp = false;
            }


            else if (canDrop && valuable.pickedUp)
            { //This needs modification when add more than one robbers
                valuable.PlaceValuable();
                canPickUp = true;
                canDrop = false;
            }

        }

    }*/

    public void actPickUpValuable()
    {
        // Debug.Log("P "+canPickUp + ", D "+ canDrop + "," + val.pickedUp);

        if (canPickUp && !val.pickedUp)
        {
            // Debug.Log("Picked up");
            val.PickUpValuable();
            //numberOfValuables++;
           
            if (controller)
            {
                controller.numCollectedValuables = controller.numCollectedValuables + 1;//numberOfValuables;
              //  Debug.Log("Valuable");
                controller.AssignRobbersReward(0.375f);
                
            }
           
            canDrop = true;
            canPickUp = false;
            canGive = true;
            hasValuable = true;
            return;

        }

    }
   
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Valuable")
        {
            val = other.gameObject.GetComponent<Valuable>();
            canPickUp = true;
            //Debug.Log("Can Pick");
        }
        if (other.gameObject.tag == "Robber")
        {
            if (hasValuable)
            {
                canGive = true;
                fellowRobber = other.gameObject;
            }

        }

       

        if (other.gameObject.tag == "Goal")
        {
           // reachGoal = true;
            int LayerIgnRay = LayerMask.NameToLayer("Ignore Raycast");

            this.transform.parent.GetChild(1).gameObject.layer = LayerIgnRay; //change RobberMesh layer to Ignore Raycast, to not be detected by cameras

            
           /* if (controller)
            {
                //Debug.Log("Goal");
                controller.AssignIndividualReward(0.3f, id);
                controller.EndRobberEpisode();
                controller.SetGoalPosition(other.gameObject.transform.GetChild(other.gameObject.transform.childCount - 1).GetChild(id),id);


            }*/
            


        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Valuable")
        {
            val = null;
            canPickUp = false;
            // Debug.Log("Cannot pick");
        }

       

    }
   

}
