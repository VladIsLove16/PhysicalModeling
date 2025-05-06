using System;
using UnityEngine;

[Serializable]
public struct ObstacleData
{
    public Vector3 position;
    public int prefabIndex;     // индекс prefab в массиве
    public bool isMovable;      // подвижное или нет
    public bool isAccelerator;  // ускоряющее или нет
}
