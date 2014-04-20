using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Force
{
	public ForceType fType;
	public bool enabled;
	public Vector2 fVec; 
	public Vector2 fDir;

	public Force(ForceType ft)
	{
		fType = ft;
	}
}

[System.Serializable]
public enum ForceType
{
	ENGINE,
	DRAG,
	ENVIRO,
	GRAVITY,
	FLOAT,
	TENSION,
}
