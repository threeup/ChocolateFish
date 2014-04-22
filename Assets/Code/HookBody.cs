	using UnityEngine;
using System.Text;


public class HookBody : TauBody
{
	public BoatPawn boatOwner;

	public Vector2 imageSize;
	public float pullForce;
	public float tensionForce;
	
	public override void StartSetup()
	{
		base.StartSetup();
		HandleAxis = HookHandleInput;
		HandleCollide = HookCollide;
		MotorUpdate = HookUpdate;
		PhysicsUpdate = SimplePhysicsUpdate;

		maxSpeed = 10f;
		dragCoeff = 1f;
		pullForce = 150f;
		tensionForce = 10f;
		thisMass = 1f;

		imageSize = new Vector2(4,4);
		
	}


	public void HookHandleInput(float x, float y)
	{
		SetForceDir(ForceType.ENGINE, y*Vector2.up);
		SetForce(ForceType.ENGINE, pullForce);
		if (Mathf.Abs(y) > 0.1)
		{
			SetForceEnabled(ForceType.ENGINE, true);
		}
		else
		{
			SetForceEnabled(ForceType.ENGINE, false);
		}
	}

	public void PullTowards(Vector2 pos)
	{
		/*Vector2 diff = (pos.x-thisPos.x)*Vector2.right;
		SetForceDir(ForceType.TENSION, diff.normalized);
		SetForce(ForceType.TENSION, diff.magnitude*tensionForce);
		SetForceEnabled(ForceType.TENSION, true);*/
		thisPos.x = pos.x;
		SyncToThis();
	}

	public void HookUpdate(float deltaTime)
	{
		SetForce(ForceType.FLOAT, 0f);
		SetForce(ForceType.GRAVITY, GameVars.GRAVITY*thisMass);

		bool oobMAXX = (thisPos.x+(imageSize.x/2) > GameVars.WATER_BOUND.xMax);
		bool oobMINX = (thisPos.x-(imageSize.x/2) < GameVars.WATER_BOUND.xMin);

		bool oobMAXY = (thisPos.y+(imageSize.y/2) < GameVars.WATER_BOUND.yMin);
		bool oobMINY = (thisPos.y-(imageSize.y/2) > 0);
		if (oobMAXX || oobMINX || oobMAXY || oobMINY)
		{
			Vector2 dir = new Vector2(thisPos.x, thisPos.y) - GameVars.WATER_BOUND.center;
			SetForceDir(ForceType.ENVIRO, -dir.normalized);
			SetForce(ForceType.ENVIRO, 1.5f*pullForce);
			SetForceEnabled(ForceType.ENVIRO, true);
			SetForceEnabled(ForceType.ENGINE, false);
		}
		else
		{
			SetForceEnabled(ForceType.ENVIRO, false);	
		}
	}

	
	public void HookCollide(GameObject otherObj)
	{
		TauCollider otherColl = otherObj.GetComponent<TauCollider>();
		FishBody otherFishBody = otherColl.body as FishBody;
		if (otherFishBody != null && otherObj.layer == Utilities.FISHLAYER)
		{
			Vector3 diffPos = otherFishBody.gameObject.transform.position - go.transform.position;
			diffPos.z = 0;
			AttachToSelf(otherFishBody, diffPos);
			return;
		}
		if (otherFishBody != null && otherObj.layer == Utilities.DEBRISLAYER)
		{
			PurgeContents(BodyState.MOVING);
			return;
		}

		
		if (otherObj.layer == Utilities.MANLAYER)
		{
			GameDirector.Instance.Score((int)Mathf.Round(thisMass - 1f));
			PurgeContents(BodyState.BACKGROUND);
			return;
		}
	}
}