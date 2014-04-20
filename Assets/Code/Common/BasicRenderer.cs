
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public enum RenderMode
{
    NONE,
    ALPHA,
    BETA,
    GAMMA,
    ALWAYS,
}

public struct LineData
{
    public Vector3 src;
    public Vector3 dst;
    public Vector3 widthVector;
    public float expire;
    public Color drawColor;
    public bool gradient;
    public bool screenSpace;

    public LineData(Vector3 sta, Vector3 en, Vector3 cro, float exp, Color col, bool grad, bool isScreenSpace = false)
    {
        src = sta;
        dst = en;
        widthVector = cro;
        expire = exp;
        drawColor = col;
        gradient = true;
        screenSpace = isScreenSpace;
    }
}

public class BasicRenderer : MonoBehaviour
{    
	private static BasicRenderer instance;
	public static BasicRenderer Instance { get { return instance; } }
	
    private int renderMode = (int)RenderMode.NONE;
    private int maxLines = 3000;
    public int count = 0;
    
    public List<Queue<LineData>> queueList;
    public Queue<LineData> allQueue;

    bool isInitialized = false;
    public bool IsInitialized{ get { return isInitialized; } }
    float timer = 0f;
    float thickness = 0.3f;
    
    int lastColor = 0;
    static Color[] autoColor;
    public static Color alphaBlack = new Color(0f,0f,0f,0.2f);
    public static Color alphaWhite = new Color(1f,1f,1f,0f);

    public Material _glMaterial;

    public void Awake()
    {
		instance = this;
        autoColor = new Color[6];
        autoColor[0] = new Color(0f,   0f,   0.5f,      0.2f);
        autoColor[1] = new Color(0.5f, 0f,   0f,        0.2f);
        autoColor[2] = new Color(0f,   0.5f, 0f,        0.2f);
        autoColor[3] = new Color(0f,   0.5f, 0.5f,      0.2f);
        autoColor[4] = new Color(0.5f, 0.5f, 0f,        0.2f);
        autoColor[5] = new Color(0.5f, 0f,   0.5f,      0.2f);
        Initialize();       
    }

    public void Initialize()
    {
        if( queueList != null )
            return; // Already Initialized.
        isInitialized = true;
        queueList = new List<Queue<LineData>>();
        int count = System.Enum.GetValues(typeof(RenderMode)).Length;
        for(int i=0; i<count; ++i)
        {
            queueList.Add(new Queue<LineData>(maxLines));
        }
        allQueue = queueList[(int)RenderMode.ALWAYS];
    }

    public RenderMode GetRenderMode()
    {
        return (RenderMode)renderMode;
    }

    public void SetRenderMode(RenderMode channel)
    {
		if (renderMode == (int)channel)
        {
            return;
        }
        renderMode = (int)channel;
    }

    public void ClearRenderMode(RenderMode mode)
    {
        queueList[(int)renderMode].Clear();
    }

    public Color GenerateColor(float percent)
    {
        Color c = new Color(1.0f, 1.0f, 1.0f);

        float dv = 1.0f;

        if (percent < (0.25f * dv)) {
          c.r = 0f;
          c.g = 4f * percent / dv;
        } else if (percent < (0.5f * dv)) {
          c.r = 0f;
          c.b = 1 + 4 * (0.25f * dv - percent) / dv;
        } else if (percent < (0.75f * dv)) {
          c.r = 4 * (percent - 0.5f * dv) / dv;
          c.b = 0f;
        } else {
          c.g = 1 + 4 * (0.75f * dv - percent) / dv;
          c.b = 0f;
        }
        return c;
    } 

    public static Color GetColorFromUID(int uid)
    {
        return autoColor[uid % 6];
    }

    public Color GetNextAutoColor()
    {
        lastColor++;
        if (lastColor >= 6) { lastColor = 0;}
        return autoColor[lastColor];
    }

