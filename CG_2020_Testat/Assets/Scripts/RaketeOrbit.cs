using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaketeOrbit : MonoBehaviour
{

    public GameObject rotationPoint;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(rotationPoint.transform.position, Vector3.up, -360.0F/duration * Time.deltaTime);
    }
}
