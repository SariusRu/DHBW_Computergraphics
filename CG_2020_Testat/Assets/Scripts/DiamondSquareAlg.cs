using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Diese Klasse beinhaltet alle Elemente und Methoden, die für die Erzeugung der Heightmap mittels Diamond-Square-Algorithmus benötigt werden.
/// </summary>
public class DiamondSquareAlg
{
   // height map two dimensional value storage
    private float[,] height_map;
    private Texture2D height_map_text;
    private float smoothness;
    private int granularity;
    private int width;
    private int height;


    // renderer to apply the generated height as texture to a object
    private Renderer renderer;
    


    // Start is called before the first frame update
    // Splits non square dimensions into square dimensions 
    // Starts Diamond Square Algorithmen on the square dimensions
    // Applies the generated texture to the object
    public DiamondSquareAlg(int width, int height, int granularity, float smoothness, Renderer t_renderer)
    {
        // Initiate global objects
        renderer = t_renderer;
        this.smoothness = smoothness;
        this.granularity = granularity;
        this.width = width;
        this.height = height;

        height_map = new float[width, height];

        // Expand the given dimensions into a square and then perform Diamond-Square
        // If the width is bigger than or equal to the height:
        if(width >= height) {
            // If width is odd:
            if(width%2 == 1) {
                // Create float array for width by width dimension
                height_map = new float[width, width];
                // Perform diamondSquare for width by width dimension
                diamondSquare(0, 0, width);
                // Remove/Override unwanted bottom-height difference with -1.0f's
                for(int i = width-1; i >= height; i--) {
                    for(int j = 0; j < width; j++) {
                        height_map[i,j] = -1.0f;
                    }
                }
            // If width is not odd:
            } else {
                // Create float array for width+1 by width+1 dimension 
                // (adding the 1 to width for getting odd numbers)
                height_map = new float[width+1, width+1];
                // Perform diamondSquare for width+1 dimension
                diamondSquare(0, 0, width+1);
                // Remove/Override the column added by width+1 with 1.0f's
                for(int i = 0; i < width+1; i++) {
                    height_map[i,width] = -1.0f;
                }
                // Remove/Override unwanted bottom-height difference with -1.0f's
                for(int i = width; i >= height; i--) {
                    for(int j = 0; j < width; j++) {
                        height_map[i,j] = -1.0f;
                    }
                }
            }
        // If the height is bigger than the width:
        } else if (width < height) {
            // If the height is odd:
            if(height%2 == 1) {
                // Create float array for height by height dimension
                height_map = new float[height, height];
                // Perform diamondSquare for height dimension
                diamondSquare(0, 0, height);
                // Remove/Override unwanted right-width difference with -1.0f's
                for(int i = height-1; i >= width; i--) {
                    for(int j = 0; j < height; j++) {
                        height_map[j,i] = -1.0f;
                    }
                }         
            // If the height is not odd:
            } else {
                // Create float array for height+1 by height+1 dimension
                // (adding the 1 to height for getting odd numbers)
                height_map = new float[height+1, height+1];
                // Perform diamondSquare for height+1 dimension
                diamondSquare(0, 0, height+1);
                // Remove/Override the row added by height+1 with 1.0f's
                for(int i = 0; i < height+1; i++) {
                    height_map[height,i] = -1.0f;
                }
                // Remove/Override unwanted right-width difference with -1.0f's
                for(int i = height; i >= width; i--) {
                    for(int j = 0; j < height; j++) {
                        height_map[j,i] = -1.0f;
                    }
                }
            }
        }

        // Create Height-Map-Texture and apply Texture to GameObject 
        convertFloatArrayToTexture();
        renderer.material.SetTexture("_HeightMap", height_map_text);
    }

    // Executes the Diamond Square Algorithm on a square object of any odd size
    void diamondSquare(int x, int y, int width) {

        // Filter invalid starting conditions
        if(width <= 2 || width%2 == 0) return;

        // Generates initial values for ...
        // ... Upper-Left Corner:
        height_map[x, y] = getRandomNormalizedValue();
        // ... Upper-Right Corner:
        height_map[x + width - 1, y] = getRandomNormalizedValue();
        // ... Lower-Left Corner:
        height_map[x, y + width - 1] = getRandomNormalizedValue();
        // ... Lower-Right Corner:
        height_map[x + width - 1, y + width - 1] = getRandomNormalizedValue();
        
        // Initiates for Diamond-Step:     
        diamondStep(0, 0, width);
    }

