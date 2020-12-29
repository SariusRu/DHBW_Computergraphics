using UnityEngine;
using System.Linq;

internal class DiamondSquareImproved
{
    private int globalWidth;
    private Renderer renderer;
    private float smoothness;
    private float[,] heightMap;

    public DiamondSquareImproved(int width, int heigth, Renderer renderer, float smoothness)
    {
        // The algorihmus works only with 2^n+1 (65,129,257...)
        // The minimum suitable width is determined and saved in the globalWidth
        SetWorkingWidth(width, heigth);

        //Problem: Algorithmus works only with 2^n+1
        this.renderer = renderer;
        this.smoothness = smoothness;

        SetCornerValues();
        RunDiamondSquare();
        this.renderer.material.SetTexture("_HeightMap", convertFloatArrayToTexture(width, heigth));
    }
    private void SetWorkingWidth(int width, int height)
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
        while (working <= comparing)
        {
            working = working * 2;
        }
        if (working % 2 == 0)
        {
            globalWidth = working + 1;
            Debug.Log("The actual width will be reduced by one.");
        }
        else
        {
            globalWidth = working;
        }
        Debug.Log("WorkingWidth is " + globalWidth);
    }

    private void RunDiamondSquare()
    {
        float range = 0.5f;
        int squareSide = 0;
        int halfSquareSide = 0;
        int x = 0;
        int y = 0 ;

        // While the side length is greater than 1
        for (squareSide = globalWidth - 1; squareSide > 1; squareSide = squareSide / 2)
        {
            halfSquareSide = squareSide / 2;

            // Run Diamond Step
            for (x = 0; x < globalWidth - 1; x += squareSide)
            {
                for (y = 0; y < globalWidth - 1; y += squareSide)
                {
                    SetAverageDiamond(x, y, squareSide, halfSquareSide, range);
                }
            }

            // Run Square Step
            for (x = 0; x < globalWidth - 1; x += halfSquareSide)
            {
                for (y = (x + halfSquareSide) % squareSide; y < globalWidth - 1; y += squareSide)
                {
                    SetAverageSquare(x, y, halfSquareSide, range);
                }
            }

            // Lower the random value range
            range -= range * 0.5f * smoothness;
        }

        return;
    }

    private void SetAverageSquare(int x, int y, int halfSide, float range)
    {
        // Get the average of the corners
        float avg = heightMap[(x - halfSide + globalWidth - 1) % (globalWidth - 1), y];
        avg += heightMap[(x + halfSide) % (globalWidth - 1), y];
        avg += heightMap[x, (y + halfSide) % (globalWidth - 1)];
        avg += heightMap[x, (y - halfSide + globalWidth - 1) % (globalWidth - 1)];
        avg = avg / 4;

        // Offset by a random value
        avg += (Random.value * (range * 2.0f)) - range;

        // Set the height value to be the calculated average
        heightMap[x, y] = avg;

        // Set the height on the opposite edge if this is
        // an edge piece
        if (x == 0)
        {
            heightMap[globalWidth - 1, y] = avg;
        }

        if (y == 0)
        {
            heightMap[x, globalWidth - 1] = avg;
        }
    }

    private void SetAverageDiamond(int x, int y, int squareSide, int halfSide, float range)
    {
        float avg = heightMap[x, y];
        avg += heightMap[x + squareSide, y];
        avg += heightMap[x, y + squareSide];
        avg += heightMap[x + squareSide, y + squareSide];
        avg = avg / 4;

        // Offset by a random value
        avg += (Random.value * (range * 2.0f)) - range;
        //Debug.Log(avg);
        heightMap[x + halfSide, y + halfSide] = avg;
    }

    private void SetCornerValues()
    {
        heightMap = new float[globalWidth, globalWidth];

        heightMap[0, 0] = Random.value;
        heightMap[globalWidth - 1, 0] = Random.value;
        heightMap[0, globalWidth - 1] = Random.value;
        heightMap[globalWidth - 1, globalWidth - 1] = Random.value;
    }

    private Texture2D convertFloatArrayToTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        float maxValue = getScaleRate();

        // GET X Start-Value
        int startValueX = (globalWidth - width) / 2;
        int endValueX = startValueX + width;

        // GET Y Start-Value
        int startValueY = (globalWidth - height) / 2;
        int endValueY = startValueY + height;

        for (int x = 0; x < globalWidth; x++)
        {
            for (int y = 0; y < globalWidth; y++)
            {
                texture.SetPixel(x, y, ColorFromValue(x, y, maxValue));
            }
        }
        texture.Apply();
        return texture;
    }


    // Sometimes the values of the average values are higher than 1, which casues problems when displaying it on th3e map.
    // The highest value within the heightmap values is looked up and then the scaling factor is calculated.
    private float getScaleRate()
    {
        // the scaling factor.
        float max = 0;
        for (int x = 0; x < globalWidth; x++)
        {
            for (int y = 0; y < globalWidth; y++)
            {
                if (heightMap[x, y] > max)
                {
                    max = heightMap[x, y];
                }
            }
        }
        return 1 / max;
    }

    private Color ColorFromValue(int x, int y, float maxValue)
    {
        //Debug.Log(heightMap[x, y]);
        float value = maxValue * heightMap[x, y];
        if(value > 1)
        {
            Debug.Log("Shit!");
        }
        return new Color(value, value, value);
    }
}