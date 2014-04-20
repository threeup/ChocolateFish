using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class BrainWorld : BasicSingleton<BrainWorld>
{
	public List<BrainEntity> entities = new List<BrainEntity>();

	public BrainWorld()
	{

	}

	public BrainEntity FindNearestPlayer(BrainEntity src)
	{
		return entities[0];
	}

	public void RegisterObject(TauPawn pawn)
	{
		pawn.machine.AddEnterListener((int)ObjectState.READY, OnAddPawn);
	}

	public void OnAddPawn(object obj)
	{
		TauPawn pawn = obj as TauPawn;
		if (pawn != null)
		{
			entities.Add(pawn.brainEntity);
		}
	}
}