using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Generador_Mapa))]
public class Generador_MapaEditor : Editor {
   public override void OnInspectorGUI() {
	  serializedObject.Update();

	  base.OnInspectorGUI();

	  EditorGUIUtility.LookLikeInspector();
	  if (GUILayout.Button("Generar")) {
		 EditorUtility.SetDirty(target);
		 ((Generador_Mapa)target).generarEscenario();
	  }
	  if (GUILayout.Button("Resetear")) {
		 EditorUtility.SetDirty(target);
		 ((Generador_Mapa)target).reiniciarEscenario();
	  }
	  if (GUILayout.Button("Borrar Todo")) {
		 EditorUtility.SetDirty(target);
		 ((Generador_Mapa)target).borrarEscenario();
	  }
	  if (GUILayout.Button("Borrar Waypoints")) {
		 EditorUtility.SetDirty(target);
		 ((Generador_Mapa)target).borrarWaypoints();
	  }
   }
}