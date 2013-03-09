using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using PathRuntime;

public class VentanaArbolEstado : EditorWindow {
   public static VentanaArbolEstado ventana;

   private Generador_MDP generador_mdp;
   private Generador_Jugadores generador_jugadores;
   private Generador_Objetivos generador_objetivos;

   private bool mostrar_gizmos = true;

   private int cantidad_estados_rango = 10;

   private bool mostrar_estados = true;
   private int rango_estados_seleccionado = 0;
   private int estado_enrango_seleccionado = 0;
   private int estado_id_seleccionado = 0;

   private bool mostrar_hijos = true;
   private int rango_hijos_seleccionado = 0;
   private int hijo_enrango_seleccionado = 0;
   private int hijo_indice_seleccionado = 0;

   private bool mostrar_padres;
   private int rango_padres_seleccionado;
   private int padre_enrango_seleccionado;
   private int padre_indice_seleccionado = 0;

   [MenuItem("Window/Debug Arbol de Estados")]
   static void Init() {
	  ventana = EditorWindow.GetWindow<VentanaArbolEstado>("A. de Estados");
   }

   void OnGUI() {
	  generador_mdp = GameObject.Find("Generadores").GetComponent<Generador_MDP>();
	  generador_jugadores = GameObject.Find("Generadores").GetComponent<Generador_Jugadores>();
	  generador_objetivos = GameObject.Find("Generadores").GetComponent<Generador_Objetivos>();

	  if (generador_mdp != null && generador_jugadores != null && generador_objetivos != null && generador_mdp.arbol_estados != null && generador_mdp.arbol_estados.estados != null) {
		 Arbol_Estados arbol_estados = generador_mdp.arbol_estados;

		 mostrar_gizmos = EditorGUILayout.Toggle("Mostrar Gizmos: ", mostrar_gizmos);

		 EditorGUILayout.IntField("Cantidad de waypoints: ", Navigation.Waypoints.Count);
		 EditorGUILayout.IntField("Cantidad de estados: ", arbol_estados.estados.Count);
		 cantidad_estados_rango = EditorGUILayout.IntField("Cantidad por rango: ", cantidad_estados_rango);

		 int cantidad_estados = arbol_estados.estados.Count;
		 List<Nodo_Estado> estados = arbol_estados.estados;

		 EditorGUILayout.Separator();
		 mostrar_estados = EditorGUILayout.Foldout(mostrar_estados, "Seleccion de estado");
		 if (mostrar_estados) {
			EditorGUILayout.BeginVertical();

			if (cantidad_estados > 0) {
			   if (cantidad_estados_rango > 0) {
				  string[] menu_rango = new string[cantidad_estados / cantidad_estados_rango + 1];
				  string[][] menu_rango_estados = new string[cantidad_estados / cantidad_estados_rango + 1][];

				  for (int i = 0; i < cantidad_estados; i++) {
					 if (menu_rango_estados[i / cantidad_estados_rango] == null) {
						menu_rango_estados[i / cantidad_estados_rango] = new string[Mathf.Min(cantidad_estados_rango, cantidad_estados - i)];
						menu_rango[i / cantidad_estados_rango] = "Estados " + (i) + " - " + (i + cantidad_estados_rango - 1);
					 }
					 menu_rango_estados[i / cantidad_estados_rango][i % cantidad_estados_rango] = "(" + i + ") " + estados[i].ToString();
				  }

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

				  estado_id_seleccionado = estados[rango_estados_seleccionado * cantidad_estados_rango + estado_enrango_seleccionado].id;

				  Nodo_Estado estado = estados[estado_id_seleccionado];

				  EditorGUILayout.Space();
				  EditorGUILayout.LabelField("Informacion:");
				  foreach (Jugador jugador in generador_jugadores.jugadores) {
					 EditorGUILayout.LabelField(jugador.nombre + " en la posicion: " + Navigation.GetNearestNode(estado.estado_actual.posicion_jugadores[jugador.id]) + ".");
				  }

				  EditorGUILayout.Space();
				  string cumplidos = "";
				  foreach (int objetivo_id in estado.estado_actual.objetivos_cumplidos) {
					 cumplidos += generador_objetivos.objetivos[objetivo_id].nombre + ", ";
				  }

				  EditorGUILayout.LabelField("Objetivos cumplidos: ", cumplidos);

				  string no_cumplidos = "";
				  foreach (int objetivo_id in estado.estado_actual.objetivos_no_cumplidos) {
					 no_cumplidos += generador_objetivos.objetivos[objetivo_id].nombre + ", ";
				  }

				  EditorGUILayout.LabelField("Objetivos no cumplidos: ", no_cumplidos);
			   }
			   else {
				  EditorGUILayout.LabelField("La cantidad de estados por rango debe estar entre 1 y la cantidad de estados.");
			   }
			}
			else {
			   estado_enrango_seleccionado = -1;
			   EditorGUILayout.LabelField("No existen estados.");
			}
			EditorGUILayout.EndVertical();
		 }

		 int cantidad_estados_hijos = estados[estado_id_seleccionado].estados_hijos.Count;
		 List<Nodo_Estado> estados_hijos = estados[estado_id_seleccionado].estados_hijos;

		 EditorGUILayout.Separator();
		 mostrar_hijos = EditorGUILayout.Foldout(mostrar_hijos, "Seleccion de hijo");
		 if (mostrar_hijos) {
			EditorGUILayout.BeginVertical();

			if (cantidad_estados_hijos > 0) {
			   EditorGUILayout.LabelField(estados[estado_id_seleccionado].ToString());
			   if (cantidad_estados_rango > 0) {
				  string[] menu_rango = new string[cantidad_estados_hijos / cantidad_estados_rango + 1];
				  string[][] menu_rango_estados = new string[cantidad_estados_hijos / cantidad_estados_rango + 1][];

				  for (int i = 0; i < cantidad_estados_hijos; i++) {
					 if (menu_rango_estados[i / cantidad_estados_rango] == null) {
						menu_rango_estados[i / cantidad_estados_rango] = new string[Mathf.Min(cantidad_estados_rango, cantidad_estados_hijos - i)];
						menu_rango[i / cantidad_estados_rango] = "Estados " + (i) + " - " + (i + cantidad_estados_rango - 1);
					 }
					 menu_rango_estados[i / cantidad_estados_rango][i % cantidad_estados_rango] = "(" + i + ") " + estados_hijos[i].ToString();
				  }

				  rango_hijos_seleccionado = EditorGUILayout.Popup("Rango: ", rango_hijos_seleccionado, menu_rango);
				  hijo_enrango_seleccionado = EditorGUILayout.Popup("Estado: ", hijo_enrango_seleccionado, menu_rango_estados[rango_hijos_seleccionado]);
				  if (hijo_enrango_seleccionado != -1) {
					 hijo_indice_seleccionado = rango_hijos_seleccionado * cantidad_estados_rango + hijo_enrango_seleccionado;

					 Accion accion = estados[estado_id_seleccionado].acciones_hijos[hijo_indice_seleccionado];
					 EditorGUILayout.LabelField("A traves de la accion " + "(" + accion.id + ")" + " [" + accion.origen + " => " + accion.destino + "]" + " de " + accion.jugador.nombre);

					 if (GUILayout.Button("Seleccionar estado")) {
						estado_id_seleccionado = estados_hijos[hijo_indice_seleccionado].id;
						estado_enrango_seleccionado = estado_id_seleccionado % cantidad_estados_rango;
						rango_estados_seleccionado = estado_id_seleccionado / cantidad_estados_rango;
					 }
				  }
			   }
			   else {
				  EditorGUILayout.LabelField("La cantidad de estados por rango debe estar entre 1 y la cantidad de estados.");
			   }
			}
			else {
			   hijo_enrango_seleccionado = -1;
			   EditorGUILayout.LabelField("No existen hijos");
			}
			EditorGUILayout.EndVertical();
		 }

		 int cantidad_estados_padres = estados[estado_id_seleccionado].estados_padres.Count;
		 List<Nodo_Estado> estados_padres = estados[estado_id_seleccionado].estados_padres;

		 EditorGUILayout.Separator();
		 mostrar_padres = EditorGUILayout.Foldout(mostrar_padres, "Seleccion de padre");
		 if (mostrar_padres) {
			EditorGUILayout.BeginVertical();
			if (cantidad_estados_padres > 0) {
			   EditorGUILayout.LabelField(estados[estado_id_seleccionado].ToString());

			   if (cantidad_estados_rango > 0) {
				  string[] menu_rango = new string[cantidad_estados_padres / cantidad_estados_rango + 1];
				  string[][] menu_rango_estados = new string[cantidad_estados_padres / cantidad_estados_rango + 1][];

				  for (int i = 0; i < cantidad_estados_padres; i++) {
					 if (menu_rango_estados[i / cantidad_estados_rango] == null) {
						menu_rango_estados[i / cantidad_estados_rango] = new string[Mathf.Min(cantidad_estados_rango, cantidad_estados_padres - i)];
						menu_rango[i / cantidad_estados_rango] = "Estados " + (i) + " - " + (i + cantidad_estados_rango - 1);
					 }
					 menu_rango_estados[i / cantidad_estados_rango][i % cantidad_estados_rango] = "(" + i + ") " + estados_padres[i].ToString();
				  }

				  rango_padres_seleccionado = EditorGUILayout.Popup("Rango: ", rango_padres_seleccionado, menu_rango);
				  padre_enrango_seleccionado = EditorGUILayout.Popup("Estado: ", padre_enrango_seleccionado, menu_rango_estados[rango_padres_seleccionado]);

				  if (padre_enrango_seleccionado != -1) {
					 padre_indice_seleccionado = rango_padres_seleccionado * cantidad_estados_rango + padre_enrango_seleccionado;

					 Accion accion = estados[estado_id_seleccionado].acciones_padres[padre_indice_seleccionado];
					 EditorGUILayout.LabelField("A traves de la accion " + "(" + accion.id + ")" + " [" + accion.origen + " => " + accion.destino + "]" + " de " + accion.jugador.nombre);

					 if (GUILayout.Button("Seleccionar estado")) {
						estado_id_seleccionado = estados_padres[padre_indice_seleccionado].id;
						estado_enrango_seleccionado = estado_id_seleccionado % cantidad_estados_rango;
						rango_estados_seleccionado = estado_id_seleccionado / cantidad_estados_rango;
					 }
				  }
			   }
			   else {
				  EditorGUILayout.LabelField("La cantidad de estados por rango debe estar entre 1 y la cantidad de estados.");
			   }
			}
			else {
			   padre_enrango_seleccionado = -1;
			   EditorGUILayout.LabelField("No existen padres");
			}
			EditorGUILayout.EndVertical();
		 }

		 EditorGUILayout.Space();
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
		 Vector3 origen, destino, centro;
		 origen = destino = centro = Vector3.zero;

		 Nodo_Estado estado = null;
		 Nodo_Estado hijo = null;

		 if (generador_mdp != null && generador_mdp.arbol_estados != null && generador_mdp.arbol_estados.estados != null) {
			if (estado_id_seleccionado >= 0 && estado_id_seleccionado < generador_mdp.arbol_estados.estados.Count) {
			   estado = generador_mdp.arbol_estados.estados[estado_id_seleccionado];
			   if (hijo_indice_seleccionado >= 0 && hijo_indice_seleccionado < generador_mdp.arbol_estados.estados[estado_id_seleccionado].estados_hijos.Count) {
				  hijo = generador_mdp.arbol_estados.estados[hijo_indice_seleccionado];
			   }
			}

			if (estado != null && hijo != null) {
			   foreach (Jugador jugador in generador_jugadores.jugadores) {
				  Handles.color = Color.Lerp(Color.blue, Color.green, jugador.id * 1f / generador_jugadores.jugadores.Count);
				  foreach (Accion accion in estado.acciones_hijos_actor[jugador.id]) {
					 if (accion != estado.acciones_hijos[hijo_indice_seleccionado]) {
						origen = accion.origen.Position;
						destino = accion.destino.Position;
						centro = (origen + destino) / 2;
						Handles.DrawWireArc(centro, Vector3.Cross(destino - origen, Vector3.down).normalized, origen - centro, 180, Vector3.Distance(origen, destino) / 2);
					 }
				  }
			   }

			   Handles.color = Color.red;
			   origen = estado.acciones_hijos[hijo_indice_seleccionado].origen.Position;
			   destino = estado.acciones_hijos[hijo_indice_seleccionado].destino.Position;
			   centro = (origen + destino) / 2;
			   Handles.DrawWireArc(centro, Vector3.Cross(destino - origen, Vector3.down).normalized, origen - centro, 180, Vector3.Distance(origen, destino) / 2);

			   Handles.color = Color.red;
			   Handles.SphereCap(0, origen, Quaternion.identity, 1.5f);
			   Handles.color = Color.yellow;
			   Handles.SphereCap(0, destino, Quaternion.identity, 1.5f);
			}

			if (estado != null) {
			   if (estado.estado_actual != null && estado.estado_actual.posicion_jugadores != null) {
				  int i = 0;
				  foreach (KeyValuePair<int, Vector3> posicion in estado.estado_actual.posicion_jugadores) {
					 Handles.color = Color.Lerp(Color.blue, Color.green, i * 1f / estado.estado_actual.posicion_jugadores.Count);
					 Handles.SphereCap(0, posicion.Value, Quaternion.identity, 0.75f);
					 i++;
				  }
			   }
			}

			sceneView.Repaint();
		 }

		 Handles.BeginGUI();
		 if (generador_mdp != null) {
			if (estado != null && estado.estado_actual != null && estado.estado_actual.posicion_jugadores != null) {
			   foreach (KeyValuePair<int, Vector3> posicion in estado.estado_actual.posicion_jugadores) {
				  imprimirLabel(posicion.Value, generador_jugadores.jugadores[posicion.Key].nombre, sceneView.camera);
			   }
			}
		 }
		 Handles.EndGUI();
	  }
   }

   public void imprimirLabel(Vector3 posicion_mundo, string label, Camera camara) {
	  Vector3 posicion_pantalla = camara.WorldToScreenPoint(posicion_mundo);
	  GUI.Label(new Rect(posicion_pantalla.x, camara.pixelHeight - posicion_pantalla.y, label.Length * 10, 20), label);
   }
}