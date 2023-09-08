using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public bool gateOpen = false;//gate is open
    public bool permanentlyLocked;
    public bool canUnlockGate = false;
    public void UnlockGate() {
        
            gateOpen = true;
            transform.GetChild(0).gameObject.SetActive(false);
        // GetComponent<Collider>().enabled = false;
        Collider[] cols;

       cols = GetComponents<Collider>();

        for (int i = 0; i < cols.Length; i++)
            cols[i].isTrigger = true;
        int LayerGate = LayerMask.NameToLayer("Gate");
            gameObject.layer = LayerGate;
        gameObject.tag = "GateClosed";

    }
    public void LockGate()
    {
        int LayerGate = LayerMask.NameToLayer("Default");
        gameObject.layer = LayerGate;
        //GetComponent<Collider>().enabled = true;
        gateOpen = false;
        transform.GetChild(0).gameObject.SetActive(true);

        Collider[] cols;

        cols = GetComponents<Collider>();
        for(int i = 0;i<cols.Length;i++)
            cols[i].isTrigger = false;
       
        gameObject.tag = "Gate";
    }

    //The agents cannot see the gate and cannot open the gate
    public void PermanentlyLocked()
    {
        gateOpen = false;
        transform.GetChild(0).gameObject.SetActive(true);


        Collider[] cols;

        cols = GetComponents<Collider>();

        for (int i = 0; i < cols.Length; i++)
            cols[i].isTrigger = false;
        gameObject.tag = "Wall"; //so it's not detectable as gate
        int LayerGate = LayerMask.NameToLayer("Default");
        gameObject.layer = LayerGate;
        permanentlyLocked = true;
        canUnlockGate = false;
    }
    public void Restart()
    {
        StopAllCoroutines();
        int LayerGate = LayerMask.NameToLayer("Default");
        gameObject.layer = LayerGate;
        gameObject.tag = "Gate";
        //GetComponent<Collider>().enabled = true;
        gateOpen = false;
        transform.GetChild(0).gameObject.SetActive(true);
        Collider[] cols;

        cols = GetComponents<Collider>();

        for (int i = 0; i < cols.Length; i++)
            cols[i].isTrigger = false;
        permanentlyLocked = false;
        canUnlockGate = false;
    }

    public void UnlockGateCoroutine()
    {
        StartCoroutine(UnlockLockDelayedGate());
    }
    public void InteruptUnlockGateCoroutine()
    {
        StopAllCoroutines();
    }
    private IEnumerator UnlockLockDelayedGate()
    {
        //Needs 2 seconds to open the gate.
        yield return new WaitForSeconds(1f);
        //Gate is opened
       UnlockGate();
        //Remains open for 7 seconds
        yield return new WaitForSeconds(14f);
        //Gate is closed
        if (!permanentlyLocked)
           LockGate();
        else
            PermanentlyLocked();

    }
}
