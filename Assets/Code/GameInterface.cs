using UnityEngine;
using System.Text;


public class GameInterface : MonoBehaviour
{
	public Texture progressBackground;
	public Texture progressForeground;
	private Vector2 progressLoc;
	private Vector2 progressSize;

	public void Awake()
	{
		progressLoc = new Vector2(0.02f*Screen.width, 0.01f*Screen.height);
		progressSize = new Vector2(0.5f*Screen.width, 0.01f*Screen.height);
	}
 
	public void OnGUI()
	{
		float progress = GameDirector.Instance.ScorePercentage();
		DrawProgress(progressLoc, progressSize, progress);
		
	}

	private void DrawProgress(Vector2 location, Vector2 size, float progress)
	{

	    GUI.DrawTexture(new Rect(location.x, location.y, size.x, size.y), progressBackground);

	    GUI.DrawTexture(new Rect(location.x, location.y, size.x * progress, size.y), progressForeground);

	}
}