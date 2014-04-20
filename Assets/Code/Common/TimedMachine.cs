using System.Collections.Generic;
using UnityEngine;

public class TimerMachine<T> : BasicMachine<T> where T : struct
{
	private BasicState nextState;
	private BasicState stallState;
	private bool hasNextState = false;
	private bool hasStallState = false;
	private bool hasTimeLock = false;
	public float stallAdvanceTimer = -1.0f;
	public float timeLockTimer = -1.0f;
	
	private string debugString;

	

	public bool HasNext()
	{
		return hasNextState;
	}
	public void ClearNext()
	{
		if (hasNextState)
		{
			hasNextState = false;
		}
	}
	public void SetNext()
	{
		hasNextState = true;
	}

	public bool IsNextState(T tValue)
	{
		return nextState.enumValue.Equals(tValue);
	}
	
	public bool? Change(T ty)
	{
		return Change(GetStateByType(ty));
	}
	public bool? Change(BasicState st)
	{
		bool? tryState = SetState(st);
		if (tryState == false)
		{
			nextState = st;
			SetNext();
		}
		return tryState;
	}

	public bool? DelayChange(T ty)
	{
		return DelayChange(GetStateByType(ty));
	}
	public bool? DelayChange(BasicState st)
	{
		if (hasNextState)
		{
			if (st == nextState)
			{
				return false;
			}
			else
			{
				Debug.Log("DelayChange "+st.enumName+"stomps over"+ToString());
			}
		}

		SetNext();
		nextState = st;
		return false;
	}

	public void ClearStall()
	{
		stallAdvanceTimer = -1;
	}

	public void SetupStall(T ty, float milli)
	{
		SetupStall(GetStateByType(ty), milli);
	}
	public void SetupStall(BasicState st, float milli)
	{
		stallState = st;
		hasStallState = true;
		stallAdvanceTimer = milli;
	}

	public bool? AdvanceStallUpdate(float milli)
	{
		if (hasStallState)
		{
			stallAdvanceTimer -= milli;
			if (stallAdvanceTimer <= 0.01f)
			{
				hasStallState = false;
				return SetState(stallState, true);		
			}		
		}
		return false;
	}

	public bool? CheckQueue(bool forced = false)
	{
		bool? result = false;
		if (hasNextState && !hasTimeLock)
		{
			BasicState savedNext = nextState;
			ClearNext();
			result = SetState(nextState, forced);
			if (result != true)
			{
				SetNext();
				nextState = savedNext;
			}
		}
		return result;
	}

	public void TimeLock(float milli)
	{
		timeLockTimer = Mathf.Max(milli, timeLockTimer);
		hasTimeLock = (timeLockTimer > 0.0f);
	}

	public void TimeLockUpdate(float milli)
	{
		if (hasTimeLock)
		{
			timeLockTimer -= milli;
			hasTimeLock = (timeLockTimer > 0.0f);
		}
	}



	public override string ToString()
	{
		debugString = currentState.enumName;
		if (hasNextState)
		{
			debugString += "{";
			debugString += ">"+nextState.enumName;
			debugString += "}";
		}
		return debugString;
	}
}
