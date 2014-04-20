using UnityEngine;
using System;
using System.Collections;

public class BoolEventArgs : EventArgs
{
	public bool BoolVal { get; private set; }
	public BoolEventArgs(bool val)
	{
		BoolVal = val;
	}
}


public enum Comparer
{
	LESS_THAN,
	GREATER_THAN,
	LESS_THAN_EQUAL,
	GREATER_THAN_EQUAL,
	EQUAL,
}

public class Condition
{
	public delegate bool ConditionCheck(int input);
	public ConditionCheck check;
	public int source;
	public Comparer compare;
	public event EventHandler<BoolEventArgs> CompareTriggerEvent;


	public void DefineCompare(Comparer comparer, int definedSource)
	{
		source = definedSource;
		compare = comparer;
		switch(comparer)
		{
			case Comparer.LESS_THAN: check = LessThanCompare; break;
			case Comparer.GREATER_THAN: check = GreaterThanCompare; break;
			case Comparer.LESS_THAN_EQUAL: check = LessThanEqualCompare; break;
			case Comparer.GREATER_THAN_EQUAL: check = GreaterThanEqualCompare; break;
			case Comparer.EQUAL: check = EqualCompare; break;
		}
	}

	public bool LessThanCompare(int input) { return input < source; }
	public bool GreaterThanCompare(int input) { return input > source; }
	public bool LessThanEqualCompare(int input) { return input <= source; }
	public bool GreaterThanEqualCompare(int input) { return input >= source; }
	public bool EqualCompare(int input) { return input == source; }

	public void DoCompare(int input)
	{
		if (check(input))
		{
			CompareTriggerEvent(this, new BoolEventArgs(true));
		}
	}
}