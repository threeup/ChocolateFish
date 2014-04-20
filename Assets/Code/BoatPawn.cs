using UnityEngine;
using System.Text;


public class BoatPawn : TauPawn
{
	public BoatBody boatBody;
	public int player = 1;
	public GameObject hookPrototype;
	public HookBody hook;

	public override void FinishSetup()
	{
		machine.SetState(ObjectState.ACTIVE);	
		base.FinishSetup();
	}


	public override void Update()
	{
		if (hook != null)
		{

		}
		base.Update();
	}

	public void SetHook(float y)
	{
		if (hook == null && y < 0)
		{
			Vector3 hookPos = this.transform.position;
			hookPos.z = -3;
			GameObject hookObj = Instantiate(hookPrototype, hookPos, Quaternion.identity) as GameObject;
			hook = hookObj.GetComponent<HookBody>();
			hook.Setup();
			hook.boatOwner = this;
			hookObj.SetActive(true);
		}
		
	}

	
}