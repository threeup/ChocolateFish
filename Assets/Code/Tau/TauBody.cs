using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum BodyState
{
	LOADING,
	WAITING,
	READY,
	ENTERING,
	MOVING,
	ATTACHED,
	BACKGROUND,
}

[System.Serializable]
public class BodyMachine : BasicMachine<BodyState>
{	
}

public class TauBody : MonoBehaviour
{

	public GameObject go;
	public TauPawn owner;
	public Rigidbody tauRigidbody;
	public TauCollider tauCollider;
	public BodyMachine machine = null;
	public bool isSetup = false;

	public Vector3 thisPos;
	public Quaternion thisRot;
	public Vector3 thisSca;


	public float maxSpeed;
 	public Vector2 thisVel;
 	public float thisSpeed;
	public Vector2 thisAcc;
	public Vector2 thisDir;
	public float thisMass;
	public float dragCoeff;


	public Force[] forces;
	public static int ForceTypeCount;

	public Dictionary<TauBody, Vector3> attachChildren = new Dictionary<TauBody, Vector3>();
	public TauBody attachParent;

	public InputManager.OnInputAxisDelegate HandleAxis;
	public Utilities.OnCollideDelegate HandleCollide;
	public Utilities.OnUpdateDelegate MotorUpdate;
	public Utilities.OnUpdateDelegate PhysicsUpdate;

	public bool isDirty = false;

	public void Setup()
	{
		StartSetup();
		DoSetup();
		FinishSetup();
	}

	public virtual void StartSetup()
	{

		thisPos = go.transform.position;
		thisSca = go.transform.localScale;
	}
	public void DoSetup()
	{
		InitMotion();
		InitMachine();
		if (HandleAxis != null)
		{
			InputManager.Instance.AddInput(HandleAxis);
		}
	}


	public void InitMotion()
	{
		ForceTypeCount = Enum.GetValues(typeof(ForceType)).Length;
		forces = new Force[ForceTypeCount];
		for(int i=0; i<ForceTypeCount; ++i)
		{
			forces[i] = new Force((ForceType)i);
		}
		thisMass = 1f;
		thisVel = Vector2.zero;
		thisAcc = Vector2.zero;
		thisSca = Vector3.one;
		thisDir = -thisPos;
		thisDir.y = 0;
		



		SetForceDir(ForceType.FLOAT, Vector2.up);
		SetForce(ForceType.FLOAT, GameVars.GRAVITY*thisMass);
		SetForceEnabled(ForceType.FLOAT, true);
		SetForceDir(ForceType.GRAVITY, -Vector2.up);
		SetForce(ForceType.GRAVITY, GameVars.GRAVITY*thisMass);
		SetForceEnabled(ForceType.GRAVITY, true);

	}

	public virtual void InitMachine()
	{
		machine = new BodyMachine();
		machine.Initialize(this, typeof(BodyState));
	}

	public virtual void FinishSetup()
	{
		isDirty = true;
		isSetup = true;
	}

	public void Update()
	{
		if (isSetup)
		{
			BodyUpdate(Time.deltaTime);
		}
	}

	public void BodyUpdate(float deltaTime)
	{
		
		if (MotorUpdate != null)
		{
			MotorUpdate(deltaTime);
		}
		if (PhysicsUpdate != null)
		{
			PhysicsUpdate(deltaTime);
		}
		if (isDirty)
		{
			foreach(KeyValuePair<TauBody, Vector3> kvp in attachChildren)
			{
				(kvp.Key as TauBody).MoveTo(thisPos + kvp.Value);
			}
			isDirty = false;
		}
	}

	public void SimplePhysicsUpdate(float deltaTime)
	{	
		CalculateForces();
		if (thisAcc != Vector2.zero) 
		{
			thisVel += thisAcc*deltaTime; // v = u + at + ft
		}
		thisSpeed = thisVel.magnitude;
		if (thisSpeed > 0f)
		{
			Vector3 velDir = thisVel.normalized;
			//thisDir = velDir;
			if (maxSpeed > 0 && thisSpeed > maxSpeed)
			{
				thisVel = velDir * maxSpeed;	
			}
			//machine.SetState(ThreeState.MOVING);
			thisPos = thisPos + Utilities.ToXYVector(thisVel*deltaTime);
			go.transform.position = thisPos;
			isDirty = true;
		}
	}


	public void SetActive(bool act)
	{
		
	}


	public void SyncToThis()
	{
		go.transform.position = thisPos;
		go.transform.rotation = thisRot;
		go.transform.localScale = thisSca;
	}

	public void Stop()
	{
		thisVel = Vector2.zero;
		thisAcc = Vector2.zero;
	}


	public void MoveTo(Vector3? pos)
	{
		thisPos = pos ?? thisPos;
		go.transform.position = thisPos;
	}



	public void SetForce(ForceType ftype, Vector2 fo)
	{
		forces[(int)ftype].fVec = fo;
		forces[(int)ftype].fDir = Vector2.zero;
	}

	public void SetForce(ForceType ftype, float mag)
	{
		forces[(int)ftype].fVec = mag * forces[(int)ftype].fDir;
	}

	public void SetForceDir(ForceType ftype, Vector2 dir)
	{
		forces[(int)ftype].fDir = dir;
	}

	public void SetForceEnabled(ForceType ftype, bool ena)
	{
		forces[(int)ftype].enabled = ena;	
	}

	public bool IsForceEnabled(ForceType ftype)
	{
		return forces[(int)ftype].enabled;	
	}

	public void CalculateDrag()
	{
		//F = 0.5*density*speed*area*dragcoefficient
		if (dragCoeff > 0f)
		{
			//speed * vel = speed*speed * normalied(vel);

			SetForce(ForceType.DRAG, -0.5f*thisSpeed*thisVel*dragCoeff);
			SetForceEnabled(ForceType.DRAG, true);
		}
		else
		{
			SetForceEnabled(ForceType.DRAG, false);
		}
	}

	public void CalculateForces()
	{
		CalculateDrag();
		thisAcc = Vector2.zero;
		for(int i=0; i < ForceTypeCount; ++i )
		{
			if (forces[i].enabled)
			{
				thisAcc += forces[i].fVec;
			}
		}
		if (thisAcc.sqrMagnitude > 0.01)
		{
			thisAcc /= thisMass;
		}
	}




#region ATTACH
	public virtual bool CanAttach()
	{
		return true;
	}

	public void AttachToSelf(TauBody other, Vector3 offs)
	{
		if (!other.CanAttach()) { return; }

		if (thisMass + other.thisMass > 11f)
		{
			PurgeContents(BodyState.MOVING);
		}
		else
		{
			other.MoveTo(thisPos + offs);
			attachChildren.Add(other, offs);
			thisMass += other.thisMass;
			other.machine.SetState(BodyState.ATTACHED);
		}

	}

	public void PurgeContents(BodyState nextState)
	{
		foreach(KeyValuePair<TauBody, Vector3> kvp in attachChildren)
		{
			TauBody jointBody = (kvp.Key as TauBody);
			thisMass -= jointBody.thisMass;
			jointBody.machine.SetState(nextState);
		}
		attachChildren.Clear();
	}
#endregion	
}