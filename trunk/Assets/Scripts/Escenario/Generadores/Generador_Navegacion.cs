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

   // Operaciones
   public void inicializar() {
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
   }

   // Generacion
   public void generar() {
	  Generador_Mapa generador_mapa = GetComponent<Generador_Mapa>();
	  int i = Navigation.Waypoints.Count;

	  foreach (Conexion conexion_v in generador_mapa.conexiones_verticales) {
		 foreach (Conexion conexion_h in generador_mapa.conexiones_horizontales) {
			int x = conexion_v.x1;
			int y = conexion_h.y1;
			if (generador_mapa.mapa.tiles[x + y * Mapa.cant_x].transitable) {
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
			   if (generador_mapa.mapa.tiles[x + y * Mapa.cant_x].transitable) {
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
			   if (generador_mapa.mapa.tiles[x + y * Mapa.cant_x].transitable) {
				  if ((x >= conexion_h.x1 && x <= conexion_h.x2) && (y >= conexion_v.y1 && y <= conexion_v.y2)) {
					 generador_mapa.intersecciones.Add(new Vector2(x, y));
				  }
			   }
			}
		 }

		 Vector3 posicion = generador_mapa.mapa.posicionRepresentacionAReal(Vector2.right * (habitacion.x1 + habitacion.ancho / 2) + Vector2.up * (habitacion.y1 + habitacion.largo / 2), 1.25f);

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

   public void optimizarMallaNavegacion() {
	  Generador_Mapa generador_mapa = GetComponent<Generador_Mapa>();
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

   public bool mismasConexiones(Waypoint w1, Waypoint w2) {
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
}