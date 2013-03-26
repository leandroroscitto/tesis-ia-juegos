using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

[Serializable]
public class Nodo_Estado : Estado_MDP, ISerializable {
   public Estado estado_juego;

   public List<Nodo_Estado> estados_padres;
   // <id_jugador, estados>
   public Dictionary<int, List<Nodo_Estado>> estados_padres_actor;

   public List<Nodo_Estado> estados_hijos;
   // <id_jugador, estados>
   public Dictionary<int, List<Nodo_Estado>> estados_hijos_actor;

   public List<float> estados_hijos_probabilidad;

   public List<Accion> acciones_padres;
   // <id_jugador, acciones>
   public Dictionary<int, List<Accion>> acciones_padres_actor;

   public List<Accion> acciones_hijos;
   // <id_jugador, acciones>
   public Dictionary<int, List<Accion>> acciones_hijos_actor;

   public Nodo_Estado()
	  : base(0) {

   }

   public Nodo_Estado(int i, Estado e)
	  : base(i) {
	  estado_juego = e;
	  estados_padres = new List<Nodo_Estado>();
	  estados_padres_actor = new Dictionary<int, List<Nodo_Estado>>();
	  estados_hijos = new List<Nodo_Estado>();
	  estados_hijos_actor = new Dictionary<int, List<Nodo_Estado>>();
	  estados_hijos_probabilidad = new List<float>();
	  acciones_padres = new List<Accion>();
	  acciones_padres_actor = new Dictionary<int, List<Accion>>();
	  acciones_hijos = new List<Accion>();
	  acciones_hijos_actor = new Dictionary<int, List<Accion>>();

	  foreach (int jugador_id in e.posicion_jugadores.Keys) {
		 acciones_padres_actor.Add(jugador_id, new List<Accion>());
		 acciones_hijos_actor.Add(jugador_id, new List<Accion>());
		 estados_padres_actor.Add(jugador_id, new List<Nodo_Estado>());
		 estados_hijos_actor.Add(jugador_id, new List<Nodo_Estado>());
	  }
   }

   public int AgregarHijo(Nodo_Estado h, Accion ja, float probabilidad) {
	  estados_hijos.Add(h);
	  estados_hijos_actor[ja.actor_id].Add(h);
	  estados_hijos_probabilidad.Add(probabilidad);
	  acciones_hijos.Add(ja);
	  acciones_hijos_actor[ja.actor_id].Add(ja);

	  h.AgregarPadre(this, ja);
	  return estados_hijos.Count - 1;
   }

   public int AgregarPadre(Nodo_Estado p, Accion ja) {
	  estados_padres.Add(p);
	  estados_padres_actor[ja.actor_id].Add(p);
	  acciones_padres.Add(ja);
	  acciones_padres_actor[ja.actor_id].Add(ja);

	  return estados_padres.Count - 1;
   }

   public float probabilidadHijoAccion(Nodo_Estado s, Accion a) {
	  int indice = estados_hijos.IndexOf(s);
	  if (indice >= 0 && acciones_hijos[indice] == a) {
		 return estados_hijos_probabilidad[indice];
	  }
	  else {
		 return 0;
	  }
   }

   public override Estado_MDP hijoAccion(Accion_MDP a) {
	  int index = acciones_hijos.IndexOf((Accion)a);
	  if (index >= 0)
		 return estados_hijos[index];
	  else
		 return null;
   }

   public override Estado_MDP padreAccion(Accion_MDP a) {
	  int index = acciones_padres.IndexOf((Accion)a);
	  if (index >= 0)
		 return estados_padres[index];
	  else
		 return null;
   }

   public override Accion_MDP[] accionesValidas(int actor_id) {
	  if (actor_id == -1) {
		 return acciones_hijos.ToArray();
	  }
	  else {
		 return acciones_hijos_actor[actor_id].ToArray();
	  }
   }

   public override Estado_MDP[] proximosEstados(int actor_id) {
	  if (actor_id == -1) {
		 return estados_hijos.ToArray();
	  }
	  else {
		 return estados_hijos_actor[actor_id].ToArray();
	  }
   }

   public override string ToString() {
	  return "Estado_ID: " + estado_juego.id + ", hijos: " + estados_hijos.Count + ", padres: " + estados_padres.Count;
   }

   // Serializacion
   public Nodo_Estado(SerializationInfo info, StreamingContext ctxt)
	  : base(info, ctxt) {
	  estado_juego = info.GetValue("Estado_Actual", typeof(Estado)) as Estado;

	  estados_padres = info.GetValue("Estados_Padres", typeof(List<Nodo_Estado>)) as List<Nodo_Estado>;
	  estados_padres_actor = info.GetValue("Estados_Padres_Actor", typeof(Dictionary<int, List<Nodo_Estado>>)) as Dictionary<int, List<Nodo_Estado>>;
	  estados_hijos = info.GetValue("Estados_Hijos", typeof(List<Nodo_Estado>)) as List<Nodo_Estado>;
	  estados_hijos_actor = info.GetValue("Estados_Hijos_Actor", typeof(Dictionary<int, List<Nodo_Estado>>)) as Dictionary<int, List<Nodo_Estado>>;
	  estados_hijos_probabilidad = info.GetValue("Estados_Hijos_Probabilidad", typeof(List<float>)) as List<float>;

	  acciones_padres = info.GetValue("Acciones_Padres", typeof(List<Accion>)) as List<Accion>;
	  acciones_padres_actor = info.GetValue("Acciones_Padres_Actor", typeof(Dictionary<int, List<Accion>>)) as Dictionary<int, List<Accion>>;
	  acciones_hijos = info.GetValue("Acciones_Hijos", typeof(List<Accion>)) as List<Accion>;
	  acciones_hijos_actor = info.GetValue("Acciones_Hijos_Actor", typeof(Dictionary<int, List<Accion>>)) as Dictionary<int, List<Accion>>;
   }

#pragma warning disable 0114
   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  base.GetObjectData(info, ctxt);

	  info.AddValue("Estado_Actual", estado_juego);

	  info.AddValue("Estados_Padres", estados_padres);
	  info.AddValue("Estados_Padres_Actor", estados_padres_actor);
	  info.AddValue("Estados_Hijos", estados_hijos);
	  info.AddValue("Estados_Hijos_Actor", estados_hijos_actor);
	  info.AddValue("Estados_Hijos_Probabilidad", estados_hijos_probabilidad);

	  info.AddValue("Acciones_Padres", acciones_padres);
	  info.AddValue("Acciones_Padres_Actor", acciones_padres_actor);
	  info.AddValue("Acciones_Hijos", acciones_hijos);
	  info.AddValue("Acciones_Hijos_Actor", acciones_hijos_actor);
   }
#pragma warning restore 0114
}
