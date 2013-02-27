using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Arbol_Estados))]
public class Arbol_EstadosEditor : Editor {
   public override void OnInspectorGUI() {
	  serializedObject.Update();

	  base.OnInspectorGUI();

	  EditorGUIUtility.LookLikeInspector();
	  if (GUILayout.Button("Preparar Estados")) {
		 EditorUtility.SetDirty(target);
		 ((Arbol_Estados)target).prepararEstados();
	  }
   }
}