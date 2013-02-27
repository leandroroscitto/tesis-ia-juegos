using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ResolucionMDP))]
public class ResolucionMDPEditor : Editor {
   public override void OnInspectorGUI() {
	  serializedObject.Update();

	  base.OnInspectorGUI();

	  EditorGUIUtility.LookLikeInspector();
	  if (GUILayout.Button("Resolver MDP")) {
		 EditorUtility.SetDirty(target);
		 ((ResolucionMDP)target).resolverMDP();
	  }
   }
}