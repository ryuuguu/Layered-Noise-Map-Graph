/*
 * HeightmapGenerator.cs
 * Created by Arian - GameDevBox
 * YouTube Channel: https://www.youtube.com/@GameDevBox
 *
 * 🎮 Want more Unity tips, tools, and advanced systems?
 * 🧠 Learn from practical examples and well-explained logic.
 * 📦 Subscribe to GameDevBox for more game dev content!
 *
 * This ScriptableObject defines settings for generating heightmaps using various noise types.
 * 
 * Noise Types Explained:
 * - Perlin: Classic smooth gradient noise, good for natural-looking terrains.
 * - Ridged: Variation of Perlin noise, often using absolute values or inversion to create sharp ridges.
 * - Cellular (Worley Noise): Produces cell-like patterns, useful for natural textures like stones or scales.
 * - Turbulence: Combines multiple frequencies of noise (often absolute values) to create flow-like patterns.
 * - DiamondSquare: A fractal terrain generation algorithm, good for heightmap fractal details.
 * - FFT_Erosion: Applies erosion simulation based on Fast Fourier Transform, simulating natural terrain weathering.
 * - FBM (Fractal Brownian Motion): Summation of multiple noise octaves to add fractal detail.
 * - DomainWarp: Warps noise input coordinates to create complex, warped noise patterns.
 * - Simplex: Faster and smoother alternative to Perlin noise, reducing directional artifacts.
 * - Value Noise: Interpolated random values producing smoother randomness.
 * - Billow: Soft ridged noise, similar to ridged but smoother and more cloud-like.
 * - WhiteNoise: Pure random noise without spatial coherence, useful for dithering or randomness.
 * - CurlNoise: Vector field noise producing swirling, curl-like patterns, often for fluid or wind simulation.
 */


 /*
 * note this code has been modified from GameDevBox's original code so will not match what is in his video
 * ConeConst: nonrandom cone
 * DomeConst: nonrandom dome
 * PyramidConst : nonrandom Pyramid
 */

using UnityEngine;

namespace AnthillPlan.LayeredNM {

    public class HeightmapGenerator {
        public enum HeightmapType {
            Perlin = 0,
            Ridged = 1,
            Cellular = 2,
            Turbulence = 3,
            DiamondSquare = 4,
            FFT_Erosion = 5,
            FBM = 6,
            DomainWarp = 7,
            Value = 8,
            Billow = 9,
            WhiteNoise = 10,
            CurlNoise = 11,
            DomeConst = 12,
            PyramidConst = 13,
            ConeConst = 14,
            SteepPeakConst = 15,
            VerySteepPeakConst = 16,
            SteepRidgedConst = 17,
        }

        public HeightmapType heightmapType = HeightmapType.Perlin;
        
        [HideInInspector] public int width = 512; //from GO
        [HideInInspector] public int height = 512; //from go
        public bool randomizeSeed = true; //from go - not used always use seed
        public int seed; //from go
        [HideInInspector] public TextureFormat format = TextureFormat.RGBA32; // for use with Unity Terrain

        public float noiseScale = 100f;
        [Range(1, 8)] public int octaves = 4;
        [Range(0, 1)] public float persistence = 0.5f;
        public float lacunarity = 2f;
        [HideInInspector] public Vector2 offset;
        [Range(0, 100f)] public float heightMultiplier = 1f;

        // later used for tiling
        public Vector2 randomOffsetRange = new Vector2(-1000, 1000);
        public Vector2 randomScaleRange = new Vector2(50, 200);

        
        // later add texture output as well as float[,]
        public Texture2D GenerateTexture(string textureName) {
            if (randomizeSeed) {
                seed = Random.Range(0, int.MaxValue);
                offset = new Vector2(
                    Random.Range(randomOffsetRange.x, randomOffsetRange.y),
                    Random.Range(randomOffsetRange.x, randomOffsetRange.y)
                );
                noiseScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
            }

            float[,] map = HeightmapUtility.GenerateNoiseMap(this);
            return HeightmapUtility.CreateTextureFromMap(map, format, heightMultiplier, textureName);
        }
    }
}