    public void AddTriangle(RenderMode channel, Vector3 start, Vector3 forward, float angle, float len, float expiration)
    {
        AddTriangle(channel, start, forward, angle, len, expiration, GetNextAutoColor());
    }
    public void AddTriangle(RenderMode channel, Vector3 start, Vector3 forward, float angle, float len, float expiration, int uid)
    {
        AddTriangle(channel, start, forward, angle, len, expiration, GetColorFromUID(uid));
    }
    public void AddTriangle(RenderMode channel, Vector3 start, Vector3 forward, float angle, float len, float expiration, Color col)
    {    
        float halfAngle = angle/2f;
        float drawLen = Mathf.Min(len, 9f);
        
        float radius = drawLen * Mathf.Clamp(Mathf.Tan(Mathf.Deg2Rad*halfAngle), -1f, 1f);
        float side = Mathf.Sqrt((drawLen*drawLen) + (radius*radius));
        
        Vector3 leftConeSide = (Quaternion.Euler(0f, halfAngle, 0f) * forward).normalized * side;
        Vector3 one = start + leftConeSide;
        AddLine(channel, new LineData(one, start, ComputeWidth(one, start), expiration + Time.time, col, true));
        
        Vector3 rightConeSide = (Quaternion.Euler(0f, -halfAngle, 0f) * forward).normalized * side;
        Vector3 two = start + rightConeSide;
        AddLine(channel, new LineData(two, start, ComputeWidth(two, start), expiration + Time.time, col, true));

        Vector3 mid = start + forward*side;
            
        AddLine(channel, new LineData(one, mid, ComputeWidth(one, mid), expiration + Time.time, col, false));
        AddLine(channel, new LineData(mid, two, ComputeWidth(mid, two), expiration + Time.time, col, false));
    }

    public void AddCircle(RenderMode channel, Vector3 start, float radius, float expiration, Color col)
    {
        for(int i=0; i<8; ++i)
        {
            Vector3 edge = start;
            switch(i)
            {
                case 0: edge.x += radius; break;
                case 2: edge.z += radius; break;
                case 4: edge.x -= radius; break;
                case 6: edge.z -= radius; break;
                case 1: edge.x += radius; edge.z -= radius; break;
                case 5: edge.x += radius; edge.z += radius; break;
                case 3: edge.x -= radius; edge.z -= radius; break;
                case 7: edge.x -= radius; edge.z += radius; break;
            }
            AddLine(channel, new LineData(start, edge, ComputeWidth(start, edge), expiration + Time.time, col, true));
        }
    
    }

    public void AddHexagon(RenderMode channel, Vector3 start, float radius, float expiration, Color col)
    {
        Vector3 last = start;
        Vector3 edge = start;
        for(int i=0; i<9; ++i)
        {
            edge = start;
            switch(i)
            {
                case 0: edge.x += radius; break;
                case 1: edge.x += radius*0.707f; edge.z -= radius*0.707f; break;
                case 2: edge.z -= radius; break;
                case 3: edge.x -= radius*0.707f; edge.z -= radius*0.707f; break;
                case 4: edge.x -= radius; break;
                case 5: edge.x -= radius*0.707f; edge.z += radius*0.707f; break;
                case 6: edge.z += radius; break;
                case 7: edge.x += radius*0.707f; edge.z += radius*0.707f; break;
                case 8: edge.x += radius; break;
            }
            if (i > 0)
            {
                AddLine(channel, new LineData(last, edge, ComputeWidth(last, edge), expiration + Time.time, col, true));
            }
            last = edge;
        }       
    }


    public void AddLine(RenderMode channel, Vector3 start, Vector3 end, float expiration)
    {
        AddLine(channel, new LineData(start, end, ComputeWidth(start, end), expiration + Time.time, GetNextAutoColor(), false));
    }

    public void AddLine(RenderMode channel, Vector3 start, Vector3 end, float expiration, Color col, bool drawInScreenSpace = false)
    {
        AddLine(channel, new LineData(start, end, ComputeWidth(start, end), expiration + Time.time, col, false, drawInScreenSpace));
    }

    public void AddLine(RenderMode channel, LineData data)
    {
        Queue<LineData> currentQueue = queueList[(int)channel];
        if (currentQueue.Count >= maxLines)
        {
            currentQueue.Dequeue();
        }
        currentQueue.Enqueue(data);         
    }


    public void AddPrism(RenderMode channel, Vector3 start, float radius, float height)
    {
            AddPrism(channel, start, radius, height, 5f, autoColor[5]);
    }
    public void AddPrism(RenderMode channel, Vector3 start, float radius, float height, float expiration, Color col)
    {
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

        AddLine(channel, new LineData(b1, b2, Vector3.forward*radius, expiration + Time.time, col, false));
        AddLine(channel, new LineData(t1, t2, Vector3.forward*radius, expiration + Time.time, col, false));
        AddLine(channel, new LineData(b1, t1, Vector3.forward*radius, expiration + Time.time, col, false));
        AddLine(channel, new LineData(b2, t2, Vector3.forward*radius, expiration + Time.time, col, false));
        AddLine(channel, new LineData(b3, t3, Vector3.left*radius, expiration + Time.time, col, false));
        AddLine(channel, new LineData(b4, t4, Vector3.left*radius, expiration + Time.time, col, false));
    }

