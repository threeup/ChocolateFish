using UnityEditor;

[CustomEditor(typeof(TauPawn))]
public class ETauPawn : Editor
{
	public override void OnInspectorGUI()
	{
		//TauPawn thisTarget = (TauPawn)target;
        //thisTarget.tauType = EditorGUILayout.IntSlider("tauType", thisTarget.tauType, 1, 10);
        DrawDefaultInspector();
	}
}