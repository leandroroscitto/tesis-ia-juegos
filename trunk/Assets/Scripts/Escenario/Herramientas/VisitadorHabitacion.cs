using UnityEngine;
using System.Collections.Generic;

public class VisitadorHabitacion : BSPTree.Visitador {
   public Dictionary<BSPTree, Mapa.Habitacion> habitaciones;
   public List<BSPTree> nodos_habitaciones;
   private Mapa mapa;
   private Generador_Mapa generador;

   public VisitadorHabitacion(Mapa map, Generador_Mapa gen_map) {
	  mapa = map;
	  generador = gen_map;
	  habitaciones = new Dictionary<BSPTree, Mapa.Habitacion>();
	  nodos_habitaciones = new List<BSPTree>();
   }

   public override bool visitarNodo(BSPTree nodo) {
	  if (nodo.esHoja()) {
		 int x, y, w, h;
		 w = Random.Range(generador.parametros.min_hab_tam, nodo.ancho - 1);
		 h = Random.Range(generador.parametros.min_hab_tam, nodo.largo - 1);
		 x = Random.Range(nodo.x, nodo.x + nodo.ancho - w - 1);
		 y = Random.Range(nodo.y, nodo.y + nodo.largo - h - 1);

		 nodo.x = x + 1;
		 nodo.y = y + 1;
		 nodo.ancho = w - 1;
		 nodo.largo = h - 1;

		 Mapa.Habitacion habitacion = (Mapa.Habitacion)mapa.crearHabitacion(x + 1, y + 1, x + w - 1, y + h - 1, generador.tile_piso);
		 habitaciones.Add(nodo, habitacion);
		 nodos_habitaciones.Add(nodo);
	  }
	  return true;
   }
}