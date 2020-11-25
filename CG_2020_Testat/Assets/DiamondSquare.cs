using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Diese Klasse beinhaltet alle Elemente und Methoden, die für die Erzeugung der Heightmap mittels Diamond-Square-Algorithmus.
/// </summary>
public class DiamondSquare : MonoBehaviour
{
    //amount of Faces. Amount of vertices equals textureWidth + 1;
    public int width = 256;
    
    //amount of Faces. Amount of vertices equals textureWidth + 1;
    public int height = 256;

    // Definiert die maximale Werten, die die Heightmap erreichen kann (WIP)
    public float maxHeight = 1f;

    // Speichert alle Höhenwerte der späteren Heightmap
    private float[,] textureValues;

    // Start is called before the first frame update
    void Start()
    {
        
        textureValues = new float[width+1, height+1];
        //Debug.Log("Setting Corner Values");
        CornerValues();
        //Debug.Log("Starting Steps");
        PerformStep();
        //Debug.Log("Steps completed");
        SetTexture(ConvertToTexture());
    }

    private void SetTexture(Texture2D heightMap)
    {
        Renderer renderer = GetComponent<Renderer>();

        //Make sure to enable the Keywords
        renderer.material.mainTexture = ConvertToTexture();
        //Set the Normal map using the Texture you assign in the Inspector
        //renderer.material.SetTexture("_BumpMap", m_Normal);
        //Set the Metallic Texture as a Texture you assign in the Inspector
        //renderer.material.SetTexture("_MetallicGlossMap", m_Metal);
    }

    private Texture2D ConvertToTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for(int x = 0; x<width; x++)
        {
            for(int y = 0; y<height; y++)
            {
                texture.SetPixel(x, y, ColorFromValue(x,y));
            }
        }
        texture.Apply();
        return texture;
    }

    private Color ColorFromValue(int x, int y)
    {
        Debug.Log(textureValues[x, y]);
        return new Color(textureValues[x, y], textureValues[x, y], textureValues[x, y]);
    }

    private void PerformStep()
    {
        int halfLength;
        for(int sideLength = width; sideLength > 1; sideLength /= 2)
        {
            //Debug.Log(sideLength);
            halfLength = sideLength / 2;
            //Debug.Log("Diamond Step");
            DiamondStep(sideLength, halfLength);
            //Debug.Log("Square Step");
            SquareStep(sideLength, halfLength);
        }
        
    }

    private void SquareStep(int sideLength, int halfLength)
    {
        for (int x = 0; x < width; x += sideLength)
        {
            for (int y = 0; y < width; y += sideLength)
            {
                //Debug.Log("Square Step: X, Y: " + x + " " + y);
                PerformSquareStep(x, y, sideLength, halfLength);
            }
        }
    }

    private void DiamondStep(int sideLength, int halfLength)
    {        
        for (int x = 0; x < width; x += sideLength)
        {
            for (int y = 0; y < width; y += sideLength)
            {
                //Debug.Log("Square Step: X, Y: " + x + " " + y);
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
        float average = textureValues[(x - halfLength + width - 1) % (width - 1), y];
        average += textureValues[(x + halfLength) % (width - 1), y];
        average += textureValues[x, (y + halfLength) % (width - 1)];
        average += textureValues[x, (y - halfLength + width - 1) % (width - 1)];
        average /= 4.0f;

        // Offset by a random value
        average += Random.value;

        // Set the height value to be the calculated average
        textureValues[x, y] = average;

        // Set the height on the opposite edge if this is
        // an edge piece
        if (x == 0)
        {
            textureValues[width, y] = average;
        }

        if (y == 0)
        {
            textureValues[x, width] = average;
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    CornerValues();
    //   PerformStep();
    //}

    void CornerValues()
    {
        textureValues[0,0] = Random.value;
        textureValues[0,height] = Random.value;
        textureValues[width, 0] = Random.value;
        textureValues[width, height] = Random.value;
    }
}
