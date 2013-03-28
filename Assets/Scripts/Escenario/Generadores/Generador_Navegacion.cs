using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;
using Conexion = Generador_Mapa.Conexion;
using Habitacion = Generador_Mapa.Habitacion;

public class Generador_Navegacion : MonoBehaviour {
   // Navegacion
   public Navigation navigation;

   // Representacion
   public GameObject navigation_objeto;

   // Listas
   public List<Waypoint> habitaciones_waypoints;
   public List<Waypoint> objetivo_waypoints;
   public List<Waypoint> conexiones_waypoints;

   // Optimizacion
   public float maximo_angulo_optimizacion = 45;

   // Operaciones
   public void inicializar(float max_ang_opt) {
	  if (navigation_objeto == null) {
		 navigation_objeto = new GameObject("Navegacion");
		 navigation = navigation_objeto.AddComponent<Navigation>();
	  }
	  navigation_objeto.transform.parent = GetComponent<Generador_Escenario>().escenario_objeto.transform;
	  navigation_objeto.transform.localPosition = Vector3.zero;

	  while (navigation_objeto.transform.childCount > 0) {
		 DestroyImmediate(navigation_objeto.transform.GetChild(0).gameObject);
	  }

	  if (habitaciones_waypoints == null) {
		 habitaciones_waypoints = new List<Waypoint>();
	  }
	  habitaciones_waypoints.Clear();
	  if (objetivo_waypoints == null) {
		 objetivo_waypoints = new List<Waypoint>();
	  }
	  objetivo_waypoints.Clear();
	  if (conexiones_waypoints == null) {
		 conexiones_waypoints = new List<Waypoint>();
	  }
	  conexiones_waypoints.Clear();

	  maximo_angulo_optimizacion = max_ang_opt;
   }

   // Generacion
   public void generar() {
	  Generador_Mapa generador_mapa = GetComponent<Generador_Mapa>();
	  int i = Navigation.Waypoints.Count;

	  foreach (Conexion conexion_v in generador_mapa.conexiones_verticales) {
		 foreach (Conexion conexion_h in generador_mapa.conexiones_horizontales) {
			int x = conexion_v.x1;
			int y = conexion_h.y1;
			if (generador_mapa.mapa.tiles[x + y * Mapa.Mapa_Instancia.cant_x].transitable) {
			   if ((x >= conexion_h.x1 && x <= conexion_h.x2) && (y >= conexion_v.y1 && y <= conexion_v.y2)) {
				  generador_mapa.intersecciones.Add(new Vector2(x, y));
			   }
			}
		 }
	  }

	  foreach (Habitacion habitacion in generador_mapa.habitaciones) {
		 List<Conexion> habitacion_horizontal = new List<Conexion>();
		 List<Conexion> habitacion_vertical = new List<Conexion>();
		 habitacion_horizontal.Add(new Conexion(habitacion.x1 - 1, habitacion.y1, habitacion.x2 + 1, habitacion.y1));
		 habitacion_horizontal.Add(new Conexion(habitacion.x1 - 1, habitacion.y2, habitacion.x2 + 1, habitacion.y2));
		 habitacion_vertical.Add(new Conexion(habitacion.x1, habitacion.y1 - 1, habitacion.x1, habitacion.y2 + 1));
		 habitacion_vertical.Add(new Conexion(habitacion.x2, habitacion.y1 - 1, habitacion.x2, habitacion.y2 + 1));

		 foreach (Conexion conexion_v in generador_mapa.conexiones_verticales) {
			foreach (Conexion conexion_h in habitacion_horizontal) {
			   int x = conexion_v.x1;
			   int y = conexion_h.y1;
			   if (generador_mapa.mapa.tiles[x + y * Mapa.Mapa_Instancia.cant_x].transitable) {
				  if ((x >= conexion_h.x1 && x <= conexion_h.x2) && (y >= conexion_v.y1 && y <= conexion_v.y2)) {
					 generador_mapa.intersecciones.Add(new Vector2(x, y));
				  }
			   }
			}
		 }

		 foreach (Conexion conexion_v in habitacion_vertical) {
			foreach (Conexion conexion_h in generador_mapa.conexiones_horizontales) {
			   int x = conexion_v.x1;
			   int y = conexion_h.y1;
			   if (generador_mapa.mapa.tiles[x + y * Mapa.Mapa_Instancia.cant_x].transitable) {
				  if ((x >= conexion_h.x1 && x <= conexion_h.x2) && (y >= conexion_v.y1 && y <= conexion_v.y2)) {
					 generador_mapa.intersecciones.Add(new Vector2(x, y));
				  }
			   }
			}
		 }

		 Vector3 posicion = generador_mapa.mapa.posicionRepresentacionAReal(Vector2.right * (habitacion.centrox) + Vector2.up * (habitacion.centroy), 1.25f);

		 agregarWaypoint(i, "Habitacion", posicion);
		 i++;
	  }

	  foreach (Vector2 interseccion in generador_mapa.intersecciones) {
		 Vector3 posicion = generador_mapa.mapa.posicionRepresentacionAReal(interseccion, 1.25f);

		 Waypoint waypoint_mas_cercano = Navigation.GetNearestNode(posicion);
		 if (waypoint_mas_cercano == null || Vector3.Distance(posicion, waypoint_mas_cercano.Position) > (generador_mapa.mapa.tamano_tile.x + generador_mapa.mapa.tamano_tile.z) / 3) {
			agregarWaypoint(i, "Conexion", posicion);
			i++;
		 }
	  }

	  Navigation.AutoScale(generador_mapa.mascara, 0.25f, Mathf.Max(generador_mapa.mapa.tamano_tile.x, generador_mapa.mapa.tamano_tile.z) * 0.8f, 0.1f);
	  Navigation.AutoConnect(generador_mapa.mascara, 0.25f, Mathf.Max(generador_mapa.mapa.tamano_tile.x, generador_mapa.mapa.tamano_tile.z) * 0.8f, 0.1f);

	  //Debug.Log(Navigation.Waypoints.Count);
	  optimizarMallaNavegacion();
	  //Debug.Log(Navigation.Waypoints.Count);
   }

