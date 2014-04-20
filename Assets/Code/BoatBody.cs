using UnityEngine;
using System.Text;


public class BoatBody : TauBody
{
	public BoatPawn boatOwner;
	

	public Vector2 imageSize;
	public float pushDuration;
	public float pushForce;

	public override void StartSetup()
	{
		HandleAxis = BoatHandleInput;
		MotorUpdate = BoatUpdate;
		PhysicsUpdate = SimplePhysicsUpdate;
		pushDuration = 0; //not applicable
		pushForce = GameVars.BOAT_PUSH_FORCE;
		imageSize = new Vector2(4,4);
		dragCoeff = 0.8f;
		base.StartSetup();
	}

	public override void FinishSetup()
	{
		base.FinishSetup();
	}





	public void BoatHandleInput(float x, float y)
	{
		SetForceDir(ForceType.ENGINE, x*Vector2.right);
		SetForce(ForceType.ENGINE, pushForce);
		if (Mathf.Abs(x) > 0.1)
		{
		
			SetForceEnabled(ForceType.ENGINE, true);
		}
		else
		{
			SetForceEnabled(ForceType.ENGINE, false);
		}

		boatOwner.SetHook(y);
	}

	public void BoatUpdate(float deltaTime)
	{
		bool oobMAX = (thisPos.x+(imageSize.x/2) > GameVars.WATER_BOUND.xMax);
		bool oobMIN = (thisPos.x-(imageSize.x/2) < GameVars.WATER_BOUND.xMin);
		if (oobMAX || oobMIN)
		{
			Vector2 dir = new Vector2(thisPos.x, thisPos.y) - GameVars.WATER_BOUND.center;
			dir.y = 0;
			SetForceDir(ForceType.ENVIRO, -dir.normalized);
			SetForce(ForceType.ENVIRO, 2.0f*pushForce);
			SetForceEnabled(ForceType.ENVIRO, true);
			SetForceEnabled(ForceType.ENGINE, false);
		}
		else
		{
			SetForceEnabled(ForceType.ENVIRO, false);	
		}

		if (boatOwner.hook != null)
		{
			boatOwner.hook.PullTowards(thisPos);
		}
		
		//Utilities.DrawForce(this);
	}
}