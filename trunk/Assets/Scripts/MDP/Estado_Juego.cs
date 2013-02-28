using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Estado_Juego {
   public int id;
   public Mapa escenario_base;
   // <id_jugador,posicion_jugador>
   public Dictionary<int, Vector3> posicion_jugadores;
   // <id_objetivo>
   public HashSet<int> objetivos_cumplidos;
   // <id_objetivo>
   public HashSet<int> objetivos_no_cumplidos;

   public Estado_Juego(int i, Mapa eb) {
	  id = i;
	  escenario_base = eb;
	  posicion_jugadores = new Dictionary<int, Vector3>();
	  objetivos_cumplidos = new HashSet<int>();
	  objetivos_no_cumplidos = new HashSet<int>();
   }

   public bool IntentarAccion(Jugador jugador, Accion accion, out Vector3 nueva_posicion) {
	  Vector3 direccion = accion.destino.Position - posicion_jugadores[jugador.id];
	  if (!Physics.Raycast(posicion_jugadores[jugador.id], direccion, direccion.magnitude, 1 << LayerMask.NameToLayer("NoPasable"))) {
		 nueva_posicion = accion.destino.Position;
		 return true;
	  }
	  else {
		 direccion = accion.origen.Position - posicion_jugadores[jugador.id];
		 if (!Physics.Raycast(posicion_jugadores[jugador.id], direccion, direccion.magnitude, 1 << LayerMask.NameToLayer("NoPasable"))) {
			nueva_posicion = accion.origen.Position;
			return true;
		 }
		 else {
			nueva_posicion = Vector3.zero;
			return false;
		 }
	  }
   }

   // Supone que los estados son del mismo juego (mismos jugadores).
   public override bool Equals(object obj) {
	  Estado_Juego estado = (Estado_Juego)obj;

	  // Verifica que la posicion de todos los jugadores sea la misma.
	  foreach (int id_jugador in posicion_jugadores.Keys) {
		 if (!estado.posicion_jugadores[id_jugador].Equals(posicion_jugadores[id_jugador]))
			return false;
	  }

	  // Verifica que los objetivos cumplidos sean los mismos.
	  foreach (int id_objetivo in objetivos_cumplidos) {
		 if (!estado.objetivos_cumplidos.Contains(id_objetivo))
			return false;
	  }

	  // Verifica que los objetivos no cumplidos sean los mismos.
	  foreach (int id_objetivo in objetivos_no_cumplidos) {
		 if (!estado.objetivos_no_cumplidos.Contains(id_objetivo))
			return false;
	  }

	  return true;
   }

   public override int GetHashCode() {
	  return id;
   }

   public override string ToString() {
	  return "Estado_id: " + id + ", objetivos_cumplidos: " + objetivos_cumplidos.Count;
   }
}