   public void borrar() {
	  while (navigation_objeto.transform.childCount > 0) {
		 DestroyImmediate(navigation_objeto.transform.GetChild(0).gameObject);
	  }
	  DestroyImmediate(navigation_objeto);
	  caminos = null;
   }

   // Utilidades
   public Waypoint agregarWaypoint(int id, string tag, Vector3 posicion) {
	  Waypoint waypoint;
	  waypoint = new GameObject("Waypoint_" + id + "_" + tag).AddComponent<Waypoint>();
	  waypoint.Tag = tag;
	  waypoint.Position = posicion;
	  waypoint.transform.parent = navigation_objeto.transform;

	  switch (tag) {
		 case "Habitacion":
			habitaciones_waypoints.Add(waypoint);
			break;
		 case "Objetivo":
			objetivo_waypoints.Add(waypoint);
			break;
		 case "Conexion":
			conexiones_waypoints.Add(waypoint);
			break;
	  }

	  return waypoint;
   }

   public void removerWaypoint(Waypoint waypoint) {
	  switch (waypoint.Tag) {
		 case "Habitacion":
			habitaciones_waypoints.Remove(waypoint);
			break;
		 case "Objetivo":
			objetivo_waypoints.Remove(waypoint);
			break;
		 case "Conexion":
			conexiones_waypoints.Remove(waypoint);
			break;
	  }
	  DestroyImmediate(waypoint.gameObject);
   }

   private void optimizarNodos() {
	  Generador_Mapa generador_mapa = GetComponent<Generador_Mapa>();

	  // Nodos en exceso
	  List<Waypoint> a_destruir = new List<Waypoint>();
	  foreach (Waypoint waypoint in Navigation.Waypoints) {
		 if (!a_destruir.Contains(waypoint)) {
			int i = 0;
			while (i < waypoint.Connections.Count) {
			   Connection connection = waypoint.Connections[i];
			   Vector3 direccion = connection.To.Position - connection.From.Position;

			   if (Physics.Raycast(connection.From.Position, direccion, direccion.magnitude, generador_mapa.mascara)) {
				  waypoint.RemoveConnection(connection);
			   }
			   else if (!a_destruir.Contains(connection.To) && mismasConexiones(waypoint, connection.To)) {
				  if (connection.To.Tag == "Conexion") {
					 a_destruir.Add(connection.To);
				  }
			   }
			   i++;
			}
		 }
	  }

	  while (a_destruir.Count > 0) {
		 Waypoint waypoint = a_destruir[0];
		 a_destruir.RemoveAt(0);
		 removerWaypoint(waypoint);
	  }
   }

