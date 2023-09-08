using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUserControl : MonoBehaviour
{
    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam3;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Num1"))
        {
            cam1.SetActive(true);
            cam2.SetActive(false);
            cam3.SetActive(false);
        }
        if (Input.GetButtonDown("Num2"))
        {
            cam1.SetActive(false);
            cam2.SetActive(true);
            cam3.SetActive(false);
        }
        if (Input.GetButtonDown("Num3"))
        {
            cam1.SetActive(false);
            cam2.SetActive(false);
            cam3.SetActive(true);
        }
    }
}
