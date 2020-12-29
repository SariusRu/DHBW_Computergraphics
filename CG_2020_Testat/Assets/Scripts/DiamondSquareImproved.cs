using UnityEngine;
using System.Linq;

internal class DiamondSquareImproved
{
    private int globalWidth;
    private Renderer renderer;
    private float smoothness;
    private float[,] heightMap;

    /// <summary>
    /// Main functuion for the Diamond-Square-Alg
    /// </summary>
    /// <param name="width"></param>
    /// <param name="heigth"></param>
    /// <param name="renderer"></param>
    /// <param name="smoothness"></param>
    public DiamondSquareImproved(int width, int heigth, Renderer renderer, float smoothness)
    {
        // The algorihmus works only with 2^n+1 (65,129,257...)
        // The minimum suitable width is determined and saved in the globalWidth
        SetWorkingWidth(width, heigth);

        // The renderer of the scene, used to store the Heightmap-Texture into the file.
        this.renderer = renderer;

        // Sets the smoothness of the elements
        this.smoothness = smoothness;

        // Sets the value of the 4 corner-values with random values in the range of 0..1
        SetCornerValues();

        // Runs the Diamond Square Alg.
        RunDiamondSquare();

        // Saves the heightmap into the Material.
        this.renderer.material.SetTexture("_HeightMap", ConvertFloatArrayToTexture(width, heigth));
    }

    /// <summary>
    /// Sets the "GlobalWidth"-Parameter to the minimum needed size. 
    /// </summary>
    /// <param name="width">the width as set by the user</param>
    /// <param name="height">the height as set by the user</param>
    private void SetWorkingWidth(int width, int height)
    {
        // Determine the bigger side of the width and height.
        // This value is used for the rest of the script.
        int comparing;
        if (width > height)
        {
            comparing = width;
        }
        else
        {
            comparing = height;
        }

        // Get the n-factor of 2^n for the value
        float factor = Mathf.Log(comparing - 1, 2);

        //Calculate the next 2^n-value for the comparing-value
        int workingValue = Mathf.FloorToInt(factor) + 1;
        workingValue = (int)Mathf.Pow(2, workingValue);

        // Make the value odd (needed for the Alg)
        if (workingValue % 2 == 0)
        {
            globalWidth = workingValue + 1;
        }
        else
        {
            globalWidth = workingValue;
        }
    }

