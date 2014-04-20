using UnityEngine;

public class Utilities
{
	public delegate void VoidDelegate();
	public delegate bool BoolDelegate();
	public delegate void OnCollideDelegate(GameObject other);
	public delegate void OnUpdateDelegate(float deltaTime);


	static public int BOATLAYER = LayerMask.NameToLayer("Boat");
	static public int HOOKLAYER = LayerMask.NameToLayer("Hook");
	static public int FISHLAYER = LayerMask.NameToLayer("Fish");
	static public int MANLAYER = LayerMask.NameToLayer("Man");

	static public void Noop()
	{
		return;
	}
	static public bool NoopBool()
	{
		return false;
	}

	static public float SqrMag(Vector2 first, Vector3 second)
	{
		return (first.x - second.x)*(first.x - second.x) + (first.y - second.y)*(first.y - second.y);
	}

	static public Vector3 ToXYVector(Vector2 vec)
	{
		return new Vector3(vec.x, vec.y, 0);
	}

	static public Vector3 ToXZVector(Vector2 vec)
	{
		return new Vector3(vec.x, 0, vec.y);
	}

	static public float SUPERFAR = 99999999f;


	public static void DrawForce(TauBody body)
	{
		BasicRenderer.Instance.SetRenderMode(RenderMode.ALPHA);
		Color color = BasicRenderer.GetColorFromUID(body.owner.tauID);
		for(int i=0; i < TauBody.ForceTypeCount; ++i)
		{
			Force f = body.forces[i];
			if (f.enabled)
			{
				Vector3 future = body.thisPos + Utilities.ToXYVector(f.fVec);
				BasicRenderer.Instance.AddLine(RenderMode.ALPHA, body.thisPos, future, 2*0.1f, color);
			}
		}
	}

	public static void DrawForce(TauBody body, float factor)
	{
		BasicRenderer.Instance.SetRenderMode(RenderMode.ALPHA);
		Color color = BasicRenderer.GetColorFromUID(body.owner.tauID);
		for(int i=0; i < TauBody.ForceTypeCount; ++i)
		{
			Force f = body.forces[i];
			if (f.enabled)
			{
				Vector3 future = body.thisPos + factor*Utilities.ToXYVector(f.fVec);
				BasicRenderer.Instance.AddLine(RenderMode.ALPHA, body.thisPos, future, 2*0.1f, color);
			}
		}
	}
}