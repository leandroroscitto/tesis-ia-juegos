using System.Collections.Generic;
using UnityEngine;

public class DisplayDatos : MonoBehaviour {
   public Arbol_Estados Arbol_Estados;
   public MDP<Estado, Accion, Objetivo, ResolucionMDP.TransicionJuego, ResolucionMDP.RecompensaJuego> MDP;
   public List<Accion> Acciones;
   public List<Objetivo> Objetivos;
   public List<Jugador> Jugadores;
   public Mapa Mapa;
}