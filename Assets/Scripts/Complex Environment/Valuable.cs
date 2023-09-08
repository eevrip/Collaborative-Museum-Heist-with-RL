using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valuable : MonoBehaviour
{
    Transform ogParent;
    public bool pickedUp = false;
    private Vector3 initialPos;
    // Start is called before the first frame update
    void Awake() {
        ogParent = this.transform.parent;
        initialPos = this.transform.localPosition;
        //Debug.Log(ogParent.name);
    }
    public void PickUpValuable(Transform other)
    {
       
        this.transform.SetParent(other, true);
        this.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        //this.transform.localPosition = new Vector3(0f,1.5f,0f);
        this.gameObject.GetComponent<Collider>().isTrigger = true;
        pickedUp = true;
        int LayerVal = LayerMask.NameToLayer("Valuable");
        gameObject.layer = LayerVal;
        //other.gameObject.GetComponent<Robber>.hasValuable = true;
        // Debug.Log("Pick" + other.name);
    }
    public void PlaceValuable()
    { 
        
        this.transform.SetParent(ogParent, true);
        //this.gameObject.GetComponent<Collider>().enabled = true;
        this.gameObject.GetComponent<Collider>().isTrigger = false;
        // Debug.Log("Place" + ogParent.name);
        pickedUp = false;
        int LayerVal = LayerMask.NameToLayer("Default");
        gameObject.layer = LayerVal;

    }
    //This method would disappear the mesh of valuable instead of changing the position of valuable
    public void PickUpValuable()
    {

        this.gameObject.GetComponent<Collider>().isTrigger = true;
        pickedUp = true;
        int LayerVal = LayerMask.NameToLayer("Valuable");
        gameObject.layer = LayerVal;
        transform.GetChild(0).gameObject.SetActive(false);
    }
    public void ResetColliderAndMesh()
    {
        this.gameObject.GetComponent<Collider>().isTrigger = false;
        transform.GetChild(0).gameObject.SetActive(true);
        pickedUp = false;
        int LayerVal = LayerMask.NameToLayer("Default");
        gameObject.layer = LayerVal;
    }
    public void ResetPosition()
    {
        this.transform.SetParent(ogParent, true);
         this.gameObject.GetComponent<Collider>().isTrigger = false;
        //this.gameObject.GetComponent<Collider>().enabled = true;
        this.transform.localPosition = initialPos;
        pickedUp = false;
        int LayerVal = LayerMask.NameToLayer("Default");
        gameObject.layer = LayerVal;
    }
    public void StateCumulativeTraining(Transform other)
    {
        this.transform.SetParent(other, true);
        this.transform.localPosition = new Vector3(0f,1.5f,0f);
        this.gameObject.GetComponent<Collider>().isTrigger = true;
        pickedUp = true;
        int LayerVal = LayerMask.NameToLayer("Valuable");
        gameObject.layer = LayerVal;

        // Debug.Log("Pick" + other.name);
    }
}
