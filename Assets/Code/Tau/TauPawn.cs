using UnityEngine;
using System.Text;
using System.Collections.Generic;

public enum ObjectState
{
	LOADING,
	READY,
	ACTIVE,
}

[System.Serializable]
public class ObjectMachine : BasicMachine<ObjectState>
{	
}

public class TauPawn : MonoBehaviour
{

	public TauBody body;
	public BrainConfig brainConfig;
	public BrainEntity brainEntity;
	public ObjectMachine machine = null;

	public int tauID = 1;

	public GameObject drawObject;
	public List<Renderer> renderList;

	public InputManager.OnInputAxisDelegate HandleAxis;
	public Utilities.OnUpdateDelegate MotorUpdate;
	public Utilities.OnUpdateDelegate PhysicsUpdate;

	public int tauType = 2;

	public void Awake()
	{
		drawObject = this.gameObject;
		renderList = new List<Renderer>();
	}

	public void Setup()
	{
		StartSetup();
		DoSetup();
		FinishSetup();
	}

	public virtual void StartSetup()
	{
		brainConfig = null;
	}
	public virtual void DoSetup()
	{
		InitMachine();
		brainEntity.InitializeEntity(body, brainConfig);
		BrainWorld.Instance.RegisterObject(this);
		machine.SetState(ObjectState.READY);
	}
	public void InitMachine()
	{
		if (machine == null)
		{
			machine = new ObjectMachine();
		}
		machine.Initialize(this, typeof(ObjectState));
		machine[(int)ObjectState.READY].CanEnter = CanReady;
		machine.AddEnterListener((int)ObjectState.READY, OnReady);
		machine.AddEnterListener((int)ObjectState.ACTIVE, OnActive);
	}
	public virtual void FinishSetup()
	{
		SetActive(true);
	}

	public virtual void Update()
	{
		//machine.CheckQueue();
	}

	public bool CanReady()
	{
		return true;
	}

	public void OnReady(object owner)
	{

	}

	public void OnActive(object owner)
	{

	}

	public virtual void SetActive(bool act)
	{
		SetVisible(act);
		gameObject.SetActive(act);
		if (body != null)
		{
			body.SetActive(act);
		}
	}

	public void SetVisible(bool vis)
	{
		if (drawObject != null)
		{
			if (drawObject.renderer != null)
			{
				drawObject.renderer.enabled = vis; 
			}
			if (renderList != null)
			{
				foreach(Renderer r in renderList)
				{
					r.enabled = vis; 
				}
			}
		}
	}

	public bool DisableSelf()
	{
		this.SetVisible(false);
		return true;
	}


}