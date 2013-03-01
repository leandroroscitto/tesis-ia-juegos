﻿using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Generador_Escenario))]
public class Generador_MapaEditor : Editor {
   public override void OnInspectorGUI() {
	  serializedObject.Update();

	  base.OnInspectorGUI();

	  EditorGUIUtility.LookLikeInspector();
	  if (GUILayout.Button("Generar")) {
		 EditorUtility.SetDirty(target);
		 ((Generador_Escenario)target).generarEscenario();
	  }
	  if (GUILayout.Button("Resetear")) {
		 EditorUtility.SetDirty(target);
		 ((Generador_Escenario)target).reiniciarEscenario();
	  }
	  if (GUILayout.Button("Borrar Todo")) {
		 EditorUtility.SetDirty(target);
		 ((Generador_Escenario)target).borrarEscenario();
	  }
   }
}