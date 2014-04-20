using UnityEditor;

[CustomEditor(typeof(BoatPawn))]
public class EBoatPawn : ETauPawn
{
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();
		DrawDefaultInspector();
	}
}
