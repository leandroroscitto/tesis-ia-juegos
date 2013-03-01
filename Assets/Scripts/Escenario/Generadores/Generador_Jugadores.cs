using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;
using Random = UnityEngine.Random;
using Habitacion = Generador_Mapa.Habitacion;

public class Generador_Jugadores : MonoBehaviour {
   // Jugadores
   public List<Jugador> jugadores;
   public List<JugadorMB> jugadores_mb;

   // Representacion
   public GameObject jugadores_objeto;

   // Operaciones
   public void inicializar() {
	  if (jugadores == null) {
		 jugadores = new List<Jugador>();
		 jugadores_mb = new List<JugadorMB>();
	  }
	  else {
		 jugadores.Clear();
		 jugadores_mb.Clear();
	  }

	  if (jugadores_objeto == null) {
		 jugadores_objeto = new GameObject("Jugadores");
	  }
	  while (jugadores_objeto.transform.childCount > 0) {
		 DestroyImmediate(jugadores_objeto.transform.GetChild(0).gameObject);
	  }

	  jugadores_objeto.transform.parent = GetComponent<Generador_Escenario>().escenario_objeto.transform;
   }

   // Generacion
   public void generar(int cant_jugadores, GameObject jugador_prefab, GameObject companero_prefab, Camara_3Persona camara) {
	  Generador_Mapa generador_mapa = GetComponent<Generador_Mapa>();
	  HashSet<int> habitaciones_usadas = new HashSet<int>();
	  int indice;
	  Habitacion habitacion;

	  if (cant_jugadores > 0) {
		 indice = Random.Range(0, generador_mapa.habitaciones.Count);
		 habitacion = generador_mapa.habitaciones[indice];
		 habitaciones_usadas.Add(indice);

		 Vector3 posicion = generador_mapa.mapa.posicionRepresentacionAReal(Vector2.right * Random.Range(habitacion.x1 + 1, habitacion.x2 - 1) + Vector2.up * Random.Range(habitacion.y1 + 1, habitacion.y2 - 1), 1.25f);
		 GameObject jugador_objeto = UnityEditor.PrefabUtility.InstantiatePrefab(jugador_prefab) as GameObject;
		 jugador_objeto.transform.position = posicion;
		 jugador_objeto.transform.parent = jugadores_objeto.transform;

		 jugador_objeto.GetComponent<Control_Directo>().camara_3persona = camara;

		 JugadorMB jugador_mb = jugador_objeto.AddComponent<JugadorMB>();
		 Jugador jugador = new Jugador(0, jugador_mb, jugador_objeto.name = "Jugador", '@', generador_mapa.mapa.posicionRealARepresentacion(posicion), Jugador.TControl.DIRECTO);
		 jugadores.Add(jugador);
		 jugadores_mb.Add(jugador_mb);
	  }

	  for (int i = 1; i < cant_jugadores; i++) {
		 do {
			indice = Random.Range(0, generador_mapa.habitaciones.Count);
			habitacion = generador_mapa.habitaciones[indice];
		 } while (habitaciones_usadas.Contains(indice));
		 habitaciones_usadas.Add(indice);

		 Vector3 posicion = generador_mapa.mapa.posicionRepresentacionAReal(Vector2.right * Random.Range(habitacion.x1 + 1, habitacion.x2 - 1) + Vector2.up * Random.Range(habitacion.y1 + 1, habitacion.y2 - 1), 1.25f);
		 GameObject jugador_objeto = UnityEditor.PrefabUtility.InstantiatePrefab(companero_prefab) as GameObject;
		 jugador_objeto.transform.position = posicion;
		 jugador_objeto.transform.parent = jugadores_objeto.transform;

		 JugadorMB jugador_mb = jugador_objeto.AddComponent<JugadorMB>();
		 Jugador jugador = new Jugador(i, jugador_mb, jugador_objeto.name = "Companero_" + i, '$', generador_mapa.mapa.posicionRealARepresentacion(posicion), Jugador.TControl.IA);
		 jugadores.Add(jugador);
		 jugadores_mb.Add(jugador_mb);
	  }
   }

   public void borrar() {
	  while (jugadores_objeto.transform.childCount > 0) {
		 DestroyImmediate(jugadores_objeto.transform.GetChild(0).gameObject);
	  }

	  DestroyImmediate(jugadores_objeto);
   }
}