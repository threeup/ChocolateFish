using UnityEngine;
using System.Text;


public class GameDirector : MonoBehaviour
{
	public BoatPawn[] boats;
	public GameSpawner[] spawners;
	public GameObject prototypes;
	public static GameDirector Instance;
	public int score = 0;



	public void Awake()
	{
		Instance = this;
		prototypes.SetActive(false);
		InputManager.Instance.Setup();
		foreach(BoatPawn boat in boats)
		{
			boat.boatBody.Setup();
			boat.Setup();
		}
	}

	public GameSpawner NearestSpawner(Vector2 pos)
	{
		GameSpawner best = null;
		float bestMag = Utilities.SUPERFAR;
		for(int i=0; i<spawners.Length; ++i)
		{
			GameSpawner spawner = spawners[i];
			float sqrMag = Utilities.SqrMag(pos, spawner.transform.position);
			if (sqrMag < bestMag)
			{
				bestMag = sqrMag;
				best = spawner;
			}
		}
		return best;
	}

	public void Score(int val)
	{
		score += val;
	}
}