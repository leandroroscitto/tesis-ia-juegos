using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using PathRuntime;

public class VentanaEscenario : EditorWindow {
   public static VentanaEscenario ventana;

   private bool mostrar_gizmos;

   private Generador_Objetivos generador_objetivos;

   private int origen, destino;

   [MenuItem("Window/Debug Escenario")]
   static void Init() {
	  ventana = EditorWindow.GetWindow<VentanaEscenario>("Escenario");
   }

   void OnGUI() {
	  generador_objetivos = GameObject.Find("Generadores").GetComponent<Generador_Objetivos>();
	  if (generador_objetivos != null) {
		 mostrar_gizmos = GUILayout.Toggle(mostrar_gizmos, "Mostrar Gizmos");
	  }
	  if (Generador_Navegacion.caminos != null) {
		 GUILayout.Space(10);
		 GUILayout.Label("Pathfinding");

		 string[] waypoints = new string[Navigation.Waypoints.Count];
		 for (int i = 0; i < Navigation.Waypoints.Count; i++) {
			waypoints[i] = Navigation.Waypoints[i].name;
		 }
		 origen = EditorGUILayout.Popup("Origen: ", origen, waypoints);
		 destino = EditorGUILayout.Popup("Destino: ", destino, waypoints);

		 GUILayout.Label("Distancia: " + Generador_Navegacion.getMinimaDistancia(Navigation.Waypoints[origen].Position, Navigation.Waypoints[destino].Position));
	  }
   }


   void OnFocus() {
	  SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

	  SceneView.onSceneGUIDelegate += this.OnSceneGUI;
   }

   void OnDestroy() {
	  SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
   }

   void OnSceneGUI(SceneView sceneView) {
	  if (mostrar_gizmos && Generador_Navegacion.caminos != null) {
		 Handles.color = Color.red;
		 List<Vector3> camino = Generador_Navegacion.getMinimoCamino(Navigation.Waypoints[origen].Position, Navigation.Waypoints[destino].Position);
		 for (int i = 0; i < camino.Count - 1; i++) {
			Handles.DrawLine(camino[i], camino[i + 1]);
		 }

		 Handles.BeginGUI();
		 Color color_actual = GUI.contentColor;
		 GUI.contentColor = Color.yellow;
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