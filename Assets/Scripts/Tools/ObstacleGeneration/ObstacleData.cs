using System;
using UnityEngine;

[Serializable]
public struct ObstacleData
{
    public Vector3 position;
    public int prefabIndex;     // ������ prefab � �������
    public bool isMovable;      // ��������� ��� ���
    public bool isAccelerator;  // ���������� ��� ���
}
