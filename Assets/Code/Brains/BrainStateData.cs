using UnityEngine;
using System;
using System.Collections.Generic;

public class BrainStateData : MonoBehaviour
{
	public float timeFromFear;
	public float timeFromBait;
	public bool wasFear;
	public Vector3 lastPosition;
	public Vector3 lastDirection;
	public Vector3 lastMoveTarget;
	public Vector3 lastRandom;
	public BrainEntity lastFear;
	public BrainEntity lastBait;
	public BrainEntity currentTargetEntity;
	public Vector3 currentMoveTarget;
	public float escapeY;
	public BrainThinker owner;


	public void InitializeStateData(BrainThinker b)
	{
		owner = b;
		Reset();
	}

	public void Reset()
	{
		timeFromFear = 0f;
		timeFromBait = 0f;
		lastPosition = Vector3.zero;
		lastMoveTarget = Vector3.zero;
		lastRandom = Vector3.zero;
		escapeY = -2f;
	}
	
	public void StateUpdate(float deltaTime)
	{
		timeFromFear += deltaTime;
		timeFromBait += deltaTime;
		lastPosition = owner.brainEntity.Pos;
		lastDirection = owner.brainEntity.Dir;
	}

	public void Goal()
	{
		escapeY = UnityEngine.Random.Range(-7f, -21f);
	}

	public void Scan()
	{
		
		//lastMoveTarget = currentMoveTarget;
		//currentMoveTarget = BrainActions.MoveForward(this);
		
	}

}