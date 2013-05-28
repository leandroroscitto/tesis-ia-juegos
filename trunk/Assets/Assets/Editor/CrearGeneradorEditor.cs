using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CrearGenerador))]
public class CrearGeneradorEditor : Editor {
   public override void OnInspectorGUI() {
	  base.OnInspectorGUI();

	  if (GUILayout.Button("Crear Escena")) {
		 ((CrearGenerador)target).crearEscena();
	  }
   }
}