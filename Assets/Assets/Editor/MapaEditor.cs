using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Mapa))]
public class MapaEditor : Editor {
   public override void OnInspectorGUI() {
	  serializedObject.Update();

	  base.OnInspectorGUI();

	  EditorGUIUtility.LookLikeInspector();
	  if (GUILayout.Button("Crear TXT")) {
		 ((Mapa)target).crearTXT("./mapa.txt");
	  }
	  if (GUILayout.Button("Generar Mesh")) {
		 ((Mapa)target).crearMesh();
	  }
	  if (GUILayout.Button("Generar NavMesh")) {
		 ((Mapa)target).crearNavegacion();
	  }
   }
}