using System.Collections.Generic;
using UnityEngine;

public class DisplayDatos : MonoBehaviour {
   [HideInInspector]
   public List<Accion> Acciones;
   [HideInInspector]
   public List<Objetivo> Objetivos;
   [HideInInspector]
   public List<Jugador> Jugadores;
   [HideInInspector]
   public Mapa Mapa;
   [HideInInspector]
   public Arbol_Estados Arbol_Estados;
   [HideInInspector]
   public ResolucionMDP Resolucion_MDP;
}