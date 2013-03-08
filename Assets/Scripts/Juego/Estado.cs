using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Estado : ISerializable {
   public int id;
   // <id_jugador,posicion_jugador>
   public Dictionary<int, Vector3> posicion_jugadores;
   // <id_objetivo>
   public HashSet<int> objetivos_cumplidos;
   // <id_objetivo>
   public HashSet<int> objetivos_no_cumplidos;

   public Estado() {
   }

   public Estado(int i) {
	  id = i;
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
   public bool mismoEstado(Estado estado) {
	  if (estado != null) {
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
	  else {
		 return false;
	  }
   }

   public override int GetHashCode() {
	  return id;
   }

   public override string ToString() {
	  if (objetivos_cumplidos != null) {
		 return "Estado_id: " + id + ", objetivos_cumplidos: " + objetivos_cumplidos.Count;
	  }
	  else {
		 return "Estado_id: " + id + ".";
	  }
   }

   // Serializacion
   public Estado(SerializationInfo info, StreamingContext ctxt) {
	  id = info.GetInt32("Id");
	  posicion_jugadores = info.GetValue("Posicion_Jugadores", typeof(Dictionary<int, Vector3>)) as Dictionary<int, Vector3>;

	  objetivos_cumplidos = info.GetValue("Objetivos_Cumplidos", typeof(HashSet<int>)) as HashSet<int>;
	  objetivos_no_cumplidos = info.GetValue("Objetivos_No_Cumplidos", typeof(HashSet<int>)) as HashSet<int>;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Id", id);
	  info.AddValue("Posicion_Jugadores", posicion_jugadores);

	  info.AddValue("Objetivos_Cumplidos", objetivos_cumplidos);
	  info.AddValue("Objetivos_No_Cumplidos", objetivos_no_cumplidos);
   }
}