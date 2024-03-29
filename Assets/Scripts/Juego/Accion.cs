﻿using System;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;

[Serializable]
public class Accion : Accion_MDP, ISerializable {
   public enum TipoAccion { MOVIMIENTO, OBJETIVO }

   public TipoAccion tipo;
   public Jugador jugador;
   public Waypoint origen;
   public Waypoint destino;
   public float distancia;

   public Accion()
	  : base(0, 0) {

   }

   public Accion(int i, TipoAccion t, Jugador j, Waypoint o, Waypoint d)
	  : base(i, j.id) {
	  tipo = t;
	  jugador = j;
	  origen = o;
	  destino = d;
	  distancia = Vector3.Distance(o.Position, d.Position);
   }

   public bool mismaAccion(Accion accion) {
	  if (accion != null) {
		 return ((accion.tipo == tipo) && (accion.origen == origen) && (accion.destino == destino));
	  }
	  else {
		 return false;
	  }
   }

   public override int GetHashCode() {
	  return id;
   }

   public override string ToString() {
	  return "ID: " + id + ", jugador: " + jugador.nombre + ", tipo: " + tipo + ", [" + origen + " => " + destino + "]";
   }

   // Serializacion
   public Accion(SerializationInfo info, StreamingContext ctxt)
	  : base(info, ctxt) {
	  tipo = (TipoAccion)info.GetInt16("Tipo");
	  jugador = info.GetValue("Jugador", typeof(Jugador)) as Jugador;

	  Vector3 origenp, destinop;
	  origenp = (Vector3)info.GetValue("Origen", typeof(Vector3));
	  destinop = (Vector3)info.GetValue("Destino", typeof(Vector3));

	  origen = destino = null;
	  foreach (Waypoint waypoint in Navigation.Waypoints) {
		 if (origen != null && destino != null) {
			break;
		 }
		 if (waypoint.Position == origenp) {
			origen = waypoint;
		 }
		 if (waypoint.Position == destinop) {
			destino = waypoint;
		 }
	  }

	  distancia = Vector3.Distance(origen.Position, destino.Position);
   }

#pragma warning disable 0114
   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  base.GetObjectData(info, ctxt);

	  info.AddValue("Tipo", (int)tipo);
	  info.AddValue("Jugador", jugador);
	  info.AddValue("Origen", origen.Position);
	  info.AddValue("Destino", destino.Position);
   }
#pragma warning restore 0114
}