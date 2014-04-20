using UnityEngine;
using System.Text;
using System.Collections;

public class GameSpawner : MonoBehaviour
{
	public GameObject objectPrefab;

	public Queue objectQueue;

	public Vector3 forward;
	private Vector3 objectScale;
	public Vector3 spawnerScale;


	private float timer = 0f;

	public void Awake()
	{
		objectQueue = new Queue();
		
		FillPool(10);
	}

	public void FillPool(int len)
	{
		Vector3 spawnPos = this.transform.position;
		Quaternion spawnQrt = this.transform.rotation;
		for(int i=0; i<len; ++i)
		{
			GameObject obj = Instantiate(objectPrefab, spawnPos, spawnQrt) as GameObject;
			obj.SetActive(false);

			obj.transform.parent = this.transform;
			objectQueue.Enqueue(obj);
			objectScale = obj.transform.localScale;

		}	
	}

	public void AddToPool(GameObject obj)
	{
		obj.SetActive(false);
		obj.transform.parent = this.transform;
		Destroy(obj);
		//objectQueue.Enqueue(obj);
	}

	void Update()
	{
		timer -= Time.deltaTime;
		if (timer < 0f)
		{
			Spawn();
			Reposition();
			timer = UnityEngine.Random.Range(2f,6f);
		}
	}

	public void Spawn()
	{
		if (objectQueue.Count < 2)
		{
			FillPool(10);
		}
		GameObject obj = objectQueue.Dequeue() as GameObject;
		obj.transform.parent = null;
		obj.transform.position = this.transform.position;
		obj.transform.eulerAngles = forward;
		obj.transform.localScale = Vector3.Scale(objectScale, spawnerScale);
		FishPawn pawn = obj.GetComponent<FishPawn>();
		if (pawn != null)
		{
			pawn.fishBody.Setup();
			pawn.Setup();
			pawn.fishBody.machine.SetState(BodyState.WAITING);
			pawn.fishBody.machine.SetState(BodyState.READY);
		}
	}

	private void Reposition()
	{
		Vector3 pos = this.transform.position;
		pos.y = UnityEngine.Random.Range(GameVars.FISH_SPAWN.yMin,GameVars.FISH_SPAWN.yMax);
		this.transform.position = pos;

	}
}