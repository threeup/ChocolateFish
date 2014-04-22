using UnityEngine;
using System.Text;

public enum SequenceType
{
	MANUAL,
	IDLE,
	MOVE,
	ATTACHED,
}

[System.Serializable]
public class SpriteSequence
{
	public SequenceType sequenceType;
	public Sprite[] spriteArray;
	public float[] frameDurations;
}

public class SpriteSequencer : MonoBehaviour
{
	public SpriteRenderer mainRenderer;
	public SpriteSequence[] sequences;
	public Vector2 framePosition = Vector2.zero;

	private int currentIndex = 0; //

	private float frameDuration;

	public bool isLooping;
	public bool isAutomatic = false;


	public SpriteSequence currentSequence;
	public delegate void OnChangeDelegate(SequenceType next);
	public OnChangeDelegate OnChange;

	public bool IsManual() { return currentSequence != null && currentSequence.sequenceType == SequenceType.MANUAL; }
	public bool IsIdle() { return currentSequence != null && currentSequence.sequenceType == SequenceType.IDLE; }

	public void Awake()
	{
		currentSequence = null;
		if (isAutomatic)
		{
			StartAnim(SequenceType.IDLE);
		}
		else
		{
			StartAnim(SequenceType.MANUAL);
		}
	}

	public void SwitchToAnim(SequenceType stype)
	{
		if (currentSequence == null || currentSequence.sequenceType != stype)
		{
			StartAnim(stype);
		}
	}

	public void StartAnim(SequenceType stype)
	{
		this.gameObject.transform.localPosition = new Vector3(framePosition.x, framePosition.y, this.gameObject.transform.localPosition.z);
		if (currentSequence == null || currentSequence.sequenceType != stype)
		{
			SpriteSequence nextSequence = System.Array.Find(sequences, x => x.sequenceType == stype);
			if (nextSequence != null)
			{
				currentSequence = nextSequence;
				if (OnChange != null)
				{
					OnChange(currentSequence.sequenceType);
				}
			}
		}
		if (currentSequence != null && !IsManual())
		{
			currentIndex = 0;
			mainRenderer.sprite = currentSequence.spriteArray[currentIndex];
			frameDuration = currentSequence.frameDurations[currentIndex];
		}
		if (IsIdle())
		{
			isLooping = true;
		}
	}

	
	public void Update()
	{
		if (currentSequence == null || IsManual())
		{
			return;
		}
		frameDuration -= Time.deltaTime;
		if (frameDuration <= 0)
		{
			currentIndex++;
			if (currentIndex == currentSequence.spriteArray.Length)
			{
				if(isLooping)
				{
					StartAnim(currentSequence.sequenceType);
				}
				else
				{
					StartAnim(SequenceType.IDLE);
				}
				return;
			}
			
			mainRenderer.sprite = currentSequence.spriteArray[currentIndex];
			frameDuration = currentSequence.frameDurations[currentIndex];
		}
	}
}