using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToAstronautFirstPerson : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;
    public Camera OverviewCamera;

    public GameObject TextToHide;

    
    private float counter = 0;
    public float deadlineFirstSwitch = 12;
    public float deadlineSecondSwitch = 30;

    private bool FirstSwitchOccured = false;
    private bool SecondSwitchOccured = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Stay 60 Seconds with the Main Camera (= Overview over the Galaxy
        //switch to second camera
        if(counter < deadlineFirstSwitch || counter < (deadlineFirstSwitch+deadlineSecondSwitch))
        {
            counter += Time.deltaTime;
        }
        if(counter >= deadlineFirstSwitch && !FirstSwitchOccured)
        {
            //Switch camera
            OverviewCamera.enabled = false;
            firstPersonCamera.enabled = false;
            TextToHide.SetActive(true);
            thirdPersonCamera.enabled = true;
            //Prevents the camera from any further switches
            counter += Time.deltaTime;
            FirstSwitchOccured = true;
        }
        if(counter >= (deadlineFirstSwitch + deadlineSecondSwitch) && !SecondSwitchOccured)
        {
            OverviewCamera.enabled = false;
            firstPersonCamera.enabled = true;
            thirdPersonCamera.enabled = false;
            TextToHide.SetActive(false);
            //Prevents the camera from any further switches
            counter += Time.deltaTime;
            SecondSwitchOccured = true;
        }
        Debug.Log(counter);
    }
}
