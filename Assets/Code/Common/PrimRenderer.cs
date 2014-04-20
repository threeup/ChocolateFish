using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum RenderChannel
{
    NONE,
    BACK,
    MID,
    FRONT,
}

public class PrimRenderer : MonoBehaviour
{    

    public static PrimRenderer Instance;
    private int maxLines = 3000;
    public int count = 0;
    
    public List<Queue<PrimLine>> queueList;

    //bool isInitialized = false;
    public float thickness = 0.4f;
    

	public Material _glMaterial;

	public void Awake()
	{
		Instance = this;

		this.gameObject.transform.position = Camera.main.transform.position;
		this.gameObject.transform.rotation = Camera.main.transform.rotation;
		Initialize();		
	}

    public void Initialize()
    {
        //isInitialized = true;
        queueList = new List<Queue<PrimLine>>();
        Array values = System.Enum.GetValues(typeof(RenderChannel));
		for(int i=0; i<values.Length; ++i)
		{
			queueList.Add(new Queue<PrimLine>(maxLines));
		}
    }

    public void ClearRenderMode(RenderChannel mode)
    {
    	queueList[(int)mode].Clear();
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

    public void AddPrism(RenderChannel channel, PrimLine data)
    {
        queueList[(int)channel].Enqueue(data);        
    }
    public void AddTriangle(RenderChannel channel, PrimLine data)
    {
        queueList[(int)channel].Enqueue(data);        
    }
    public void AddLine(RenderChannel channel, PrimLine data)
    {
        queueList[(int)channel].Enqueue(data);        
    }



    public static Vector3 ComputeWidth(Vector3 start, Vector3 end, float thickness)
    {
    	Vector3 widthVector = (end - start).normalized;
    	widthVector.y = widthVector.z;
    	widthVector.z = widthVector.x;
    	widthVector.x = widthVector.y;
    	widthVector.y = 0.0f;
    	widthVector *= thickness/2f;
    	return widthVector;
    }

    public void Reset()
    {
        
    }

    public void Update()
    {
        
    }

	public void OnPostRender() 
	{
		RenderProj();
	}   

	public void RenderProj()
	{
	    if (_glMaterial == null)
        {
             Debug.Log("drawnull");
            return;
        }
        count = 0;
        GL.PushMatrix();
	    _glMaterial.SetPass(0);
	    GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
	    GL.Begin(GL.QUADS);
        foreach(Queue<PrimLine> lineQueue in queueList)
        {
    	    foreach(PrimLine line in lineQueue)
    	    {
    	    	GL.Color(line.drawColor);
                GL.Vertex3(line.src.x + line.widthVector.x, line.src.y - line.widthVector.y, line.src.z);
                GL.Vertex3(line.src.x - line.widthVector.x, line.src.y + line.widthVector.y, line.src.z);
                if (line.gradient)
                { 
                    GL.Color(Color.white);
                }

                GL.Vertex3(line.dst.x - line.widthVector.x, line.dst.y + line.widthVector.y, line.dst.z);
                GL.Vertex3(line.dst.x + line.widthVector.x, line.dst.y - line.widthVector.y, line.dst.z);
                count++;
    		}
        }
	    GL.End();
	    GL.PopMatrix();
	}
}