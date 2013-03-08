using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;

[Serializable]
public class Comparador_Arreglo_Vector3 : IEqualityComparer<Vector3[]> {

   public bool Equals(Vector3[] x, Vector3[] y) {
	  if (x.Length == y.Length) {
		 for (int i = 0; i < x.Length; i++) {
			if (!x[i].Equals(y[i]))
			   return false;
		 }
		 return true;
	  }
	  else
		 return false;
   }

   public int GetHashCode(Vector3[] obj) {
	  int dimensiones = Mapa.Mapa_Instancia.cant_x * Mapa.Mapa_Instancia.cant_y;
	  int valor = 0;
	  for (int i = 0; i < obj.Length; i++) {
		 valor += obj[i].GetHashCode() * (int)Math.Pow(Math.Floor(Math.Log10(dimensiones)), i);
	  }
	  return valor;
   }
}

[Serializable]
public class Arbol_Estados : ISerializable {
   public Mapa mapa_base;

   public List<Objetivo> objetivos;
   public List<Jugador> jugadores;
   public List<Accion> acciones;

   public Nodo_Estado nodo_estado_inicial;

   public List<Nodo_Estado> estados;

   public Arbol_Estados() {
   }

   public Arbol_Estados(Mapa eb, List<Jugador> jugs, List<Accion> accs, List<Objetivo> objs) {
	  mapa_base = eb;
	  acciones = accs;
	  objetivos = objs;
	  jugadores = jugs;
   }

   // Serializacion
   public Arbol_Estados(SerializationInfo info, StreamingContext ctxt) {
		 mapa_base = info.GetValue("Escenario_Base", typeof(Mapa)) as Mapa;
		 objetivos = info.GetValue("Objectivos", typeof(List<Objetivo>)) as List<Objetivo>;
		 jugadores = info.GetValue("Jugadores", typeof(List<Jugador>)) as List<Jugador>;
		 acciones = info.GetValue("Acciones", typeof(List<Accion>)) as List<Accion>;
		 estados = info.GetValue("Estados", typeof(List<Nodo_Estado>)) as List<Nodo_Estado>;
		 nodo_estado_inicial = info.GetValue("Nodo_Estado_Inicial", typeof(Nodo_Estado)) as Nodo_Estado;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Escenario_Base", mapa_base);
	  info.AddValue("Objectivos", objetivos);
	  info.AddValue("Jugadores", jugadores);
	  info.AddValue("Acciones", acciones);
	  info.AddValue("Estados", estados);
	  info.AddValue("Nodo_Estado_Inicial", nodo_estado_inicial);
   }
}