    public void AddOutlineSquare(RenderMode channel, Vector3 start, float radius, float expiration, Color col)
    {
        Vector3 b1 = start, b2 = start, b3 = start, b4 = start;
        b1.x += radius;
        b1.z -= radius;
        b2.x -= radius;
        b2.z -= radius;
        b3.x -= radius;
        b3.z += radius;
        b4.x += radius;
        b4.z += radius;

        AddLine(channel, new LineData(b1, b2, ComputeWidth(b1, b2), expiration + Time.time, col, false));   
        AddLine(channel, new LineData(b2, b3, ComputeWidth(b2, b3), expiration + Time.time, col, false));   
        AddLine(channel, new LineData(b3, b4, ComputeWidth(b3, b4), expiration + Time.time, col, false));   
        AddLine(channel, new LineData(b4, b1, ComputeWidth(b4, b1), expiration + Time.time, col, false));   
    }
    
    public void AddSolidSquare(RenderMode channel, Vector3 start, float radius, float expiration, Color col)
    {
        Vector3 b1 = start, b2 = start;
        b1.x -= radius;
        b2.x += radius;
        AddLine(channel, new LineData(b1, b2, Vector3.forward*radius, expiration + Time.time, col, false));
    }

    private Vector3 ComputeWidth(Vector3 start, Vector3 end)
    {
        Vector3 widthVector = (end - start).normalized;
        float oldX = widthVector.x;
        widthVector.x = widthVector.y;
        widthVector.y = oldX;
        widthVector.z = 0.0f;
        widthVector *= thickness/2f;
        return widthVector;
    }

    public void Reset()
    {
        
    }

    public void Show(float duration)
    {
        timer = duration;
    }

    public void Update()
    {
        if (IsRendererActive())
        {
            timer -= Time.deltaTime;
        }
    }

    public void OnPostRender() 
    {
		Queue<LineData> lineQueue = queueList[(int)renderMode]; 
        if (IsRendererActive() && (lineQueue.Count > 0 || allQueue.Count > 0))
        {
            RenderProj(lineQueue);
        }
    }   

    public void DrawLine(LineData line)
    {
        GL.Color(line.drawColor);
        GL.Vertex3(line.src.x + line.widthVector.x, line.src.y - line.widthVector.y, line.src.z - line.widthVector.z);
        GL.Vertex3(line.src.x - line.widthVector.x, line.src.y + line.widthVector.y, line.src.z + line.widthVector.z);
        if (line.gradient)
        { 
            GL.Color(alphaWhite);
        }
        GL.Vertex3(line.dst.x - line.widthVector.x, line.dst.y + line.widthVector.y, line.dst.z + line.widthVector.z);
        GL.Vertex3(line.dst.x + line.widthVector.x, line.dst.y - line.widthVector.y, line.dst.z - line.widthVector.z);
    }

    public void RenderProj(Queue<LineData> lineQueue)
    {
        if (_glMaterial == null)
        {
            timer = 0f;
            return;
        }
        GL.PushMatrix();
        _glMaterial.SetPass(0);
        GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
        //GL.LoadPixelMatrix();
        GL.Begin(GL.QUADS);
        float currentTime = Time.time;
        foreach(LineData line in lineQueue)
        {
            if (line.expire > currentTime && !line.screenSpace)
            {
                DrawLine(line);    
            }
        }
        foreach(LineData line in allQueue)
        {
            if (line.expire > currentTime && !line.screenSpace)
            {
                DrawLine(line);    
            }
        }
        
        
        
        GL.End();
        GL.PopMatrix();
        GL.PushMatrix();
        GL.LoadPixelMatrix();
        GL.Begin(GL.LINES);
        
        foreach(LineData line in lineQueue)
        {
            if (line.expire > currentTime && line.screenSpace)
            {
                //DrawLine(line);
                GL.Color(line.drawColor);
                GL.Vertex3(line.src.x, line.src.y, line.src.z);
                GL.Vertex3(line.dst.x, line.dst.y, line.dst.z);
            }
        }
        
       
        GL.End();
        GL.PopMatrix();
        count = lineQueue.Count + allQueue.Count;
    }


    public bool IsRendererActive()
    {
        return true;//timer > 0f && lineQueue != null;
    }
}