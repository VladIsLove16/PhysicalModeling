using System.Collections.Generic;
using UnityEngine;

public static class ObstacleGenerator
{
    public static List<ObstacleData> GenerateObstacles(GenerationParams generationParams)
    {
        System.Random rnd;
        if (generationParams.fullRandomGen)
            rnd = new System.Random( CombineSeed(generationParams.seed, generationParams.count)); // Генерация зависит и от seed, и от count
        else
            rnd = new System.Random(generationParams.seed); 

       var obstacles = new List<ObstacleData>();

        for (int i = 0; i < generationParams.count; i++)
        {
            Vector3 position = new Vector3(
                Mathf.Lerp(generationParams.areaMin.x, generationParams.areaMax.x, (float)rnd.NextDouble()),
                Mathf.Lerp(generationParams.areaMin.y, generationParams.areaMax.y, (float)rnd.NextDouble()),
                Mathf.Lerp(generationParams.areaMin.z, generationParams.areaMax.z, (float)rnd.NextDouble())
            );

            ObstacleData data = new ObstacleData
            {
                position = position,
                prefabIndex = rnd.Next(generationParams.prefabCount),
                isMovable = rnd.NextDouble() < 0.5,
                isAccelerator = rnd.NextDouble() < 0.3
            };

            obstacles.Add(data);
        }

        return obstacles;
    }
    private static int CombineSeed(int seed, int count)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + seed;
            hash = hash * 31 + count;
            return hash;
        }
    }
    public struct GenerationParams
    {
        public int seed;
        public int count;
        public Vector3 areaMin;
        public Vector3 areaMax;
        public int prefabCount;
        public bool fullRandomGen;

    }

}
