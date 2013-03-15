using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;
using Random = UnityEngine.Random;
using Habitacion = Generador_Mapa.Habitacion;

public class Generador_Objetivos : MonoBehaviour {
   // Representacion visual
   public Material fuente_material;
   public Font fuente;

   // Objetivos
   [NonSerialized]
   public List<Objetivo> objetivos;
   public List<ObjetivoMB> objetivos_mb;

   // Representacion
   public GameObject objetivos_objeto;

   // Operaciones
   public void inicializar(Material fuente_m, Font fuente_o) {
	  if (objetivos == null) {
		 objetivos = new List<Objetivo>();
		 objetivos_mb = new List<ObjetivoMB>();
	  }
	  else {
		 objetivos.Clear();
		 objetivos_mb.Clear();
	  }

	  if (objetivos_objeto == null) {
		 objetivos_objeto = new GameObject("Objetivos");
	  }
	  objetivos_objeto.transform.parent = GetComponent<Generador_Escenario>().escenario_objeto.transform;

	  while (objetivos_objeto.transform.childCount > 0) {
		 DestroyImmediate(objetivos_objeto.transform.GetChild(0).gameObject);
	  }

	  fuente_material = fuente_m;
	  fuente = fuente_o;
   }

   // Generacion
   public void generar(int cant_objetivos) {
	  Generador_Mapa generador_mapa = GetComponent<Generador_Mapa>();
	  Generador_Navegacion generador_navegacion = GetComponent<Generador_Navegacion>();

	  HashSet<int> habitaciones_usadas = new HashSet<int>();

	  for (int i = 0; i < cant_objetivos * 2; i++) {
		 int indice;
		 Habitacion habitacion;
		 do {
			indice = Random.Range(0, generador_mapa.habitaciones.Count);
			habitacion = generador_mapa.habitaciones[indice];
		 } while (habitaciones_usadas.Contains(indice));
		 habitaciones_usadas.Add(indice);

		 Vector3 posicion = generador_mapa.mapa.posicionRepresentacionAReal(Vector2.right * Random.Range(habitacion.x1, habitacion.x2) + Vector2.up * Random.Range(habitacion.y1, habitacion.y2), 0.75f);
		 do {
			posicion = generador_mapa.mapa.posicionRepresentacionAReal(Vector2.right * Random.Range(habitacion.x1, habitacion.x2) + Vector2.up * Random.Range(habitacion.y1, habitacion.y2), 0.75f);
		 } while (posicion == generador_mapa.mapa.posicionRepresentacionAReal(Vector2.right * habitacion.centrox + Vector2.up * habitacion.centroy, posicion.y));
		 Waypoint waypoint = generador_navegacion.agregarWaypoint(i, "Objetivo", new Vector3(posicion.x, 1.25f, posicion.z));

		 string nombre_objetivo = ((char)(i / 2 + 65)).ToString();
		 if (i % 2 == 1) {
			nombre_objetivo = objetivos[i - 1].nombre.ToLower();
		 }
		 GameObject zona_objetivo = new GameObject("Objetivo_" + nombre_objetivo);
		 zona_objetivo.transform.position = posicion;
		 zona_objetivo.transform.parent = objetivos_objeto.transform;
		 zona_objetivo.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

		 TextMesh texto_objetivo = zona_objetivo.AddComponent<TextMesh>();
		 zona_objetivo.AddComponent<MeshRenderer>();

		 texto_objetivo.text = nombre_objetivo;
		 texto_objetivo.fontSize = 48;
		 texto_objetivo.characterSize = 0.5f;
		 texto_objetivo.anchor = TextAnchor.MiddleCenter;
		 texto_objetivo.alignment = TextAlignment.Center;
		 texto_objetivo.font = fuente;
		 zona_objetivo.renderer.sharedMaterial = fuente_material;

		 ObjetivoMB objetivo_mb = texto_objetivo.gameObject.AddComponent<ObjetivoMB>();
		 Objetivo objetivo = new Objetivo(i, nombre_objetivo, posicion, waypoint);
		 objetivo_mb.inicializar(objetivo, 0.5f, "Jugador");
		 Objetivo.mapeo_waypoint_objetivo.Add(waypoint, objetivo_mb);
		 if (i % 2 == 1) {
			objetivo.agregarComplementario(objetivos[i - 1]);
			objetivos[i - 1].agregarComplementario(objetivo);
		 }

		 objetivos.Add(objetivo);
		 objetivos_mb.Add(objetivo_mb);
	  }
   }

   public void borrar() {
	  while (objetivos_objeto.transform.childCount > 0) {
		 DestroyImmediate(objetivos_objeto.transform.GetChild(0).gameObject);
	  }

	  DestroyImmediate(objetivos_objeto);
   }
}