    // Performs the diamond-step of the Diamond-Square Algorithm
    void diamondStep(int x, int y, int width) {

        // Top-Left Value:
        float avg = height_map[x, y];

        // Top-Left Value:
        avg += height_map[x + width - 1, y];

        // Top-Left Value:
        avg += height_map[x, y + width - 1];

        // Top-Left Value:
        avg += height_map[x + width - 1, y + width - 1];

        // Calculate the average value for center & add random value:
        avg = avg / 4;
        avg += (Random.Range(-avg, 1 - avg) * smoothness);

        // Add calculated value to height_map
        height_map[x + (width/2), y + (width/2)] = avg;

        // Make Square-Step
        squareStep(x, y, width);
    }

    // Performs the square-step of the Diamond-Square Algorithm
    void squareStep(int x, int y, int width) {
        
        // Calculate Average-Value for ...
        // ... Top-Edge Midpoint: 
        diamondAvg(( x + (width-1)/2), y, (width-1)/2);
        // ... Left-Edge Midpoint: 
        diamondAvg(x, ( y + (width-1)/2), (width-1)/2);
        // ... Right-Edge Midpoint: 
        diamondAvg(( x + (width-1)), ( y + (width-1)/2), (width-1)/2);
        // ... Bottom-Edge Midpoint:
        diamondAvg(( x + (width-1)/2), ( y + (width-1)), (width-1)/2);

        // Break contition recursion
        if(width-1 <= 2) return;

        // Recursivly perform Diamond-Step for ...
        // ... Upper-Left Corner:
        diamondStep(x,y,(width+1)/2);
        // ... Upper-Right Corner:
        diamondStep(x+((width-1)/2),y,(width+1)/2);
        // ... Lower-Left Corner:
        diamondStep(x,y+((width-1)/2),(width+1)/2);
        // ... Lower-Right Corner:
        diamondStep(x+((width-1)/2),y+((width-1)/2),(width+1)/2);
        
    }

    // Calcuate the average value for the Edge-Midpoints of the Square-Step
    void diamondAvg(int x, int y, int size) {
        
        // Local variables
        float avg = 0.0f;
        int count = 0;

        // Get Value from ...
        // ... Top-Point:
        if(y - size >= 0) avg += height_map[x, (y - size)]; count++;
        // ... Left-Point:
        if(x - size >= 0) avg += height_map[(x - size), y]; count++;
        // ... Right-Point: 
        if(x + size < height_map.GetLength(0)) avg += height_map[(x + size), y]; count++;
        // ... Bottom-Point: 
        if(y + size < height_map.GetLength(1)) avg += height_map[x, (y + size)]; count++;

        // Calculate the average value & add the random value
        avg = avg / count;
        avg += (Random.Range(-avg, 1 - avg) * smoothness);

        // add the calculated value to the height map
        height_map[x,y] = avg;
    }

    // Returns a random value in the range of 0 -> 1 with specified granularity
    float getRandomNormalizedValue() {
        return Random.Range(0, granularity)/(float)granularity;
    }


    // Converts a float array into a string for debugging
    string toString(float[,] array){
        string res = "[" + array.GetLength(0) + "|" + array.GetLength(1) + "]: \n";
        for(int i = 0; i < array.GetLength(0); i++) {
            for(int j = 0; j < array.GetLength(1); j++) {
                res += array[i,j].ToString("F2") + " | ";
            }
            res += "\n";
        }
        return res;
    }

    // Converts the float array height_map to a Texture 2D object
    void convertFloatArrayToTexture(){
        height_map_text = new Texture2D(width, height, TextureFormat.ARGB32, false);
        string res = "[" + height_map.GetLength(0) + "|" + height_map.GetLength(1) + "]: \n";
        for(int i = 0; i < height_map.GetLength(0); i++) {
            for(int j = 0; j < height_map.GetLength(1); j++) {
                if(height_map[i,j] != -1.0f) {
                    height_map_text.SetPixel(i,j,new Color(height_map[i,j],height_map[i,j],height_map[i,j],1.0f));
                    res += height_map[i,j].ToString("F2") + " | ";
                }
            }
            res += "\n";
        }
        //Debug.Log(res);
        height_map_text.Apply();
    }
}
