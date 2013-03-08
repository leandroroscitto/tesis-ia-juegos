using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;

public class Generador_Acciones : MonoBehaviour {
   // Acciones
   [NonSerialized]
   public List<Accion> acciones;

   // Representacion
   public GameObject acciones_objeto;

   // Operaciones
   public void inicializar() {
	  if (acciones_objeto == null) {
		 acciones_objeto = new GameObject("Acciones");
	  }
	  acciones_objeto.transform.parent = GetComponent<Generador_Escenario>().escenario_objeto.transform;

	  if (acciones == null) {
		 acciones = new List<Accion>();
	  }
	  acciones.Clear();
   }

   // Generacion
   public void generar() {
	  int i = 0;
	  List<Jugador> jugadores = GetComponent<Generador_Jugadores>().jugadores;
	  foreach (Jugador jugador in jugadores) {
		 foreach (Waypoint waypoint in Navigation.Waypoints) {
			acciones.Add(new Accion(i, jugador, waypoint, waypoint));
			i++;
			foreach (Connection connection in waypoint.Connections) {
			   acciones.Add(new Accion(i, jugador, connection.From, connection.To));
			   i++;
			}
		 }
	  }
   }

   public void borrar() {
	  while (acciones_objeto.transform.childCount > 0) {
		 DestroyImmediate(acciones_objeto.transform.GetChild(0));
	  }

	  DestroyImmediate(acciones_objeto);
   }
}