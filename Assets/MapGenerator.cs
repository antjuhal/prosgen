using System;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Renderer textureRenderer;

    // This can be set to true in the Inspector to automatically re-render the map on any change.
    public bool autoUpdate;

    // Feel free to make new variables for more colors.
    public Color groundColor;
    public Color oceanColor;

    [Header("General Settings")]
    public int mapWidth;
    public int mapHeight;
    public int maxBridgeLength;
    public Vector2 offset;

    // See documentation for the different noise settings in the Noise.cs file.
    // Feel free to add Range sliders if you want to limit the values.
    [Header("Noise Settings")]
    public float noiseScale;
    public int octaves;
    [Range(0, 1)] public float persistence;
    public float lacunarity;
    public int seed;
    [Range(0, 1)] public float groundLimit;

    public TerrainType[] regions;
    public void GenerateMap()
    {
        // If you want to generate additional noisemaps, you can call the function many times with randomized seeds and different options.
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);
        
        // This actually draws the map so don't remove it.
        
        DrawNoiseMap(noiseMap);
    }

    /// <summary>
    /// There are used to clamp values in the inspector, since they break some parts of the map.
    /// </summary>
    void OnValidate()
    {
        if (mapWidth < 1) mapWidth = 1;
        if (mapHeight < 1) mapHeight = 1;
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
    }

    /// <summary>
    /// Luodaan v‰rit kartalle
    /// </summary>
    /// <param name="noiseMap"></param>
    public void DrawNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = noiseMap[x, y]; // haetaan iteroitavana olevan ruudun korkeus
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height) // inspectorissa aroviksi laitoin: bridge - 1, snow - 0.99999, mountain3 - 0.9, mountain2 - 0.8, mountain - 0.7, land - 0.6, sand - 0.45, water - 0.4
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour; // vaihetaan v‰ri‰ korkeusarvon perusteella
                        break;
                    }
                }
                if (currentHeight >= 0.7 && currentHeight <= 0.8)
                {
                    noiseMap = CreateBridge(noiseMap, x, y);
                }
            }
        }

        // These just set colors to the texture and apply it. No need to touch these.
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width * 0.1f, 1f, height * 0.1f);
    }

    /// <summary>
    /// Luo sillan kahden vuoren v‰lille
    /// </summary>
    /// <param name="noiseMap"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    float[,] CreateBridge(float[,] noiseMap, int x, int y)
    {
        try // en jaksanu rajottaa arrayn ulkopuolisia arvoja nii trycatchil menn‰‰
        {
            for (int i = 2; i < maxBridgeLength + 2; i++)
            {
                if (noiseMap[x + 1, y] < 0.7) // jos vuori loppuu oikealle
                {
                    if (noiseMap[x + i, y] >= 0.7 && noiseMap[x + i, y] <= 0.8) // ja lˆydet‰‰n myˆhemmin uusi vuori
                    {
                        for (int j = 0; j < i; j++)
                        {
                            noiseMap[x + j, y] = 1.00f; // asettaa jokaisen vuorien v‰lisen ruudun korkeudeks tasan 1 eli siit‰ tulee Bridge
                        }
                    }
                }
                if (noiseMap[x, y + 1] < 0.7) // jos vuori loppuu ylˆsp‰in
                {
                    if (noiseMap[x, y + i] >= 0.7 && noiseMap[x + i, y] <= 0.8) // ja lˆydet‰‰n ylh‰‰lt‰p‰in uusi vuori
                    {
                        for (int j = 0; j < i; j++)
                        {
                            noiseMap[x, y + j] = 1.00f; // asettaa jokaisen vuorien v‰lisen ruudun korkeudeks tasan 1 eli siit‰ tulee Bridge
                        }
                    }
                }

            }
        } catch { }
        return noiseMap;
    }
}
/// <summary>
/// Tutorialin mukaisesti luodaan inspectorissa erityyppist‰ maastoa, joilla on eri tasoja
/// </summary>
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

