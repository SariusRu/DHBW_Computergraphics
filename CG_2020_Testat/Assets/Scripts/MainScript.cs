using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{

    public int width = 65;
    public int height = 65;
    public int granularity = 255;
    // Start is called before the first frame update
    void Start()
    {
        new DiamondSquareAlg(width, height, granularity,GetComponent<Renderer>());
        new PerlinNoiseAlg(width, height, GetComponent<Renderer>());
    }
}