   private void optimizarConexiones() {
	  List<KeyValuePair<Waypoint, Waypoint>> conexiones_a_eliminar = new List<KeyValuePair<Waypoint, Waypoint>>();

	  // Conexiones en exceso
	  foreach (Waypoint wi in Navigation.Waypoints) {
		 foreach (Waypoint wj in Navigation.Waypoints) {
			foreach (Waypoint wk in Navigation.Waypoints) {
			   float dik = Vector3.Distance(wi.Position, wk.Position);
			   float dij = Vector3.Distance(wi.Position, wj.Position);
			   float djk = Vector3.Distance(wj.Position, wk.Position);
			   bool rango_distancia = Mathf.Abs(dik - (dij + djk)) <= 0.65f;

			   //float angulo = Vector3.Angle(wi.Position - wj.Position, wk.Position - wj.Position);
			   //bool rango_angulo = (180 - angulo) <= maximo_angulo_optimizacion;

			   bool conexion_transitiva = (wi.ConnectsTo(wj) && wj.ConnectsTo(wk) && wi.ConnectsTo(wk));

			   if (conexion_transitiva && rango_distancia) {
				  //if (conexion_transitiva && rango_angulo) {
				  conexiones_a_eliminar.Add(new KeyValuePair<Waypoint, Waypoint>(wi, wk));
			   }
			}
		 }
	  }

	  foreach (KeyValuePair<Waypoint, Waypoint> conexion in conexiones_a_eliminar) {
		 if (conexion.Key.ConnectsTo(conexion.Value)) {
			conexion.Key.RemoveConnection(conexion.Value);
		 }
		 if (conexion.Key.ConnectsTo(conexion.Key)) {
			conexion.Value.RemoveConnection(conexion.Key);
		 }
	  }
   }

   private void optimizarMallaNavegacion() {
	  optimizarNodos();
	  optimizarConexiones();
   }

   private bool mismasConexiones(Waypoint w1, Waypoint w2) {
	  HashSet<Waypoint> conexiones_w1 = new HashSet<Waypoint>();
	  HashSet<Waypoint> conexiones_w2 = new HashSet<Waypoint>();

	  foreach (Connection conexion in w1.Connections) {
		 conexiones_w1.Add(conexion.To);
	  }
	  foreach (Connection conexion in w2.Connections) {
		 conexiones_w2.Add(conexion.To);
	  }

	  if (conexiones_w1.Contains(w2)) {
		 conexiones_w1.Remove(w2);
	  }
	  else {
		 return false;
	  }

	  if (conexiones_w2.Contains(w1)) {
		 conexiones_w2.Remove(w1);
	  }
	  else {
		 return false;
	  }

	  return (conexiones_w1.IsSupersetOf(conexiones_w2));
   }

   // Pathfinding
   public static Dictionary<Vector3, Dictionary<Vector3, KeyValuePair<List<Vector3>, float>>> caminos;

   public static Waypoint waypointMasCercano(Vector3 posicion, bool visible, out float distancia_mas_cercana) {
	  float maxima_distancia = Mathf.Max(Mapa.Mapa_Instancia.ancho, Mapa.Mapa_Instancia.largo);

	  distancia_mas_cercana = float.MaxValue;
	  Waypoint waypoint_mas_cercano = null;
	  foreach (Waypoint waypoint in Navigation.Waypoints) {
		 float distancia = Vector3.Distance(posicion, waypoint.Position);
		 if (visible && Physics.Raycast(posicion, waypoint.Position, maxima_distancia) && distancia <= distancia_mas_cercana) {
			distancia_mas_cercana = distancia;
			waypoint_mas_cercano = waypoint;
		 }
		 else if (distancia <= distancia_mas_cercana) {
			distancia_mas_cercana = distancia;
			waypoint_mas_cercano = waypoint;
		 }
	  }

	  return waypoint_mas_cercano;
   }

