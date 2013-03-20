using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CrearGenerador))]
public class CrearGeneradorEditor : Editor {
   public override void OnInspectorGUI() {
	  base.OnInspectorGUI();

	  if (GUILayout.Button("Crear Generadores")) {
		 ((CrearGenerador)target).crearGenerador();
	  }
   }
}