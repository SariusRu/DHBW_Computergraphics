using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Diese Klasse beinhaltet alle Elemente und Methoden,
/// die für die Erzeugung der Moisture-Map mittels des Perlin-Noise-Algorithmus benötigt werden.
/// </summary>
public class PerlinNoiseAlg
{
    // width of texture
    private int width;

    //height of texture
    private int height;

    // Value storage for generated values and texture 
    private Texture2D moisture_map_text;

    // renderer for appling the generated moisture map
    private Renderer renderer;


    // Start is called before the first frame update
    public PerlinNoiseAlg(int width, int height, Renderer t_renderer)
    {
        renderer = t_renderer;
        this.width = width;
        this.height = height;
        renderer.material.SetTexture("_MoistureMap", getTexture(width, height));

    }

    Texture2D getTexture(int width, int height){
        Texture2D texture = new Texture2D(width, height);
        int i_rand_offset = Random.Range(0,1000);
        int j_rand_offset = Random.Range(0,1000);
        string debug = "MoistureMap: \n";
        for(int i = 0; i < height; i++){
            for(int j = 0; j < width; j++) {
                float sample = Mathf.PerlinNoise(((float)j / width) + j_rand_offset,
                                                 ((float)i / height) + i_rand_offset);
                debug += sample.ToString("F2") + " | ";
                texture.SetPixel(j, i, new Color(sample, sample, sample));
            }
            debug += "\n";
        }
        // Debug.Log(debug);
        texture.Apply();
        return texture;
    }
}
