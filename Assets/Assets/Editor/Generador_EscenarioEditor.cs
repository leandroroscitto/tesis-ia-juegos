using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Generador_Escenario))]
public class Generador_EscenarioEditor : Editor {
   public override void OnInspectorGUI() {
	  base.OnInspectorGUI();

	  EditorGUIUtility.LookLikeInspector();
	  if (GUILayout.Button("Generar")) {
		 ((Generador_Escenario)target).generarEscenario();
	  }
	  if (GUILayout.Button("Resetear")) {
		 ((Generador_Escenario)target).reiniciarEscenario();
	  }
	  if (GUILayout.Button("Borrar Todo")) {
		 ((Generador_Escenario)target).borrarEscenario();
	  }
	  if (GUILayout.Button("Guardar Datos")) {
		 ((Generador_Escenario)target).guardarDatos();
	  }
	  if (GUILayout.Button("Cargar Datos")) {
		 ((Generador_Escenario)target).cargarDatos();
	  }

	  //EditorUtility.SetDirty(target);
	  if (VentanaArbolEstado.ventana != null) {
		 EditorUtility.SetDirty(VentanaArbolEstado.ventana);
		 VentanaArbolEstado.ventana.Repaint();
	  }

	  if (VentanaMDP.ventana != null) {
		 EditorUtility.SetDirty(VentanaMDP.ventana);
		 VentanaMDP.ventana.Repaint();
	  }

	  if (VentanaEscenario.ventana != null) {
		 EditorUtility.SetDirty(VentanaEscenario.ventana);
		 VentanaEscenario.ventana.Repaint();
	  }
   }
}