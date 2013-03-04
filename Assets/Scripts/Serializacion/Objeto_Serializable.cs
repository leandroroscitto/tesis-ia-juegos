using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Objeto_Serializable : ISerializable {
   private ResolucionMDP _resolucionmdp;
   public Nodo_Estado nodo_estado;
   public Estado estado;
   public Mapa mapa;

   public ResolucionMDP Resolucion_MDP {
	  get {
		 return _resolucionmdp;
	  }
	  set {
		 _resolucionmdp = value;
	  }
   }
   public Arbol_Estados Arbol_Estados {
	  get {
		 return _resolucionmdp.arbol_estados;
	  }
   }
   public Mapa Mapa {
	  get {
		 return Arbol_Estados.mapa_base;
	  }
   }
   public MDP<Nodo_Estado, Accion, Objetivo, ResolucionMDP.TransicionJuego, ResolucionMDP.RecompensaJuego> MDP {
	  get {
		 return _resolucionmdp.mdp;
	  }
   }
   public List<Accion> Acciones {
	  get {
		 return _resolucionmdp.arbol_estados.acciones;
	  }
   }
   public List<Objetivo> Objetivos {
	  get {
		 return _resolucionmdp.arbol_estados.objetivos;
	  }
   }
   public List<Jugador> Jugadores {
	  get {
		 return _resolucionmdp.arbol_estados.jugadores;
	  }
   }

   public Objeto_Serializable() {

   }

   public Objeto_Serializable(SerializationInfo info, StreamingContext ctxt) {
	  _resolucionmdp = info.GetValue("Resolucion_MDP", typeof(ResolucionMDP)) as ResolucionMDP;

	  nodo_estado = (Nodo_Estado)info.GetValue("Nodo_Estado", typeof(Nodo_Estado));
	  estado = (Estado)info.GetValue("Estado", typeof(Estado));
	  mapa = (Mapa)info.GetValue("Mapa", typeof(Mapa));
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Resolucion_MDP", _resolucionmdp);

	  info.AddValue("Nodo_Estado", nodo_estado);
	  info.AddValue("Estado", estado);
	  info.AddValue("Mapa", mapa);
   }
}
