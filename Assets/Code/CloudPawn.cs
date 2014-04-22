using UnityEngine;
using System.Text;


public class CloudPawn : TauPawn
{
	public CloudBody cloudBody;

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


	public void ReturnToSpawner()
	{
		SetActive(false);
		GameSpawner spawner = GameDirector.Instance.NearestSpawner(body.thisPos);
		spawner.AddToPool(this.gameObject);
	}
	
}