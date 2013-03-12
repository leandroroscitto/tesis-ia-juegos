using UnityEngine;
using System;
using System.Linq;
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
   // <tiempo, accion>
   public SortedDictionary<float, Accion> acciones;

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

   // Operaciones
   public Jugador() {

   }

   public Jugador(int i, string n, char r, Vector3 p, TControl c) {
	  id = i;
	  nombre = n;
	  representacion = r;
	  control = c;
	  acciones = new SortedDictionary<float, Accion>();

	  jugador_mb.jugador = this;
	  posicion = p;
   }

   // Acciones
   public Accion registrarAccion(float tiempo, Accion accion) {
	  if (acciones.Count == 0) {
		 acciones.Add(tiempo, accion);
	  }
	  else {
		 float ultimo_tiempo = acciones.Keys.Last<float>();
		 Accion ultima_accion = acciones[ultimo_tiempo];
		 Debug.Log(ultimo_tiempo);

		 if (ultima_accion.mismaAccion(accion)) {
			acciones.Remove(ultimo_tiempo);
			acciones.Add(tiempo, ultima_accion);
		 }
		 else {
			acciones.Add(tiempo, accion);
		 }
	  }

	  return accion;
   }

   public void removerAccion(float tiempo) {
	  acciones.Remove(tiempo);
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
	  acciones = info.GetValue("Acciones", typeof(SortedDictionary<float, Accion>)) as SortedDictionary<float, Accion>;

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
