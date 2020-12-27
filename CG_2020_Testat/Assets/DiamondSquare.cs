using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary>
/// Diese Klasse beinhaltet alle Elemente und Methoden, die für die Erzeugung der Heightmap mittels Diamond-Square-Algorithmus benötigt werden.
/// </summary>
/// <see cref="https://learn.64bitdragon.com/articles/computer-science/procedural-generation/the-diamond-square-algorithm"/>
public class DiamondSquare : MonoBehaviour
{
    int halfSide;
    public int width = 65;

    public int height = 65;

    private int workingWidth;

    // Speichert alle Höhenwerte der späteren Heightmap
    private float[,] textureValues;

    // Start is called before the first frame update
    void Start()
    {
        //Check if the width is even or odd. If the width is even, it is decreased by one.
        SetWorkingWidth();

        textureValues = new float[workingWidth, workingWidth];
        //Debug.Log("Setting Corner Values");
        CornerValues();
        diamondSquareAlg();
        //Debug.Log("Starting Steps");
        //PerformStep();
        //Debug.Log("Steps completed");
        SetTexture(ConvertToTexture());
    }

    private void SetWorkingWidth()
    {
        int comparing = 0;
        if (width > height)
        {
            comparing = width;
        }
        else
        {
            comparing = height;
        }
        int working = 2;
        while (working < comparing)
        {
            working = working * 2;
        }
        if (working % 2 == 0)
        {
            workingWidth = working + 1;
            Debug.Log("The actual width will be reduced by one.");
        }
        else
        {
            workingWidth = working;
        }
        Debug.Log("WorkingWidth is " + workingWidth );
    }

    void diamondSquareAlg()
    {
        int tileWidth = workingWidth - 1;
        while (tileWidth > 1)
        {
            halfSide = tileWidth / 2;
            //Diamond
            for (int x = 0; x < workingWidth - 1; x += tileWidth)
            {
                for (int y = 0; y < workingWidth - 1; y += tileWidth)
                {
                    float avg = textureValues[x,y];
                    avg += textureValues[x + tileWidth,y];
                    avg += textureValues[x,y + tileWidth];
                    avg += textureValues[x + tileWidth,y + tileWidth];
                    avg = avg / 4;
                    avg += Random.Range(0, 255);
                    textureValues[x + halfSide, y + halfSide] = avg;
                    Debug.Log("DiamondStep done");
                }
            }

            //SquareStep
            for (int x = 0; x < workingWidth - 1; x += halfSide)
            {
                for (int y = (x + halfSide) % tileWidth; y < workingWidth - 1; y += tileWidth)
                {
                    float avg = textureValues[(x - halfSide + workingWidth - 1) % (workingWidth - 1), y];
                    avg += textureValues[(x + halfSide) % (workingWidth - 1), y];
                    avg += textureValues[x, (y + halfSide) % (workingWidth - 1)];
                    avg += textureValues[x, (y - halfSide + workingWidth - 1) % (workingWidth - 1)];

                    avg = avg / 4;
                    avg = Random.Range(0, 255);

                    textureValues[x, y] = avg;

                    //because the values wrap round, the left and right edges are equal, same with top and bottom
                    if (x == 0)
                    {
                        textureValues[workingWidth - 1, y] = avg;
                    }
                    if (y == 0)
                    {
                        textureValues[x, workingWidth - 1] = avg;
                    }
                }
            }


            tileWidth = tileWidth / 2;
        }
   
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
        Debug.Log(textureValues);
        Texture2D texture = new Texture2D(width, height);

        float maxValue = getScaleRate();

        // GET X Start-Value
        int startValueX = (workingWidth - width)/2;
        Debug.Log(startValueX);
        int endValueX = startValueX + width;
        Debug.Log(endValueX);

        // GET Y Start-Value
        int startValueY = (workingWidth - height) / 2;
        Debug.Log(startValueY);
        int endValueY = startValueY + height;
        Debug.Log(endValueY);


        for (int x = startValueX; x < endValueX; x++)
        {
            for(int y = startValueY; y < endValueY; y++)
            {
                texture.SetPixel(x, y, ColorFromValue(x,y, maxValue));
            }
        }
        texture.Apply();
        return texture;
    }

    private float getScaleRate()
    {
        float max = 0;
        for (int x = 0; x < workingWidth; x++)
        {
            for (int y = 0; y < workingWidth; y++)
            {
                if(textureValues[x,y]>max)
                {
                    max = textureValues[x, y];
                }
            }
        }
        return 1/max;
    }

    private Color ColorFromValue(int x, int y, float maxValue)
    {
        Debug.Log(textureValues[x, y]);
        float value = maxValue * textureValues[x, y];
        return new Color(value, value, value);
    }

    void CornerValues()
    {
        textureValues[0,0] = Random.Range(0, 255); ;
        textureValues[0, workingWidth - 1] = Random.Range(0, 255); ;
        textureValues[workingWidth - 1, 0] = Random.Range(0, 255); ;
        textureValues[workingWidth - 1, workingWidth - 1] = Random.Range(0, 255); ;
        Debug.Log("Cornervalues set");
    }
}
