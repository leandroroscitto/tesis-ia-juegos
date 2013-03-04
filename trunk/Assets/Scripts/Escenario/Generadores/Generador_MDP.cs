using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;

public class Generador_MDP : MonoBehaviour {
   // Representacion
   public GameObject mdp_objeto;

   // Resolucion
   [NonSerialized]
   public ResolucionMDP resolucion_mdp;
   [NonSerialized]
   public Arbol_Estados arbol_estados;

   // Operaciones
   public void inicializar(Mapa mapa, List<Jugador> jugadores, List<Accion> acciones, List<Objetivo> objetivos) {
	  if (mdp_objeto == null) {
		 mdp_objeto = new GameObject("MDP");
	  }
	  mdp_objeto.transform.parent = GetComponent<Generador_Escenario>().escenario_objeto.transform;

	  arbol_estados = new Arbol_Estados(mapa, jugadores, acciones, objetivos);
	  resolucion_mdp = new ResolucionMDP(arbol_estados);
   }

   // Generacion
   public void generar() {
	  arbol_estados.prepararEstados();
	  //resolucion_mdp.resolverMDP();
   }

   public void borrar() {
	  while (mdp_objeto.transform.childCount > 0) {
		 DestroyImmediate(mdp_objeto.transform.GetChild(0));
	  }

	  DestroyImmediate(mdp_objeto);
   }
}