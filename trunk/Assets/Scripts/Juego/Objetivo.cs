using UnityEngine;
using System;
using System.Runtime.Serialization;
using PathRuntime;
using System.Collections.Generic;

[Serializable]
public class Objetivo : Objetivo_MDP {
   public bool cumplido;
   public Vector2 posicion;
   public string nombre;
   public Objetivo complementario;
   public Waypoint waypoint_asociado;

   public Radar radar;

   public Objetivo(int i, ObjetoMB<Objetivo> objetivo_mb, string nombre_in, Vector2 p, Waypoint wayp_asociado_in)
	  : base(i) {
	  inicializar(objetivo_mb, nombre_in, p, wayp_asociado_in);
   }

   public void inicializar(ObjetoMB<Objetivo> objetivo_mb, string nombre_in, Vector2 p, Waypoint wayp_asociado_in) {
	  nombre = nombre_in;
	  posicion = p;
	  waypoint_asociado = wayp_asociado_in;

	  radar = objetivo_mb.gameObject.AddComponent<Radar>();
	  radar.DetectionRadius = 2.5f;
	  radar.DetectDisabledVehicles = true;
	  radar.LayersChecked = 1 << LayerMask.NameToLayer("Jugador");
   }

   public void agregarComplementario(Objetivo obj) {
	  complementario = obj;
   }

   public override string ToString() {
	  return "Objetivo_id: " + id + ", cumplido: " + cumplido + ", complementario_id: " + complementario.id;
   }

   // Serializacion
   public Objetivo(SerializationInfo info, StreamingContext ctxt)
	  : base(info, ctxt) {
	  cumplido = info.GetBoolean("Cumplido");
	  posicion = (Vector2)info.GetValue("Posicion", typeof(Vector2));
	  nombre = info.GetString("Nombre");
	  complementario = info.GetValue("Complementario", typeof(Objetivo)) as Objetivo;
	  waypoint_asociado = info.GetValue("Waypoint_Asociado", typeof(Waypoint)) as Waypoint;
   }

   public new void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  base.GetObjectData(info, ctxt);

	  info.AddValue("Cumplido", cumplido);
	  info.AddValue("Posicion", posicion);
	  info.AddValue("Nombre", nombre);
	  info.AddValue("Complementario", complementario);
	  info.AddValue("Waypoint_Asociado", waypoint_asociado);
   }
}