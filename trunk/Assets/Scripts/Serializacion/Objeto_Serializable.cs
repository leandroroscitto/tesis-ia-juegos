using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Objeto_Serializable : ISerializable {
   private ResolucionMDP _resolucionmdp;

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
   public MDP<Estado, Accion, Objetivo, ResolucionMDP.TransicionJuego, ResolucionMDP.RecompensaJuego> MDP {
	  get {
		 return _resolucionmdp.mdp;
	  }
   }
   public Mapa Mapa {
	  get {
		 return _resolucionmdp.arbol_estados.mapa_base;
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
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Resolucion_MDP", _resolucionmdp);
   }
}
