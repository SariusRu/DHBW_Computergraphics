﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    // width of the generated height map
    public int width = 65;


    // height of the generated height map
    public int height = 65;

    // size of possible added random value
    public float smoothness = 0.75f;

    // Runs the script used for the procedural terrain generation.
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        new DiamondSquareImproved(width, height, renderer, smoothness);
        new PerlinNoiseAlg(width, height, renderer);
    }
}
