using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquare : MonoBehaviour
{
    //amount of Faces. Amount of vertices equals textureWidth + 1;
    public int textureWidth = 256;
    //amount of Faces. Amount of vertices equals textureWidth + 1;
    public int textureHeigth = 256;

    public float maxHeight = 1f;

    private float[,] textureValues;

    // Start is called before the first frame update
    void Start()
    {
        textureValues = new float[textureWidth+1, textureHeigth+1];
        CornerValues();
        PerformStep();
    }

    private void PerformStep()
    {
        int halfLength;
        for(int sideLength = textureWidth; sideLength > 1; sideLength /= 2)
        {
            Debug.Log(sideLength);
            halfLength = sideLength / 2;
            DiamondStep(sideLength, halfLength);
            SquareStep(sideLength, halfLength);
        }
        
    }

    private void SquareStep(int sideLength, int halfLength)
    {
        for (int x = 0; x < textureWidth; x += sideLength)
        {
            for (int y = 0; y < textureWidth; y += sideLength)
            {
                Debug.Log("X, Y: " + x + " " + y);
                PerformSquareStep(x, y, sideLength, halfLength);
            }
        }
    }

    private void DiamondStep(int sideLength, int halfLength)
    {        
        for (int x = 0; x < textureWidth; x += sideLength)
        {
            for (int y = 0; y < textureWidth; y += sideLength)
            {
                Debug.Log("X, Y: " + x + " " + y);
                PerformDiamondStep(x, y, sideLength, halfLength);
            }
        }
    }

    private void PerformDiamondStep(int x, int y, int offset, int arrayOffset)
    {
        float avg = textureValues[x,y] + textureValues[x+ offset, y] + textureValues[x, y + offset] + textureValues[x+ offset, y+ offset];
        avg /= 4;
        textureValues[x+arrayOffset, y+arrayOffset] = Random.value + avg;
    }

    private void PerformSquareStep(int x, int y, int sideLength, int halfLength)
    {
        float average = textureValues[(x - halfLength + textureWidth - 1) % (textureWidth - 1), y];
        average += textureValues[(x + halfLength) % (textureWidth - 1), y];
        average += textureValues[x, (y + halfLength) % (textureWidth - 1)];
        average += textureValues[x, (y - halfLength + textureWidth - 1) % (textureWidth - 1)];
        average /= 4.0f;

        // Offset by a random value
        average += Random.value;

        // Set the height value to be the calculated average
        textureValues[x, y] = average;

        // Set the height on the opposite edge if this is
        // an edge piece
        if (x == 0)
        {
            textureValues[textureWidth, y] = average;
        }

        if (y == 0)
        {
            textureValues[x, textureWidth] = average;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CornerValues();
        PerformStep();
    }

    void CornerValues()
    {
        textureValues[0,0] = Random.value;
        textureValues[0,textureHeigth] = Random.value;
        textureValues[textureWidth, 0] = Random.value;
        textureValues[textureWidth, textureHeigth] = Random.value;
    }
}
