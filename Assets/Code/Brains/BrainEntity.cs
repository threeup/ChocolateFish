using UnityEngine;
using System;
using System.Collections.Generic;

public class BrainEntity : MonoBehaviour
{
	public TauBody body;
	public bool hasBrain;
	private BrainThinker thinker;
	public bool isInitDone = false;
	
	public Vector3 Pos { get { return body.thisPos; } }
	public Vector2 Dir { get { return body.thisDir; } }



	public void InitializeEntity(TauBody p_body, BrainConfig brainConfig)
	{
		body = p_body;
		hasBrain = brainConfig != null;
		thinker = this.GetComponent<BrainThinker>();
		thinker.InitializeThinker(this, brainConfig);
		isInitDone = true;
	}

	public void BrainUpdate(float deltaTime)
	{
		if (hasBrain)
		{
			thinker.BrainUpdate(deltaTime);
		}
	}

	public bool ControlFishBody(FishBody body)
	{
		Vector3 moveToward = thinker.GetMoveToward();		
		//Vector3 lookToward = thinker.GetLookToward();
		Vector3 moveDiff = moveToward - Pos;

		body.pushDir = (new Vector2(moveDiff.x, moveDiff.y)).normalized;
		

		body.SetForceDir(ForceType.ENGINE, body.pushDir);
		body.SetForce(ForceType.ENGINE, body.pushForce);
		
		bool didChange = false;
		if (body.thisSpeed > body.pushMax)
		{
			didChange = body.SetForceEnabled(ForceType.ENGINE, false);
		}
		else if (body.thisSpeed < body.pushMin)
		{
			didChange = body.SetForceEnabled(ForceType.ENGINE, true);
		}
		return didChange;
	}

	public bool ControlCloudBody(CloudBody body)
	{
		Vector3 moveToward = thinker.GetMoveToward();		
		//Vector3 lookToward = thinker.GetLookToward();
		Vector3 moveDiff = moveToward - Pos;

		body.pushDir = (new Vector2(moveDiff.x, moveDiff.y)).normalized;
		

		body.SetForceDir(ForceType.ENGINE, body.pushDir);
		body.SetForce(ForceType.ENGINE, body.pushForce);
		
		bool didChange = false;
		if (body.thisSpeed > body.pushMax)
		{
			didChange = body.SetForceEnabled(ForceType.ENGINE, false);
		}
		else if (body.thisSpeed < body.pushMin)
		{
			didChange = body.SetForceEnabled(ForceType.ENGINE, true);
		}
		return didChange;
	}
}