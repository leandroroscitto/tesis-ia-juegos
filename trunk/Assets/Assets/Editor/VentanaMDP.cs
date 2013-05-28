using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using PathRuntime;

public class VentanaMDP : EditorWindow {
   public static VentanaMDP ventana;

   private Generador_MDP generador_mdp;
   private Generador_Jugadores generador_jugadores;
   private Generador_Objetivos generador_objetivos;

   private int jugador_indice = 0;
   private int objetivo_indice = 0;
   private int estado_indice = 0;

   private int cantidad_estados_rango = 25;
   private int rango_estados_seleccionado = 0;
   private int estado_enrango_seleccionado = 0;
   private bool mostrar_informacion = false;
   private bool mostrar_gizmos = false;

   [MenuItem("Window/Debug MDP")]
   static void Init() {
	  ventana = EditorWindow.GetWindow<VentanaMDP>("MDP");
   }

   void OnGUI() {
	  generador_mdp = GameObject.Find("Generadores").GetComponent<Generador_MDP>();
	  generador_jugadores = GameObject.Find("Generadores").GetComponent<Generador_Jugadores>();
	  generador_objetivos = GameObject.Find("Generadores").GetComponent<Generador_Objetivos>();

	  Arbol_Estados arbol_estados = null;
	  MDP<Nodo_Estado, Accion, Objetivo, ResolucionMDP.TransicionJuego, ResolucionMDP.RecompensaJuego> mdp = null;
	  List<Nodo_Estado> estados = null;
	  List<Jugador> jugadores = null;
	  List<Objetivo> objetivos = null;

	  if (generador_mdp != null && generador_jugadores != null && generador_objetivos != null) {
		 arbol_estados = generador_mdp.arbol_estados;
		 if (generador_mdp.resolucion_mdp != null) {
			mdp = generador_mdp.resolucion_mdp.mdp;
		 }
		 if (arbol_estados != null) {
			estados = arbol_estados.estados;
		 }
		 jugadores = generador_jugadores.jugadores;
		 objetivos = generador_objetivos.objetivos;

		 if (mdp != null && mdp.Politica != null && jugadores != null && objetivos != null && estados != null) {
			EditorGUILayout.BeginVertical();

			mostrar_gizmos = EditorGUILayout.Toggle("Mostrar Gizmos", mostrar_gizmos);

			cantidad_estados_rango = EditorGUILayout.IntField("Cantidad por rango: ", cantidad_estados_rango);

			string[] jugadores_menu = new string[jugadores.Count];
			int i = 0;
			foreach (Jugador jugador in jugadores) {
			   jugadores_menu[i] = jugador.nombre + " [" + jugador.control.ToString() + "]";
			   i++;
			}

			string[] objetivos_menu = new string[objetivos.Count];
			i = 0;
			foreach (Objetivo objetivo in objetivos) {
			   objetivos_menu[i] = "Objetivo " + objetivo.nombre + " [" + objetivo.waypoint_asociado + "] " + "(" + objetivo.complementario.nombre + ")";
			   i++;
			}

			if (estados.Count > 0) {
			   if (cantidad_estados_rango > 0) {
				  string[] menu_rango = new string[estados.Count / cantidad_estados_rango + 1];
				  string[][] menu_rango_estados = new string[estados.Count / cantidad_estados_rango + 1][];

				  for (int j = 0; j < estados.Count; j++) {
					 if (menu_rango_estados[j / cantidad_estados_rango] == null) {
						menu_rango_estados[j / cantidad_estados_rango] = new string[Mathf.Min(cantidad_estados_rango, estados.Count - j)];
						menu_rango[j / cantidad_estados_rango] = "Estados " + (j) + " - " + (j + cantidad_estados_rango - 1);
					 }
					 menu_rango_estados[j / cantidad_estados_rango][j % cantidad_estados_rango] = "(" + j + ") " + estados[j].ToString();
				  }

				  mostrar_informacion = EditorGUILayout.Foldout(mostrar_informacion, "Informacion");
				  if (mostrar_informacion) {
					 if (rango_estados_seleccionado < menu_rango.Length) {
						rango_estados_seleccionado = EditorGUILayout.Popup("Rango: ", rango_estados_seleccionado, menu_rango);
					 }
					 else {
						rango_estados_seleccionado = 0;
					 }

					 if (estado_enrango_seleccionado < menu_rango_estados[rango_estados_seleccionado].Length) {
						estado_enrango_seleccionado = EditorGUILayout.Popup("Estado: ", estado_enrango_seleccionado, menu_rango_estados[rango_estados_seleccionado]);
					 }
					 else {
						estado_enrango_seleccionado = 0;
					 }

					 estado_indice = estados[rango_estados_seleccionado * cantidad_estados_rango + estado_enrango_seleccionado].id;
					 jugador_indice = EditorGUILayout.Popup("Jugador: ", jugador_indice, jugadores_menu);
					 objetivo_indice = EditorGUILayout.Popup("Objetivo: ", objetivo_indice, objetivos_menu);

					 Accion accion = mdp.Politica[jugador_indice][objetivo_indice][estado_indice];
					 EditorGUILayout.LabelField("Politica: ", "(" + accion.id + ") " + jugadores[accion.actor_id].nombre + ", " + "[" + accion.origen + " => " + accion.destino + "]");
				  }
			   }
			}

			EditorGUILayout.EndVertical();
		 }
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
	  if (mostrar_gizmos) {
		 generador_mdp = GameObject.Find("Generadores").GetComponent<Generador_MDP>();
		 generador_jugadores = GameObject.Find("Generadores").GetComponent<Generador_Jugadores>();
		 generador_objetivos = GameObject.Find("Generadores").GetComponent<Generador_Objetivos>();

		 Arbol_Estados arbol_estados = null;
		 MDP<Nodo_Estado, Accion, Objetivo, ResolucionMDP.TransicionJuego, ResolucionMDP.RecompensaJuego> mdp = null;
		 List<Nodo_Estado> estados = null;
		 List<Jugador> jugadores = null;
		 List<Objetivo> objetivos = null;

		 if (generador_mdp != null && generador_jugadores != null && generador_objetivos != null) {
			arbol_estados = generador_mdp.arbol_estados;
			if (generador_mdp.resolucion_mdp != null) {
			   mdp = generador_mdp.resolucion_mdp.mdp;
			}
			if (arbol_estados != null) {
			   estados = arbol_estados.estados;
			}
			jugadores = generador_jugadores.jugadores;
			objetivos = generador_objetivos.objetivos;

			if (mdp != null && mdp.Politica != null && jugadores != null && objetivos != null && estados != null) {
			   Nodo_Estado estado_actual = estados[estado_indice];
			   Accion accion_actual = mdp.Politica[jugador_indice][objetivo_indice][estado_indice];
			   int i = 0;
			   while (estado_actual.estado_juego.posicion_jugadores[jugador_indice] != objetivos[objetivo_indice].posicion && i < generador_mdp.arbol_estados.acciones.Count) {
				  Vector3 origen = accion_actual.origen.Position;
				  Vector3 destino = accion_actual.destino.Position;
				  Vector3 centro = (origen + destino) / 2;

				  Handles.color = Color.Lerp(Color.red, Color.yellow, i * 1f / Navigation.Waypoints.Count);
				  Handles.DrawWireArc(centro, Vector3.Cross(destino - origen, Vector3.down).normalized, origen - centro, 180, Vector3.Distance(origen, destino) / 2);

				  estado_actual = estado_actual.hijoAccion(accion_actual) as Nodo_Estado;
				  accion_actual = mdp.Politica[jugador_indice][objetivo_indice][estado_actual.id];
				  i++;

				  if (accion_actual.origen == accion_actual.destino) {
					 break;
				  }
			   }
			}
		 }
	  }
   }
}