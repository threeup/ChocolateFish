using UnityEngine;
using System.Text;


public class FishBody : TauBody
{
	public FishPawn fishOwner;

	public Vector2 imageSize;

	public float pushMin;
	public float pushMax;
	public float pushForce;
	public Vector2 pushDir;

	float hookImmuneTime;

	
	public Quaternion baseRot;
	
	public bool CanMove { get { return !machine.IsState(BodyState.BACKGROUND); } }

	
	public override void StartSetup()
	{
		base.StartSetup();
		//HandleAxis = HookHandleInput;
		//HandleCollide = FishCollide;
		MotorUpdate = FishNothing;
		PhysicsUpdate = SimplePhysicsUpdate;

		int fishSpeed = Random.Range(1,3); 
		float fishQuickness = fishSpeed*0.5f;
		pushMin = GameVars.FISH_SPEED_MIN*fishQuickness;
		pushMax = GameVars.FISH_SPEED_MAX*fishQuickness;
		pushForce = GameVars.FISH_PUSH_FORCE*fishQuickness;
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
		
		machine.AddEnterListener((int)BodyState.ATTACHED, OnAttach);
		machine.AddExitListener((int)BodyState.ATTACHED, OnAttachExit);
		machine.AddEnterListener((int)BodyState.BACKGROUND, OnBackground);
	}


	public void CheckOOB()
	{
		bool oobMAXX = (thisPos.x+(imageSize.x/2) > GameVars.WATER_BOUND.xMax);
		bool oobMINX = (thisPos.x-(imageSize.x/2) < GameVars.WATER_BOUND.xMin);

		bool oobMAXY = (thisPos.y+(imageSize.y/2) < GameVars.WATER_BOUND.yMin);
		bool oobMINY = (thisPos.y-(imageSize.y/2) > GameVars.WATER_BOUND.yMax);
		bool isOOB = oobMAXX || oobMINX || oobMAXY || oobMINY;
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
		hookImmuneTime = 999;
		MotorUpdate = FishMotorUpdate;
	}

	public void OnMoving(object obj)
	{
		tauCollider.collider.enabled = true;
		hookImmuneTime = GameVars.HOOKIMMUNE;
		MotorUpdate = FishMotorUpdate;
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
		MotorUpdate = FishNothing;
		fishOwner.ReturnToSpawner();
		
	}

	public void OnAttach(object obj)
	{
		Stop();
		SetForceEnabled(ForceType.ENGINE, false);
		tauCollider.collider.enabled = false;
		MotorUpdate = FishAttachedMotor;
	}

	public void OnAttachExit(object obj)
	{
		
	}

	public void FishNothing(float deltaTime)
	{

	}

	public void FishAttachedMotor(float deltaTime)
	{

	}

	public void UpdateTimers(float deltaTime)
	{
		if (hookImmuneTime > 0f)
		{
			hookImmuneTime -= deltaTime;
		}
	}

	public void FishMotorUpdate(float deltaTime)
	{
		CheckOOB();
		UpdateTimers(deltaTime);
		if (!CanMove)
		{
			return;
		}

		fishOwner.brainEntity.BrainUpdate(deltaTime);
		bool didChange = fishOwner.brainEntity.ControlBody(this);

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
		return hookImmuneTime < 0.01f;
	}
}