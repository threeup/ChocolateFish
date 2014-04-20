using UnityEditor;

[CustomEditor(typeof(TauBody))]
public class ETauBody : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
	}
}