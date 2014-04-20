using UnityEngine;
using System.Text;

public enum SpriteState
{
	ON,
	OFF,
}

public class SpriteSequencer : MonoBehaviour
{
	public SpriteRenderer mainRenderer;
	public Sprite[] spriteArray;
	public float[] frameDurations;
	public Vector2 framePosition = Vector2.zero;

	private int currentIndex = 0; //

	private float frameDuration;

	public bool isLooping;
	public bool isAutomatic = false;


	public SpriteState currentSpriteState;
	public delegate void OnChangeDelegate(SpriteState next);
	public OnChangeDelegate OnChange;

	public void Awake()
	{
		if (isAutomatic)
		{
			StartAnim();
		}
		else
		{
			currentSpriteState = SpriteState.OFF;
		}
	}

	public void StartAnim()
	{
		this.gameObject.transform.localPosition = new Vector3(framePosition.x, framePosition.y, 0f);
		currentIndex = 0;
		currentSpriteState = SpriteState.ON;
		mainRenderer.sprite = spriteArray[currentIndex];
		frameDuration = frameDurations[currentIndex];
	}

	
	public void Update()
	{
		if ((currentSpriteState == SpriteState.OFF))
		{
			return;
		}
		frameDuration -= Time.deltaTime;
		if (frameDuration <= 0)
		{
			currentIndex++;
			if (currentIndex == spriteArray.Length)
			{
				if(isLooping)
				{
					currentSpriteState = SpriteState.ON;
					currentIndex = 0;
				}
				else
				{
					currentSpriteState = SpriteState.OFF;
					currentIndex = -99;

					//Debug.Log("off currentIndex"+currentIndex);
					if (OnChange != null)
					{
						OnChange(currentSpriteState);
					}
					return;
				}
			}
			
			
			if (currentSpriteState != SpriteState.OFF)
			{
				mainRenderer.sprite = spriteArray[currentIndex];
				frameDuration = frameDurations[currentIndex];
			}
		}
	}
}