using UnityEngine;
using System.Text;


public class CloudBody : TauBody
{
	public CloudPawn cloudOwner;

	public float configSpeedMin = 1.0f;
	public float configSpeedMax = 3.0f;

	public Vector2 imageSize;

	public float pushMin;
	public float pushMax;
	public float pushForce;
	public Vector2 pushDir;
	
	public Quaternion baseRot;
	
	public bool CanMove { get { return !machine.IsState(BodyState.BACKGROUND); } }

	
	public override void StartSetup()
	{
		base.StartSetup();
		//HandleAxis = HookHandleInput;
		//HandleCollide = FishCollide;
		MotorUpdate = CloudNothing;
		PhysicsUpdate = SimplePhysicsUpdate;

		float fishSpeed = Random.Range(configSpeedMin,configSpeedMax); 
		pushMin = GameVars.FISH_SPEED_MIN*fishSpeed;
		pushMax = GameVars.FISH_SPEED_MAX*fishSpeed;
		pushForce = GameVars.FISH_PUSH_FORCE*fishSpeed;
		dragCoeff = 0.1f;

		imageSize = new Vector2(4,4);

	}

	public override void FinishSetup()
	{
		base.FinishSetup();
		SetForce(ForceType.FLOAT, 0f);
		SetForce(ForceType.GRAVITY, 0f);
	}

	public override void InitMachine()
	{
		base.InitMachine();
		machine.AddEnterListener((int)BodyState.WAITING, OnWaiting);
		
		machine.AddEnterListener((int)BodyState.READY, OnReady);
		machine.AddEnterListener((int)BodyState.ENTERING, OnEntering);
		machine.AddEnterListener((int)BodyState.MOVING, OnMoving);
		
		machine.AddEnterListener((int)BodyState.BACKGROUND, OnBackground);
	}


	public void CheckOOB()
	{
		bool oobMAXX = (thisPos.x+(imageSize.x/2) > GameVars.WATER_BOUND.xMax);
		bool oobMINX = (thisPos.x-(imageSize.x/2) < GameVars.WATER_BOUND.xMin);

		//bool oobMAXY = (thisPos.y+(imageSize.y/2) < GameVars.WATER_BOUND.yMin);
		//bool oobMINY = (thisPos.y-(imageSize.y/2) > GameVars.WATER_BOUND.yMax);
		bool isOOB = oobMAXX || oobMINX;// || oobMAXY || oobMINY;
		if (machine.IsState(BodyState.ENTERING))
		{
			if (!isOOB)
			{
				machine.SetState(BodyState.MOVING);
			}
		}
		else if (machine.IsState(BodyState.MOVING))
		{
			if (isOOB)
			{
				machine.SetState(BodyState.BACKGROUND);
			}
		}
	}

	public void OnReady(object obj)
	{
		machine.SetState(BodyState.ENTERING);
	}

	public void OnEntering(object obj)
	{
		tauCollider.collider.enabled = false;
		MotorUpdate = CloudMotorUpdate;
	}

	public void OnMoving(object obj)
	{
		tauCollider.collider.enabled = true;
		MotorUpdate = CloudMotorUpdate;
	}

	public void OnWaiting(object obj)
	{
		tauCollider.collider.enabled = false;
	}
	
	public void OnBackground(object obj)
	{
		thisPos = Vector2.zero;
		Stop();
		tauCollider.collider.enabled = false;
		MotorUpdate = CloudNothing;
		cloudOwner.ReturnToSpawner();
		
	}

	public void CloudNothing(float deltaTime)
	{

	}


	public void UpdateTimers(float deltaTime)
	{
		
	}

	public void CloudMotorUpdate(float deltaTime)
	{
		CheckOOB();
		UpdateTimers(deltaTime);
		if (!CanMove)
		{
			return;
		}

		owner.brainEntity.BrainUpdate(deltaTime);
		bool didChange = owner.brainEntity.ControlCloudBody(this);

		if (didChange)
		{
			if (IsForceEnabled(ForceType.ENGINE))
			{
				sequencer.SwitchToAnim(SequenceType.MOVE);
			}
			else
			{
				sequencer.isLooping = false;
			}
				 //: SequenceType.IDLE);
		}

		//Utilities.DrawForce(this);
	}


	public override bool CanAttach()
	{
		return false;
	}
}