   public static int compararDistancias(KeyValuePair<int, float> e1, KeyValuePair<int, float> e2) {
	  if (e1.Value == e2.Value) {
		 return 0;
	  }
	  else if (e1.Value > e2.Value) {
		 return 1;
	  }
	  else {
		 return -1;
	  }
   }

   public static int getClave(KeyValuePair<int, float> e1) {
	  return e1.Key;
   }

   public static void calcularMinimaDistanciaWaypoints() {
	  caminos = new Dictionary<Vector3, Dictionary<Vector3, KeyValuePair<List<Vector3>, float>>>();

	  Dictionary<Waypoint, int> indices = new Dictionary<Waypoint, int>(Navigation.Waypoints.Count);
	  for (int i = 0; i < Navigation.Waypoints.Count; i++) {
		 indices.Add(Navigation.Waypoints[i], i);
	  }

	  for (int i = 0; i < Navigation.Waypoints.Count; i++) {
		 float[] distancia = new float[Navigation.Waypoints.Count];
		 int[] anterior = new int[Navigation.Waypoints.Count];
		 for (int j = 0; j < Navigation.Waypoints.Count; j++) {
			distancia[j] = float.PositiveInfinity;
			anterior[j] = -1;
		 }

		 distancia[i] = 0;
		 HeapBinario<KeyValuePair<int, float>> cola = new HeapBinario<KeyValuePair<int, float>>(Navigation.Waypoints.Count, compararDistancias, getClave);
		 for (int j = 0; j < Navigation.Waypoints.Count; j++) {
			cola.insertar(new KeyValuePair<int, float>(j, distancia[j]));
		 }

		 while (cola.tamano > 0) {
			KeyValuePair<int, float> u = cola.suprimirMinimo();
			if (float.IsPositiveInfinity(u.Value)) {
			   break;
			}

			foreach (Connection conexion in Navigation.Waypoints[u.Key].Connections) {
			   float alt = u.Value + Vector3.Distance(conexion.From.Position, conexion.To.Position);
			   int v_indice = indices[conexion.To];
			   if (alt < distancia[v_indice]) {
				  distancia[v_indice] = alt;
				  anterior[v_indice] = indices[conexion.From];
				  int indice = cola.getIndice(v_indice);
				  KeyValuePair<int, float> v = new KeyValuePair<int, float>(v_indice, alt);
				  cola.modificar(indice, v);
			   }
			}
		 }

		 Dictionary<Vector3, KeyValuePair<List<Vector3>, float>> distancias = new Dictionary<Vector3, KeyValuePair<List<Vector3>, float>>(Navigation.Waypoints.Count);
		 for (int j = 0; j < Navigation.Waypoints.Count; j++) {
			List<Vector3> lista = new List<Vector3>();
			int origen = i;
			int waypoint = j;
			while (waypoint != origen) {
			   lista.Insert(0, Navigation.Waypoints[waypoint].Position);
			   waypoint = anterior[waypoint];
			}
			lista.Insert(0, Navigation.Waypoints[origen].Position);
			distancias.Add(Navigation.Waypoints[j].Position, new KeyValuePair<List<Vector3>, float>(lista, distancia[j]));
		 }
		 caminos.Add(Navigation.Waypoints[i].Position, distancias);
	  }
   }

   public static float getMinimaDistancia(Vector3 p1, Vector3 p2) {
	  if (caminos == null) {
		 calcularMinimaDistanciaWaypoints();
	  }

	  Waypoint w1, w2;
	  float d1, d2;

	  w1 = waypointMasCercano(p1, true, out d1);
	  w2 = waypointMasCercano(p2, true, out d2);

	  return (d1 + caminos[w1.Position][w2.Position].Value + d2);
   }

   public static List<Vector3> getMinimoCamino(Vector3 p1, Vector3 p2) {
	  if (caminos == null) {
		 calcularMinimaDistanciaWaypoints();
	  }

	  Waypoint w1, w2;
	  float d1, d2;

	  w1 = waypointMasCercano(p1, true, out d1);
	  w2 = waypointMasCercano(p2, true, out d2);

	  return caminos[w1.Position][w2.Position].Key;
   }
}