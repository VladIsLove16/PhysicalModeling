using System;
using System.Collections.Generic;
using UnityEngine;
using static ObstacleGenerator;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs (должны быть одинаково отсортированы)")]
    //public GameObject[] obstaclePrefabs;
    public GameObject AccelerationObstacle;
    public GameObject NonKinemationObstacle;
    public GameObject KinemationObstacle;
    private const int OBSTACLELENGTH = 3;
    [Header("Параметры генерации")]
    public int seed = 12345;
    public int obstacleCount = 5;
    public Vector3 areaMin = new Vector3(-5f, -5f, - 5f);
    public Vector3 areaMax = new Vector3(5f, 5f, 5f);

    public bool clearObstaclesOnGeneration = true;
    private List<GameObject> generatedObstacles = new List<GameObject>();

    //[Header("Компоненты поведения")]
    //public PhysicsMaterial bouncyMaterial;

    public void SetSeed(int newSeed)
    {
        seed = newSeed;
        Debug.Log("new Seed: " +  seed);
    }


    [ContextMenu("GenerateSeed")]
    public void GenerateSeed()
    {
        seed = new System.Random(seed).Next();
    }
    [ContextMenu("SpawnObstaclesWithNewSeed")]
    public void SpawnObstaclesWithNewSeed()
    {
        GenerateSeed();
        SpawnObstacles();
    }
    [ContextMenu("SpawnObstacles")]
    public void SpawnObstacles()
    {
        List<ObstacleData> generated = GenerateObstaclesData();
        InstantiateObstacles(generated);
    }

    private List<ObstacleData> GenerateObstaclesData()
    {
        ObstacleGenerator.GenerationParams generationParams = new ObstacleGenerator.GenerationParams()
        {
            seed = seed,
            areaMax = areaMax,
            areaMin = areaMin,
            count = obstacleCount,
            prefabCount = OBSTACLELENGTH,
            fullRandomGen = false,
        };
        List<ObstacleData> generated = ObstacleGenerator.GenerateObstacles(generationParams);
        return generated;
    }

    [ContextMenu("ClearObstacles")]
    public void ClearObstacles()
    {
        foreach (GameObject obstacle in generatedObstacles)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(obstacle);
            else
                Destroy(obstacle);
#endif
        }
        generatedObstacles.Clear();
    }

    void InstantiateObstacles(List<ObstacleData> obstacleDataList)
    {
        if(clearObstaclesOnGeneration)
            ClearObstacles();
        foreach (var data in obstacleDataList)
        {
            GameObject prefab;
            if (data.isAccelerator)
                prefab = AccelerationObstacle;
            else if (data.isMovable)
            {
                prefab = NonKinemationObstacle;
            }
            else
                prefab = KinemationObstacle;
            GameObject instance = Instantiate(prefab, data.position, Quaternion.identity, transform);
            generatedObstacles.Add(instance);
        }
    }

    public void SetObstaclesMass(float value)
    {
        foreach(GameObject obstacle in generatedObstacles)
        {
            obstacle.GetComponent<Rigidbody>().mass = value;
        }
    }
    //legacy
    //void InstantiateObstacles(List<ObstacleData> obstacleDataList)
    //{
    //    foreach (var data in obstacleDataList)
    //    {
    //        GameObject prefab = obstaclePrefabs[data.prefabIndex];
    //        GameObject instance = Instantiate(prefab, data.position, Quaternion.identity, transform);

    //        Rigidbody rb = instance.GetComponent<Rigidbody>();
    //        Collider col = instance.GetComponent<Collider>();
    //        MeshRenderer meshRenderer = instance.GetComponent<MeshRenderer>();

    //        if (rb != null)
    //        {
    //            rb.isKinematic = data.isMovable ? true : false;
    //        }

    //        if (col != null)
    //        {
    //            col.sharedMaterial = bouncyMaterial;

    //            if (data.isAccelerator)
    //            {
    //                instance.tag = "Bouncer"; // Для логики столкновений
    //            }
    //        }
    //    }
    // }
}

