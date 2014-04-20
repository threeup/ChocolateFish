using UnityEngine;
using System.Text;


public class FishPawn : TauPawn
{
	public FishBody fishBody;

	public override void StartSetup()
	{
		base.StartSetup();
		brainConfig = new BrainConfig();
	}
	
	public override void FinishSetup()
	{
		machine.SetState(ObjectState.ACTIVE);	
		base.FinishSetup();
	}

	public override void Update()
	{
		base.Update();
	}

	public void SetHook(float y)
	{
	
	}

	public void ReturnToSpawner()
	{
		SetActive(false);
		GameSpawner spawner = GameDirector.Instance.NearestSpawner(body.thisPos);
		spawner.AddToPool(this.gameObject);
	}
	
}