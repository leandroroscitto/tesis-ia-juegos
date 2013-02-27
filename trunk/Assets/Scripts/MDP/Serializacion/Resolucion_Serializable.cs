using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Resolucion_Serializable : ISerializable {
   private ResolucionMDP _resolucionMDP;
   private Arbol_Estados _arbolestados;
   private Mapa _escenario;

   public ResolucionMDP Resolucion_MDP {
	  get {
		 return _resolucionMDP;
	  }
	  set {
		 _resolucionMDP = value;
	  }
   }
   public Arbol_Estados Arbol_Estados {
	  get {
		 return _arbolestados;
	  }
	  set {
		 _arbolestados = value;
	  }
   }
   public Mapa Escenario {
	  get {
		 return _escenario;
	  }
	  set {
		 _escenario = value;
	  }
   }

   public Resolucion_Serializable() {

   }

   public Resolucion_Serializable(SerializationInfo info, StreamingContext ctxt) {
	  _resolucionMDP = info.GetValue("Resolucion_MDP", typeof(ResolucionMDP)) as ResolucionMDP;
	  _arbolestados = info.GetValue("Arbol_Estados", typeof(Arbol_Estados)) as Arbol_Estados;
	  _escenario = info.GetValue("Mapa", typeof(Mapa)) as Mapa;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Resolucion_MDP", _resolucionMDP);
	  info.AddValue("Arbol_Estados", _arbolestados);
	  info.AddValue("Mapa", _escenario);
   }
}
