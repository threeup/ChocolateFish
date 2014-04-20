using UnityEngine;
using System;
using System.Collections.Generic;

public delegate Vector3 PositionMethod(BrainStateData state);
public delegate bool BoolMethod(BrainStateData state);


public struct BrainDecision
{
	public string thought;
	public PositionMethod moveToward;
	public PositionMethod lookToward;
}

public class BrainConsts
{
	public const float MIDRANGE = 10.0f;  // min distance for mid range, for gun switching
	public const float LONGRANGE = 50.0f;  // min distance for long range, for gun switching
	public const float SUPERFAR = 100000.0f;

}

public class BrainActions
{
	public delegate BrainDecision BrainActionMethod();
    public delegate Vector3 TargetSelectMethod(BrainStateData state);

	public static BrainEntity NearestTarget(BrainStateData state)
	{
		return BrainWorld.Instance.FindNearestPlayer(state.owner.brainEntity);
	}

	public static Vector3 MoveForward(BrainStateData state)
    {
        return state.lastPosition + 10*state.lastDirection;
    }

    public static Vector3 MoveEscape(BrainStateData state)
    {
    	Vector3 away = Vector3.zero;
    	away.x = state.lastPosition.x - 30.0f*Mathf.Sign(state.lastDirection.x);
    	away.y = state.escapeY;

        return away;
    }

	public static BrainDecision Forward()
	{
		BrainDecision decision;
		decision.moveToward = BrainActions.MoveForward;
		decision.lookToward = BrainActions.MoveForward;
		decision.thought = "Forward";
		return decision;
	}

	public static BrainDecision Escape()
	{
		BrainDecision decision;
		decision.moveToward = BrainActions.MoveEscape;
		decision.lookToward = BrainActions.MoveForward;
		decision.thought = "Escape";
		return decision;
	}
}




public enum BrainTargetType
{
	FORWARD,
	NEAREST,
	ESCAPE,    
	HUNGRY,  
}
[System.Serializable]
public class BrainConfig
{
	public int fear = 50;  // higher fear is more likely to feel in danger -> dont chase
	public int aggression = 50; // higher aggression is more likely to feel bored -> dont hide
	public int focus = 50; // higher focus is more likely to stay on target based on targettype
	public int fitness = 50; // higher fitness has less time stopped between points
	public float scanRate = 0.2f;
	public float goalRate = 2f;
	public float decideRate = 4f;
	public BrainTargetType targetType = BrainTargetType.FORWARD;
}

public class BrainThinker : MonoBehaviour
{
	public BrainEntity brainEntity;
	private BrainConfig config;
	private BrainStateData currentState;

	public BrainDecision decision;
	public string thought;

	public float scanDuration = -1f;
	public float goalDuration = -1f;
	public float decideDuration = -1f;

	public bool CanChange { get { return decideDuration < 0f; } }
	public bool CanGoal { get { return goalDuration < 0f; } }
	public bool CanScan { get { return scanDuration < 0f; } }


	public void InitializeThinker(BrainEntity p_brainEntity, BrainConfig p_config)
	{
		brainEntity = p_brainEntity;
		if (p_config == null)
		{
			decision = BrainActions.Forward();
			return;
		}
		currentState = this.GetComponent<BrainStateData>();
		currentState.InitializeStateData(this);
		config = p_config;

		Decide();
		currentState.Goal();
		currentState.Scan();
	}

	public void Decide()
	{
		 //config.targetType = BrainTargetType.ESCAPE;
		 switch(config.targetType)
		 {
		 	case BrainTargetType.FORWARD:
		 	{
		 		decision = BrainActions.Forward();
		 		break;
		 	}
		 	case BrainTargetType.ESCAPE:
		 	{
		 		decision = BrainActions.Escape();
		 		break;
		 	}
		 }
		 thought = decision.thought;
	}


	public void BrainUpdate(float deltaTime)
	{
		if (decideDuration > 0f) { decideDuration -= deltaTime; }
		if (scanDuration > 0f) { scanDuration -= deltaTime; }
		
		if (CanChange)
		{
			decideDuration = config.decideRate;
			Decide();
		}
		if (CanGoal)
		{
			goalDuration = config.scanRate;
			currentState.Goal();
		}
		if (CanScan)
		{
			scanDuration = config.scanRate;
			currentState.Scan();
		}
		currentState.StateUpdate(deltaTime);

#if UNITY_EDITOR
		Debug.DrawRay(transform.position, this.GetLookToward() - transform.position, Color.cyan);
		Debug.DrawRay(transform.position, this.GetMoveToward() - transform.position, Color.yellow);
#endif
	}


	protected void SlowUpdate(float deltaTime)
	{

		
	}
	

	public Vector3 GetMoveToward()
    {
		Vector3 movePos = decision.moveToward(currentState);
		//currentState.lastMoveTarget = movePos;
		return movePos;
    }
    public Vector3 GetLookToward()
    {
        return decision.lookToward(currentState);
    }

    
}