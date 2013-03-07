using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DisplayDatosEditor : EditorWindow {
   private static DisplayDatosEditor ventana;

   private DisplayDatos display;

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
	  ventana = EditorWindow.GetWindow<DisplayDatosEditor>("A. de Estados");
   }

   void Update() {

   }

   void OnGUI() {
	  if (GUILayout.Button("Actualizar")) {
		 Debug.Log("Actualizando");
	  }

	  display = EditorGUILayout.ObjectField("Display de datos:", display, typeof(DisplayDatos), true) as DisplayDatos;

	  if (display != null) {
		 Arbol_Estados arbol_estados = display.Arbol_Estados;

		 int cantidad_estados = arbol_estados.estados.Count;
		 List<Nodo_Estado> estados = arbol_estados.estados;

		 EditorGUILayout.Space();
		 mostrar_estados = EditorGUILayout.Foldout(mostrar_estados, "Seleccion de estado");
		 if (mostrar_estados) {
			EditorGUILayout.BeginVertical();

			cantidad_estados_rango = EditorGUILayout.IntField("Cantidad por rango: ", cantidad_estados_rango);

			if (cantidad_estados > 0) {
			   if (cantidad_estados_rango > 0) {
				  string[] menu_rango = new string[cantidad_estados / cantidad_estados_rango + 1];
				  string[][] menu_rango_estados = new string[cantidad_estados / cantidad_estados_rango + 1][];

				  for (int i = 0; i < cantidad_estados; i++) {
					 if (menu_rango_estados[i / cantidad_estados_rango] == null) {
						menu_rango_estados[i / cantidad_estados_rango] = new string[Mathf.Min(cantidad_estados_rango, cantidad_estados - i)];
						menu_rango[i / cantidad_estados_rango] = "Estados " + (i) + " - " + (i + cantidad_estados_rango - 1);
					 }
					 menu_rango_estados[i / cantidad_estados_rango][i % cantidad_estados_rango] = estados[i].ToString();
				  }

				  rango_estados_seleccionado = EditorGUILayout.Popup("Rango: ", rango_estados_seleccionado, menu_rango);
				  estado_enrango_seleccionado = EditorGUILayout.Popup("Estado: ", estado_enrango_seleccionado, menu_rango_estados[rango_estados_seleccionado]);
				  estado_id_seleccionado = estados[rango_estados_seleccionado * cantidad_estados_rango + estado_enrango_seleccionado].id;
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

		 EditorGUILayout.Space();
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
					 menu_rango_estados[i / cantidad_estados_rango][i % cantidad_estados_rango] = estados_hijos[i].ToString();
				  }

				  rango_hijos_seleccionado = EditorGUILayout.Popup("Rango: ", rango_hijos_seleccionado, menu_rango);
				  hijo_enrango_seleccionado = EditorGUILayout.Popup("Estado: ", hijo_enrango_seleccionado, menu_rango_estados[rango_hijos_seleccionado]);
				  if (hijo_enrango_seleccionado != -1) {
					 hijo_indice_seleccionado = rango_hijos_seleccionado * cantidad_estados_rango + hijo_enrango_seleccionado;
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

		 EditorGUILayout.Space();
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
					 menu_rango_estados[i / cantidad_estados_rango][i % cantidad_estados_rango] = estados_padres[i].ToString();
				  }

				  rango_padres_seleccionado = EditorGUILayout.Popup("Rango: ", rango_padres_seleccionado, menu_rango);
				  padre_enrango_seleccionado = EditorGUILayout.Popup("Estado: ", padre_enrango_seleccionado, menu_rango_estados[rango_padres_seleccionado]);

				  if (padre_enrango_seleccionado != -1) {
					 padre_indice_seleccionado = rango_padres_seleccionado * cantidad_estados_rango + padre_enrango_seleccionado;
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

		 if (estado_id_seleccionado >= 0 && estado_id_seleccionado < arbol_estados.estados.Count && hijo_indice_seleccionado >= 0 && hijo_indice_seleccionado < arbol_estados.estados[estado_id_seleccionado].estados_hijos.Count) {
			Nodo_Estado estado = arbol_estados.estados[estado_id_seleccionado];
			Nodo_Estado hijo = arbol_estados.estados[hijo_indice_seleccionado];
			EditorGUILayout.LabelField(estado.acciones_hijos[hijo_indice_seleccionado].origen.name + " => " + estado.acciones_hijos[hijo_indice_seleccionado].destino.name);

			Handles.color = Color.red;
			Vector3 origen = estado.acciones_hijos[hijo_indice_seleccionado].origen.Position;
			Vector3 destino = estado.acciones_hijos[hijo_indice_seleccionado].destino.Position;
			Handles.ArrowCap(0, origen, Quaternion.LookRotation(destino - origen), Vector3.Distance(origen, destino));
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
	  if (display != null) {
		 if (estado_id_seleccionado >= 0 && estado_id_seleccionado < display.Arbol_Estados.estados.Count && hijo_indice_seleccionado >= 0 && hijo_indice_seleccionado < display.Arbol_Estados.estados[estado_id_seleccionado].estados_hijos.Count) {
			Nodo_Estado estado = display.Arbol_Estados.estados[estado_id_seleccionado];
			Nodo_Estado hijo = display.Arbol_Estados.estados[hijo_indice_seleccionado];
			EditorGUILayout.LabelField(estado.acciones_hijos[hijo_indice_seleccionado].origen.name + " => " + estado.acciones_hijos[hijo_indice_seleccionado].destino.name);

			Handles.color = Color.red;
			Vector3 origen = estado.acciones_hijos[hijo_indice_seleccionado].origen.Position;
			Vector3 destino = estado.acciones_hijos[hijo_indice_seleccionado].destino.Position;
			Vector3 centro = (origen + destino) / 2;
			Handles.DrawWireArc(centro, Vector3.Cross(destino - origen, Vector3.down).normalized, origen - centro, 180, Vector3.Distance(origen, destino) / 2);

			Handles.color = Color.blue;
			Handles.SphereCap(0, origen, Quaternion.identity, 1.5f);
			Handles.color = Color.green;
			Handles.SphereCap(0, destino, Quaternion.identity, 1.5f);

			Handles.color = Color.Lerp(Color.red, Color.yellow, 0.75f);
			foreach (Accion accion in estado.acciones_hijos) {
			   if (accion != estado.acciones_hijos[hijo_indice_seleccionado]) {
				  origen = accion.origen.Position;
				  destino = accion.destino.Position;
				  centro = (origen + destino) / 2;
				  Handles.DrawWireArc(centro, Vector3.Cross(destino - origen, Vector3.down).normalized, origen - centro, 180, Vector3.Distance(origen, destino) / 2);

				  Handles.color = Color.Lerp(Color.blue, Color.yellow, 0.75f);
				  Handles.SphereCap(0, origen, Quaternion.identity, 1);
				  Handles.color = Color.Lerp(Color.green, Color.yellow, 0.75f);
				  Handles.SphereCap(0, destino, Quaternion.identity, 1);
			   }
			}
		 }
	  }

	  Handles.BeginGUI();

	  Handles.EndGUI();
   }
}