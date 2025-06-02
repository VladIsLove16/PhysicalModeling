using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

public class PistonVisualController : MonoBehaviour
{
    [SerializeField] GameObject mass;
    [SerializeField] GameObject piston1;
    [SerializeField] GameObject piston2;
    [SerializeField] float s1;
    [SerializeField] float s2;
    private float MAXSIZE = 10f;
    private float MINSIZE = 0.1f;
    private Vector3 MAXSIZEVector =new Vector3(10f, 10f, 10f);
    private List<Vector3> sizes = new  List<Vector3> ();
    private void Awake()
    {
        sizes.Add(piston1.transform.localScale);
        sizes.Add(piston2.transform.localScale);
        sizes.Add(piston1.transform.position);
        sizes.Add(piston2.transform.position);
    }
    float y;
    public Vector3 GetPistionTopDeadCenter(float index)
    {
        if (2 <= index)
            throw new ArgumentException("index >= 2 is not available");
        GameObject piston;
        if (index == 0)
            piston = piston1;
        else
            piston = piston2;
        y = piston.transform.lossyScale.y + piston.transform.position.y  + mass.transform.lossyScale.y/2;
        return new Vector3(piston.transform.position.x, y, piston.transform.position.z);
    }
    [ContextMenu("SetSquare")]
    public void SetSquare()
    {
        SetSquare(0, s1);
    }
    [ContextMenu("SetSquare2")]
    public void SetSquare2()
    {
        SetSquare(1, s2);
    }
    public void ResetSizes()
    {
        piston1.transform.localScale = sizes[0];  
        piston2.transform.localScale = sizes[1];
        piston1.transform.position = sizes[2];
        piston2.transform.position = sizes[3];
    }
    public void SetSquare(float index, float newSquare)
    {
        if (2 <= index)
            throw new ArgumentException("index >= 2 is not available");
        GameObject piston;
        if (index == 0)
            piston = piston1;
        else
            piston = piston2;
       float r = Mathf.Sqrt(newSquare/Mathf.PI);
       Vector3 newSize = new Vector3(r, piston.transform.lossyScale.y, r);
       SetSize(piston, newSize);
    }
    [ContextMenu("Move")]
    public void Move()
    {
        Move(0.1f);
    }
    [ContextMenu("MoveBackwards")]
    public void MoveBackwards()
    {
        Move(-0.1f);
    }
    public void Move(float height)
    {
        if (IsPistonInDeadPosition(piston1) && MathF.Sign(height) < 0)
            return;
        if (IsPistonInDeadPosition(piston2) && MathF.Sign(height) > 0)
            return;
        float heigth1 = piston1.transform.localScale.y + height;
        float heigth2 = piston1.transform.localScale.y + height;

        float maxDelta =Mathf.Min(MAXSIZE - piston1.transform.localScale.y, piston2.transform.localScale.y - MINSIZE );
        float minDelta = Mathf.Min(piston1.transform.localScale.y - MINSIZE, MAXSIZE - piston2.transform.localScale.y);
        float totalDelta;
        if(Mathf.Sign(height) > 0)
        {
            totalDelta = -minDelta;
        }
        else 
            totalDelta = maxDelta;

        MovePiston(0, totalDelta) ;
        MovePiston(1, -totalDelta); 
        Vector3 newMassPosition = GetPistionTopDeadCenter(1); 
        mass.transform.position = newMassPosition;
    }
    public bool IsPistonInDeadPosition(GameObject piston)
    {
        if(piston.transform.localScale.y < 0.1f)
        {
            return true; 
        }
        return false;
    }
    private void MovePiston(float index , float deltaHeight)
    {
        if (2 <= index)
            throw new ArgumentException("index >= 2 is not available");
        GameObject piston;
        if (index == 0)
        {
            piston = piston1;
        }
        else
        {
            piston = piston2;
        }
        Vector3 size = piston.transform.lossyScale;
        Vector3 pos = piston.transform.position;
        size.y += deltaHeight;
        size = ClampVector(size);
        SetHeight(piston, size.y);
    }
    private void SetSize(GameObject piston, Vector3 newSize)
    {
        Debug.Log("previous size " + piston.transform.localScale);
        piston.transform.localScale = newSize;
    }

    private void SetHeight(GameObject piston,float newHeight)
    {
        if(newHeight > MAXSIZE || newHeight< MINSIZE)
        {
            Debug.Log("Clamping deltaHeight " + newHeight);
            newHeight = Mathf.Clamp(newHeight, MINSIZE, MAXSIZE); 
        }
        Vector3 size = piston.transform.lossyScale;
        Vector3 position = piston.transform.position;
        SetSize (piston,new Vector3(size.x, newHeight, size.z));
        piston.transform.position = position +  new Vector3(0,  newHeight - size.y, 0);
    }

    private Vector3 ClampVector(Vector3 vector3)
    {
        float x = vector3.x;
        float y = vector3.y;
        float z = vector3.z;
        if(Mathf.Abs(x) > MAXSIZE || Mathf.Abs(y) > MAXSIZE || Mathf.Abs(z) > MAXSIZE ||
        Mathf.Abs(x) < MINSIZE || Mathf.Abs(y) < MINSIZE || Mathf.Abs(z) < MINSIZE)
        {
            Debug.Log("Clamping size " + vector3);
            x = Mathf.Clamp(x, MINSIZE, MAXSIZE);
            y = Mathf.Clamp(y, MINSIZE, MAXSIZE);
            z = Mathf.Clamp(z, MINSIZE, MAXSIZE);
        }
       
        return new Vector3(x, y, z);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.yellow;
        Gizmos.DrawSphere(GetPistionTopDeadCenter(1), 0.1f);
    }
}
