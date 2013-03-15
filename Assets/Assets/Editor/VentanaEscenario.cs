using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using PathRuntime;

public class VentanaEscenario : EditorWindow {
   public static VentanaEscenario ventana;

   private bool mostrar_gizmos;

   private Generador_Objetivos generador_objetivos;
   private Generador_Mapa generador_mapa;

   [MenuItem("Window/Debug Escenario")]
   static void Init() {
	  ventana = EditorWindow.GetWindow<VentanaEscenario>("Escenario");
   }

   void OnGUI() {
	  mostrar_gizmos = GUILayout.Toggle(mostrar_gizmos, "Mostrar Gizmos:");
   }


   void OnFocus() {
	  SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

	  SceneView.onSceneGUIDelegate += this.OnSceneGUI;
   }

   void OnDestroy() {
	  SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
   }

   void OnSceneGUI(SceneView sceneView) {
	  if (mostrar_gizmos) {
		 Handles.BeginGUI();
		 Color color_actual = GUI.contentColor;
		 GUI.contentColor = Color.yellow;
		 generador_objetivos = GameObject.Find("Generadores").GetComponent<Generador_Objetivos>();
		 if (generador_objetivos != null && generador_objetivos.objetivos != null) {
			foreach (Objetivo objetivo in generador_objetivos.objetivos) {
			   imprimirLabel(objetivo.posicion, "Objetivo_" + objetivo.nombre, sceneView.camera);
			}
		 }
		 GUI.contentColor = color_actual;
		 Handles.EndGUI();
	  }
   }

   public void imprimirLabel(Vector3 posicion_mundo, string label, Camera camara) {
	  Vector3 posicion_pantalla = camara.WorldToScreenPoint(posicion_mundo);
	  GUI.Label(new Rect(posicion_pantalla.x, camara.pixelHeight - posicion_pantalla.y, label.Length * 10, 20), label);
   }
}