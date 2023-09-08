using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation_CE : MonoBehaviour
{
    private float rotTime;
    private bool rotateClckWs;
    public Vector3 angVelocity;
  
    private bool startNewRotation = true;
    [Range(-180, 180)]
    public float startingAngle;
    [Range(-180, 180)]
    public float endingAngle;
    private Vector3 pos_ti;
    void Awake()
    {
        //rotTime = UnityEngine.Random.Range(1f, 5f);
        rotTime = 5f;
        pos_ti = this.transform.forward;

    }
    // Update is called once per frame
    public void Restart()
    {
        StopAllCoroutines();
        startNewRotation = true;
        rotateClckWs = false;
        transform.localRotation = Quaternion.Euler(0f, startingAngle, 0f); 
        // rotTime = UnityEngine.Random.Range(2f, 5f);
    }
   void FixedUpdate()
    {if (startNewRotation && rotateClckWs)
        {

             StartCoroutine(Rotate(rotTime, -endingAngle + startingAngle));
          //  StartCoroutine(Rotate2(rotTime, endingAngle ,startingAngle));

        }
        else if (startNewRotation && !rotateClckWs)
        {

              StartCoroutine(Rotate(rotTime, endingAngle - startingAngle));
           // StartCoroutine(Rotate2(rotTime, startingAngle,endingAngle ));
        } 
        
        angVelocity = (this.transform.forward - pos_ti);///Time.fixedDeltaTime;
        pos_ti = this.transform.forward;
        
    }
    
    IEnumerator Rotate(float duration, float targetAngle)
    {
        startNewRotation = false;
        Quaternion initRotation = transform.localRotation;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.localRotation = initRotation * Quaternion.AngleAxis(timer * targetAngle / duration, Vector3.up);
            yield return null;
        }
        startNewRotation = true;
        rotateClckWs = !rotateClckWs;
    }
    IEnumerator Rotate2(float duration, float targetAngle, float startingAngle)
    {
        startNewRotation = false;
        Quaternion initRotation = transform.localRotation;
        float dtheta = Time.fixedDeltaTime*(targetAngle - startingAngle) / duration; //8
        float timer = 0f;
        float angleY = startingAngle; 
        
        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;
            
            transform.localRotation = Quaternion.Euler(0f,angleY + dtheta,0f);
            angleY = angleY + dtheta;
            yield return null;
        }
        startNewRotation = true;
        rotateClckWs = !rotateClckWs;
    }


}
