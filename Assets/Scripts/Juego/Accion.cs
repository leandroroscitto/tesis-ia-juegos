using System;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;

[Serializable]
public class Accion : Accion_MDP, ISerializable {
   public Jugador jugador;
   public Waypoint origen;
   public Waypoint destino;
   public float distancia;

   public Accion(int i, Jugador j, Waypoint o, Waypoint d)
	  : base(i, j.id) {
	  jugador = j;
	  origen = o;
	  destino = d;
	  distancia = Vector3.Distance(o.Position, d.Position);
   }

   public override bool Equals(object obj) {
	  Accion accion = (Accion)obj;
	  return ((accion.jugador.id == jugador.id) && (accion.origen == origen) && (accion.destino == destino));
   }

   public override int GetHashCode() {
	  return id;
   }

   public override string ToString() {
	  return "Accion_id: " + id + ", jugador: " + jugador.nombre + ", origen: " + origen + ", direccion: " + destino;
   }

   // Serializacion
   public Accion(SerializationInfo info, StreamingContext ctxt)
	  : base(info, ctxt) {
	  jugador = info.GetValue("Jugador", typeof(Jugador)) as Jugador;
	  origen = info.GetValue("Origen", typeof(Waypoint)) as Waypoint;
	  destino = info.GetValue("Destino", typeof(Waypoint)) as Waypoint;
	  distancia = Vector3.Distance(origen.Position, destino.Position);
   }

   public new void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  base.GetObjectData(info, ctxt);

	  info.AddValue("Jugador", jugador);
	  info.AddValue("Origen", origen);
	  info.AddValue("Destino", destino);
   }
}