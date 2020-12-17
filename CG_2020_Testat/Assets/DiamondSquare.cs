using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Diese Klasse beinhaltet alle Elemente und Methoden, die für die Erzeugung der Heightmap mittels Diamond-Square-Algorithmus.
/// </summary>
/// <see cref="https://learn.64bitdragon.com/articles/computer-science/procedural-generation/the-diamond-square-algorithm"/>
public class DiamondSquare : MonoBehaviour
{
    int halfSide;
    int width = 65;

    // Speichert alle Höhenwerte der späteren Heightmap
    private float[,] textureValues;

    // Start is called before the first frame update
    void Start()
    {

        textureValues = new float[width, width];
        //Debug.Log("Setting Corner Values");
        CornerValues();
        diamondSquareAlg();
        //Debug.Log("Starting Steps");
        //PerformStep();
        //Debug.Log("Steps completed");
        SetTexture(ConvertToTexture());
    }

    void diamondSquareAlg()
    {
        int tileWidth = width - 1;
        while (tileWidth > 1)
        {
            halfSide = tileWidth / 2;
            //Diamond
            for (int x = 0; x < width - 1; x += tileWidth)
            {
                for (int y = 0; y < width - 1; y += tileWidth)
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
            for (int x = 0; x < width - 1; x += halfSide)
            {
                for (int y = (x + halfSide) % tileWidth; y < width - 1; y += tileWidth)
                {
                    float avg = textureValues[(x - halfSide + width - 1) % (width - 1), y];
                    avg += textureValues[(x + halfSide) % (width - 1), y];
                    avg += textureValues[x, (y + halfSide) % (width - 1)];
                    avg += textureValues[x, (y - halfSide + width - 1) % (width - 1)];

                    avg = avg / 4;
                    avg = Random.Range(0, 255);

                    textureValues[x, y] = avg;

                    //because the values wrap round, the left and right edges are equal, same with top and bottom
                    if (x == 0)
                    {
                        textureValues[width-1, y] = avg;
                    }
                    if (y == 0)
                    {
                        textureValues[x, width-1] = avg;
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
        Texture2D texture = new Texture2D(width, width);

        float maxValue = getScaleRate();

        for(int x = 0; x<width; x++)
        {
            for(int y = 0; y<width; y++)
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
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
        textureValues[0,width-1] = Random.Range(0, 255); ;
        textureValues[width-1, 0] = Random.Range(0, 255); ;
        textureValues[width-1, width-1] = Random.Range(0, 255); ;
        Debug.Log("Cornervalues set");
    }
}
