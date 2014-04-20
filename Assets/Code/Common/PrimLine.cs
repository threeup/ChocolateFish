using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public struct PrimLine
{
    public Vector3 src;
    public Vector3 dst;
    public Vector3 widthVector;
    public Color drawColor;
    public bool gradient;

    public PrimLine(Vector3 sta, Vector3 en, Vector3 cro, Color col, bool grad)
    {
        src = sta;
        dst = en;
        widthVector = cro;
        drawColor = col;
        gradient = grad;
    }

    public PrimLine(Vector3 start, float radius, Color col, bool grad)
    {

        Vector3 b1 = start, b2 = start, b3 = start, b4 = start;
        b1.x -= radius;
        b2.x += radius;
        b3.y -= radius;
        b4.y += radius;
        src = b1;
        dst = b2;
        widthVector = Vector3.forward*radius;
        drawColor = col;
        gradient = grad;
    }
}


public struct PrimPrism
{
    public PrimLine[] lines; 

    public PrimPrism(Vector3 start, float radius, float height, Color col)
    {
        lines = new PrimLine[6];
        // bottom
        Vector3 b1 = start, b2 = start, b3 = start, b4 = start;
        b1.x -= radius;
        b2.x += radius;
        b3.z -= radius;
        b4.z += radius;
        Vector3 t1 = start, t2 = start, t3 = start, t4 = start;
        t1.y += height;
        t2.y += height;
        t3.y += height;
        t4.y += height;
        t1.x -= radius;
        t2.x += radius;
        t3.z -= radius;
        t4.z += radius;

        lines[0] = new PrimLine(b1, b2, Vector3.forward*radius, col, false);
        lines[1] = new PrimLine(t1, t2, Vector3.forward*radius, col, false);
        lines[2] = new PrimLine(b1, t1, Vector3.forward*radius, col, false);
        lines[3] = new PrimLine(b2, t2, Vector3.forward*radius, col, false);
        lines[4] = new PrimLine(b3, t3, Vector3.left*radius, col, false);
        lines[5] = new PrimLine(b4, t4, Vector3.left*radius, col, false);
    }
}


public struct PrimTriangle
{
    public static float thickness = 2.0f;
    public PrimLine[] lines;
    public PrimTriangle( Vector3 start, Vector3 forward, float angle, float len, Color col)
    {
        lines = new PrimLine[4];
        float halfAngle = angle/2f;
        float drawLen = Mathf.Min(len, 9f);
        
        float radius = drawLen * Mathf.Clamp(Mathf.Tan(Mathf.Deg2Rad*halfAngle), -1f, 1f);
        float side = Mathf.Sqrt((drawLen*drawLen) + (radius*radius));
        
        Vector3 leftConeSide = (Quaternion.Euler(0f, halfAngle, 0f) * forward).normalized * side;
        Vector3 one = start + leftConeSide;
        lines[0] = new PrimLine(start, one, PrimRenderer.ComputeWidth(start, one, thickness), col, true);
        
        Vector3 rightConeSide = (Quaternion.Euler(0f, -halfAngle, 0f) * forward).normalized * side;
        Vector3 two = start + rightConeSide;
        lines[1] = new PrimLine(start, two, PrimRenderer.ComputeWidth(start, two, thickness), col, true);

        Vector3 mid = start + forward*side;
            
        lines[2] = new PrimLine(one, mid, PrimRenderer.ComputeWidth(one, mid, thickness), col, true);
        lines[3] = new PrimLine(mid, two, PrimRenderer.ComputeWidth(mid, two, thickness), col, true);
    
    }
}