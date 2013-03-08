using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

[Serializable]
public class Jugador : ISerializable {
   public enum TControl {
	  DIRECTO, IA
   }

   public int id;
   public string nombre;
   public char representacion;
   //public Vector2 posicion;
   public TControl control;
   // <turno, accion>
   public Dictionary<int, Accion> acciones;

   private JugadorMB _jugadormb;
   public JugadorMB jugador_mb {
	  get {
		 if (_jugadormb == null) {
			GameObject jugador_objeto = GameObject.Find(nombre);
			if (jugador_objeto != null) {
			   _jugadormb = jugador_objeto.GetComponent<JugadorMB>();
			}
		 }

		 return _jugadormb;
	  }

	  set {
		 _jugadormb = value;
	  }
   }

   public Vector3 posicion {
	  get {
		 return jugador_mb.transform.position;
	  }
	  set {
		 jugador_mb.transform.position = value;
	  }
   }

   public Jugador() {

   }

   public Jugador(int i, string n, char r, Vector3 p, TControl c) {
	  id = i;
	  nombre = n;
	  representacion = r;
	  control = c;
	  acciones = new Dictionary<int, Accion>();

	  jugador_mb.jugador = this;
	  posicion = p;
   }

   public Accion RegistrarAccion(int turno, Accion accion) {
	  acciones.Add(turno, accion);
	  return accion;
   }

   public override string ToString() {
	  return "Jugado_id: " + id + ", nombre: " + nombre + ", posicion: " + posicion;
   }

   // Serializacion
   public Jugador(SerializationInfo info, StreamingContext ctxt) {
	  id = info.GetInt16("Id");
	  nombre = info.GetString("Nombre");
	  representacion = info.GetChar("Representacion");
	  posicion = (Vector3)info.GetValue("Posicion", typeof(Vector3));
	  control = (TControl)info.GetInt16("Control");
	  acciones = info.GetValue("Acciones", typeof(Dictionary<int, Accion>)) as Dictionary<int, Accion>;

	  jugador_mb.jugador = this;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Id", id);
	  info.AddValue("Nombre", nombre);
	  info.AddValue("Representacion", representacion);
	  info.AddValue("Posicion", posicion);
	  info.AddValue("Control", control);
	  info.AddValue("Acciones", acciones);
   }
}
