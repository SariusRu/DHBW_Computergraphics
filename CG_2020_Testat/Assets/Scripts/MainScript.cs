using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    // width of the generated height map
    public int width = 65;

    // height of the generated height map
    public int height = 65;

    // granularity of values between 0 and 1 in the height map
    public int granularity = 255;

    // size of possible added random value
    public float smoothness = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        new DiamondSquareAlg(width, height, granularity, smoothness, renderer);
        new PerlinNoiseAlg(width, height, renderer);
    }
}