    private void RunDiamondSquare()
    {
        float range = 0.5f;

        // variables used for running trough the values of the texture
        int algSquareDim, halfAlgSquareDim, x, y;

        // Run the Diamond-Square-Alg for as long as the squareSide is bigger than 1.
        // Without this check the "squareSide" would be divided by 2, causing the script to crash.
        for (algSquareDim = globalWidth - 1; algSquareDim > 1; algSquareDim = algSquareDim / 2)
        {
            //value used for setting the center-value of the Diamond_Step
            //https://en.wikipedia.org/wiki/Diamond-square_algorithm#/media/File:Diamond_Square.svg,
            //PerformDiamondStep
            halfAlgSquareDim = algSquareDim / 2;

            // For every Square established by the outer for-loop,
            // run the Diamond step and the Square-step.
            // Increasing the x-value/y-value by the squareSide-value,
            // the squares are used to run through themselfes.

            // Run Diamond Step
            for (x = 0; x < globalWidth - 1; x += algSquareDim)
            {
                for (y = 0; y < globalWidth - 1; y += algSquareDim)
                {
                    // Calculates the average value of the 4 corners and adds a random value.
                    SetAverageDiamond(x, y, algSquareDim, halfAlgSquareDim, range);
                }
            }

            // Run Square Step
            for (x = 0; x < globalWidth - 1; x += halfAlgSquareDim)
            {
                for (y = (x + halfAlgSquareDim) %
                algSquareDim; y < globalWidth - 1; y += algSquareDim)
                {
                    SetAverageSquare(x, y, halfAlgSquareDim, range);
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

    /// <summary>
    /// Calculates the average value of the 4 corners and adds a random value atop of that.
    /// </summary>
    /// <param name="x">Current X-Position within the texture</param>
    /// <param name="y">Current Y-Position within the texture</param>
    /// <param name="algSquareDim">The width/height of the current square</param>
    /// <param name="halfAlgSquareDim">half the size of the squareSide-value,
    /// could be calculated within the function easily, however it's easier and faster to calc
    /// it once outside.</param>
    /// <param name="range">A parameter for the random value</param>
    private void SetAverageDiamond(int x,
                                   int y,
                                   int algSquareDim,
                                   int halfAlgSquareDim,
                                   float range)
    {
        //Calculate average values
        float avg = heightMap[x, y];
        avg += heightMap[x + algSquareDim, y];
        avg += heightMap[x, y + algSquareDim];
        avg += heightMap[x + algSquareDim, y + algSquareDim];
        avg = avg / 4;

        // Add the random value using the range as paramater and to make sure it
        // is as small as possible.
        avg += (Random.value * (range * 2.0f)) - range;

        // Save the value within the float-Array at the position determined by
        // x
        // y
        // algSquareDim
        // and halfAlgSquareDim
        heightMap[x + halfAlgSquareDim, y + halfAlgSquareDim] = avg;
    }

    /// <summary>
    /// Sets the value of the 4 corner-values with random values in the range of 0..1
    /// </summary>
    private void SetCornerValues()
    {
        // Initialize the heightmap-float-array
        heightMap = new float[globalWidth, globalWidth];

        // Random.value will create values within 0..1
        heightMap[0, 0] = Random.value;
        heightMap[globalWidth - 1, 0] = Random.value;
        heightMap[0, globalWidth - 1] = Random.value;
        heightMap[globalWidth - 1, globalWidth - 1] = Random.value;
    }

    /// <summary>
    /// Converts the flaot-array into a texture usable by the Unity-Renderer.
    /// All values will be scaled down to be within the range of 0..1
    /// </summary>
    /// <param name="width">the Texture-Width as set by the user</param>
    /// <param name="height">The Texture-Heigth as set by the user</param>
    /// <returns></returns>
    private Texture2D ConvertFloatArrayToTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);


        // Used for the scaling of the procedural generated values. Somethimes the values are bigger
        // than 1, resulting in problems when displaying the values on the displacement-map
        float maxValue = getScaleRate();


        // Cuts down the texture to the size set by the user...
        // GET X Start-Value
        int startValueX = (globalWidth - width) / 2;
        int endValueX = startValueX + width;

        // GET Y Start-Value
        int startValueY = (globalWidth - height) / 2;
        int endValueY = startValueY + height;

        for (int x = startValueX; x < endValueX; x++)
        {
            for (int y = startValueY; y < endValueY; y++)
            {
                texture.SetPixel(x - startValueX, y - startValueY, ColorFromValue(x, y, maxValue));
            }
        }
        texture.Apply();
        return texture;
    }


    /// <summary>
    /// Sometimes the values of the average values are higher than 1, which casues
    /// problems when displaying it on th3e map. The highest value within the heightmap values
    /// is looked up and then the scaling factor is calculated.
    /// </summary>
    /// <returns>The scaling-factor. Mutiplying all values within the array by this value will
    ///make sure no values higher than 1 are left.</returns>
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

    /// <summary>
    /// Converts the float-value within the float-array into a color.
    /// The value is scaled beforehand.
    /// </summary>
    /// <param name="x">X-Position within the array</param>
    /// <param name="y">Y-Position within the array</param>
    /// <param name="scalingFactor">the scaling-Facotr</param>
    /// <returns></returns>
    private Color ColorFromValue(int x, int y, float scalingFactor)
    {
        float value = scalingFactor * heightMap[x, y];
        return new Color(value, value, value);
    }
}