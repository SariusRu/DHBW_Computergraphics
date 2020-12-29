using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateAround : MonoBehaviour
{
    public GameObject target;

    public float speedX = 20;
    public float speedY = 20;
    public float speedZ = 20;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(target.transform.position, Vector3.up, speedX * Time.deltaTime);
        transform.RotateAround(target.transform.position, Vector3.right, speedY * Time.deltaTime);
        transform.RotateAround(target.transform.position, Vector3.forward, speedZ * Time.deltaTime);
    }
}
