using UnityEngine;

public class Noise
{
    /// <summary>
    /// Generates a Perlin noise map where each value of the 2D array is a semi-randomized value between 0.0 and 1.0.
    /// </summary>
    /// <param name="mapWidth">Width of noise map</param>
    /// <param name="mapHeight">Height of noise map</param>
    /// <param name="seed">A random seed used to generate the map</param>
    /// <param name="scale">Scale of the noise map. A higher value "zooms" into the map. This value should always be more than 1 to avoid weirdness.</param>
    /// <param name="octaves">The amount of additional, randomized levels of noise that is added to the map.</param>
    /// <param name="persistance">The strength of these additional octaves. 0 means no effect, 1 is full effect.</param>
    /// <param name="lacunarity">When set to 1, the additional octaves don't add additional detail to the map. With greater numbers, the added octaves produce smaller and smaller detail.</param>
    /// <param name="offset">Set this value if you want to "move" around on the generated noise map.</param>
    /// <returns></returns>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-10000, 10000) + offset.x;
            float offsetY = prng.Next(-10000, 10000) + offset.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }


        if (scale <= 0)
        {
            scale = 0.001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2;
        float halfHeight = mapHeight / 2;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }

        }

        return noiseMap;
    }
}