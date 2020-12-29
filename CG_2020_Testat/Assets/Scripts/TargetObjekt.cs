using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lets the directional lights always face the assigned object.
public class TargetObjekt : MonoBehaviour
{
    Vector3 offset = new Vector3(0, 5, 0);
    public GameObject target;
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform.position, offset);
    }
}
