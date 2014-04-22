using UnityEngine;
using System.Text;
using System.Collections;

public class GameSpawner : MonoBehaviour
{
	public GameObject objectPrefab;

	public Queue objectQueue;

	public Vector3 forward;
	private Vector3 objectScale;
	private Vector3 spawnerScale;

	public bool isLeftToRight = true;
	public bool isUnderwater = true;
	public float spawnerMinY = 0.0f;
	public float spawnerMaxY = 1.0f;
	public float spawnRateMin = 2f;
	public float spawnRateMax = 6f;


	private float timer = 0f;

	public void Awake()
	{
		objectQueue = new Queue();
		spawnerScale.x = isLeftToRight ? -1f : 1f;
		spawnerScale.y = 1f;
		spawnerScale.z = 1f;
		Reposition();
		

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
			timer = UnityEngine.Random.Range(spawnRateMin,spawnRateMax);
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
		TauPawn pawn = obj.GetComponent<TauPawn>();
		if (pawn != null)
		{
			pawn.body.Setup();
			pawn.Setup();
			pawn.body.machine.SetState(BodyState.WAITING);
			pawn.body.machine.SetState(BodyState.READY);
		}
	}

	private void Reposition()
	{
		Vector3 pos = this.transform.position;
		pos.x = isLeftToRight ? GameVars.FISH_SPAWN.x : -GameVars.FISH_SPAWN.x;
		float min = isUnderwater ? GameVars.FISH_SPAWN.yMin : 0;
		float max = isUnderwater ? GameVars.FISH_SPAWN.yMax : 7;
		pos.y = UnityEngine.Random.Range(
					Mathf.Lerp(min, max, spawnerMinY),
					Mathf.Lerp(min, max, spawnerMaxY));
		this.transform.position = pos;

	}
}