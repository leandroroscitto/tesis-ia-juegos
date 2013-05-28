using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class Estado_MDP : ISerializable {
   public int id;

   public Estado_MDP(int i) {
	  id = i;
   }

   public abstract Estado_MDP[] proximosEstados(int actor_id);
   public abstract Accion_MDP[] accionesValidas(int actor_id);
   public abstract Estado_MDP hijoAccion(Accion_MDP a);
   public abstract Estado_MDP padreAccion(Accion_MDP a);

   // Serializacion
   public Estado_MDP(SerializationInfo info, StreamingContext ctxt) {
	  id = info.GetInt32("Id");
   }
   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Id", id);
   }
}
[Serializable]
public abstract class Accion_MDP : ISerializable {
   public int id;
   public int actor_id;

   public Accion_MDP(int i, int ai) {
	  id = i;
	  actor_id = ai;
   }

   // Serializacion
   public Accion_MDP(SerializationInfo info, StreamingContext ctxt) {
	  id = info.GetInt32("Id");
	  actor_id = info.GetInt32("Actor_Id");
   }
   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Id", id);
	  info.AddValue("Actor_Id", actor_id);
   }
}
[Serializable]
public abstract class Objetivo_MDP : ISerializable {
   public int id;

   public Objetivo_MDP(int i) {
	  id = i;
   }

   // Serializacion
   public Objetivo_MDP(SerializationInfo info, StreamingContext ctxt) {
	  id = info.GetInt32("Id");
   }
   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Id", id);
   }
}
[Serializable]
public abstract class Transicion_MDP<S, A> {
   public abstract float getValor(A a, S s, S sp);
}
[Serializable]
public abstract class Recompensa_MDP<S, O> {
   public abstract float getValor(S s, O o, int actor_id);
   public abstract Vector2 getPosicionObjetivo(O obj);
}

[Serializable]
public class MDP<S, A, O, T, R>
   where S : Estado_MDP
   where A : Accion_MDP
   where O : Objetivo_MDP
   where T : Transicion_MDP<S, A>
   where R : Recompensa_MDP<S, O> {
   public List<S> estados;
   public List<A> acciones;
   public List<O> objetivos;
   public int numero_actores;
   public T transicion;
   public R recompensa;
   public float factor_descuento;

   // <jugador_id, objetivo_id, estado_id>
   public float[][][] Utilidad;
   public A[][][] Politica;

   public MDP() {

   }

   public MDP(List<S> est, List<A> acs, List<O> objs, int na, T trn, R rep, float fac) {
	  estados = est;
	  for (int i = 0; i < estados.Count; i++)
		 estados[i].id = i;
	  acciones = acs;
	  for (int j = 0; j < acciones.Count; j++)
		 acciones[j].id = j;
	  objetivos = new List<O>();
	  for (int q = 0; q < objs.Count; q++) {
		 objetivos.Insert(objs[q].id, objs[q]);
	  }

	  numero_actores = na;
	  transicion = trn;
	  recompensa = rep;
	  factor_descuento = fac;

	  //Calcular_Utilidad_VI();
	  Calcular_Utilidad_PI();

	  float max_utilidad = float.MinValue;
	  for (int i = 0; i < Utilidad.Length; i++) {
		 for (int j = 0; j < Utilidad[i].Length; j++) {
			for (int q = 0; q < Utilidad[i][j].Length; q++) {
			   max_utilidad = Mathf.Max(max_utilidad, Utilidad[i][j][q]);
			}
		 }
	  }

	  for (int i = 0; i < Utilidad.Length; i++) {
		 for (int j = 0; j < Utilidad[i].Length; j++) {
			for (int q = 0; q < Utilidad[i][j].Length; q++) {
			   Utilidad[i][j][q] /= max_utilidad;
			}
		 }
	  }
   }

   public void Calcular_Utilidad_PI() {
	  float[][][] Utilidad_Aux = new float[numero_actores][][];
	  A[][][] Politica_Aux = new A[numero_actores][][];
	  float[][][] Value_Policy = new float[numero_actores][][];
	  for (int actor = 0; actor < numero_actores; actor++) {
		 Utilidad_Aux[actor] = new float[objetivos.Count][];
		 Politica_Aux[actor] = new A[objetivos.Count][];
		 Value_Policy[actor] = new float[objetivos.Count][];
		 for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
			Utilidad_Aux[actor][objetivo_id] = new float[estados.Count];
			Politica_Aux[actor][objetivo_id] = new A[estados.Count];
			Value_Policy[actor][objetivo_id] = new float[estados.Count];
		 }
	  }

	  foreach (S i in estados) {
		 for (int actor = 0; actor < numero_actores; actor++) {
			A[] acciones_validas = (A[])i.accionesValidas(actor);
			for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
			   A accion = acciones_validas[Random.Range(0, acciones_validas.Length)];
			   Politica_Aux[actor][objetivo_id][i.id] = accion;
			   Utilidad_Aux[actor][objetivo_id][i.id] = recompensa.getValor(i, objetivos[objetivo_id], actor);
			}
		 }
	  }

	  bool sincambios;
	  float diferencia_total;
	  do {
		 int cont_porcentaje = 10;
		 // Value_Determination
		 foreach (S i in estados) {
			for (int actor = 0; actor < numero_actores; actor++) {
			   for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
				  Utilidad_Aux[actor][objetivo_id][i.id] = recompensa.getValor(i, objetivos[objetivo_id], actor) + factor_descuento * Value_Policy[actor][objetivo_id][i.id];
			   }
			}
		 }
		 sincambios = true;
		 diferencia_total = 0;

		 foreach (S i in estados) {
			A[][] action_max = new A[numero_actores][];
			float[][] value_max = new float[numero_actores][];
			for (int actor = 0; actor < numero_actores; actor++) {
			   action_max[actor] = new A[objetivos.Count];
			   value_max[actor] = new float[objetivos.Count];
			   for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
				  action_max[actor][objetivo_id] = null;
				  value_max[actor][objetivo_id] = float.MinValue;
			   }
			}

			for (int actor = 0; actor < numero_actores; actor++) {
			   for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
				  foreach (A a in i.accionesValidas(actor)) {
					 float value = transicion.getValor(a, i, (S)i.hijoAccion(a)) * Utilidad_Aux[actor][objetivo_id][i.hijoAccion(a).id];
					 if (value > value_max[actor][objetivo_id]) {
						value_max[actor][objetivo_id] = value;
						action_max[actor][objetivo_id] = a;
					 }
				  }
			   }
			}

			float[][] value_policy = new float[numero_actores][];
			for (int actor = 0; actor < numero_actores; actor++) {
			   value_policy[actor] = new float[objetivos.Count];
			   for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
				  value_policy[actor][objetivo_id] = 0;
			   }
			}

			for (int actor = 0; actor < numero_actores; actor++) {
			   for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
				  A accion_politica = Politica_Aux[actor][objetivo_id][i.id];
				  value_policy[actor][objetivo_id] = transicion.getValor(accion_politica, i, (S)i.hijoAccion(accion_politica)) * Utilidad_Aux[actor][objetivo_id][i.hijoAccion(accion_politica).id];

				  float diferencia = Mathf.Abs(value_max[actor][objetivo_id] - value_policy[actor][objetivo_id]);
				  if (diferencia != 0) {
					 diferencia_total += diferencia;
					 Politica_Aux[actor][objetivo_id][i.id] = action_max[actor][objetivo_id];
					 Value_Policy[actor][objetivo_id][i.id] = value_max[actor][objetivo_id];
					 sincambios = false;
				  }
				  else {
					 Value_Policy[actor][objetivo_id][i.id] = value_policy[actor][objetivo_id];
				  }
			   }
			}

			float porcentaje = (i.id * 100f / estados.Count);

			if (porcentaje % 10 <= 0.1 && porcentaje / 10 > (10 - cont_porcentaje)) {
			   //Debug.Log("Progreso: " + (int)porcentaje + ", diferencia: " + diferencia_total);
			   cont_porcentaje--;
			}
		 }
	  } while (!sincambios);

	  Utilidad = Utilidad_Aux;
	  Politica = Politica_Aux;
   }

   public void Calcular_Utilidad_VI() {
	  float[][][] Utilidad_Aux = new float[numero_actores][][];
	  A[][][] Politica_Aux = new A[numero_actores][][];
	  Utilidad = new float[numero_actores][][];
	  for (int actor = 0; actor < numero_actores; actor++) {
		 Utilidad_Aux[actor] = new float[objetivos.Count][];
		 Politica_Aux[actor] = new A[objetivos.Count][];
		 Utilidad[actor] = new float[objetivos.Count][];
		 for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
			Utilidad_Aux[actor][objetivo_id] = new float[estados.Count];
			Politica_Aux[actor][objetivo_id] = new A[estados.Count];
			Utilidad[actor][objetivo_id] = new float[estados.Count];
		 }
	  }

	  for (int actor = 0; actor < numero_actores; actor++) {
		 for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
			foreach (S i in estados) {
			   Utilidad_Aux[actor][objetivo_id][i.id] = recompensa.getValor(i, objetivos[objetivo_id], actor);
			}
		 }
	  }

	  float[][] value_max = new float[numero_actores][];
	  for (int actor = 0; actor < numero_actores; actor++) {
		 value_max[actor] = new float[objetivos.Count];
	  }

	  do {
		 int cont_porcentaje = 10;
		 for (int actor = 0; actor < Utilidad_Aux.Length; actor++) {
			for (int objetivo = 0; objetivo < Utilidad_Aux[actor].Length; objetivo++) {
			   for (int estado = 0; estado < Utilidad_Aux[actor][objetivo].Length; estado++) {
				  Utilidad[actor][objetivo][estado] = Utilidad_Aux[actor][objetivo][estado];
			   }
			}
		 }

		 int count = 0;
		 foreach (S i in estados) {
			for (int actor = 0; actor < numero_actores; actor++) {
			   for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
				  value_max[actor][objetivo_id] = float.MinValue;
				  foreach (A a in i.accionesValidas(actor)) {
					 //float value = 0;
					 //foreach (S j in i.proximosEstados(a.actor_id)) {
					 //   value += transicion.valor(a, i, j) * Utilidad_Aux[a.actor_id][objetivo_id][j.id];
					 //}
					 float value = transicion.getValor(a, i, (S)i.hijoAccion(a)) * Utilidad_Aux[actor][objetivo_id][i.hijoAccion(a).id];
					 if (value > value_max[actor][objetivo_id]) {
						value_max[actor][objetivo_id] = value;
						Politica_Aux[actor][objetivo_id][i.id] = a;
					 }

					 count++;
					 float porcentaje = (100 * (count * 1f / (estados.Count * acciones.Count * numero_actores)));
					 if (porcentaje % 10 <= 0.1 && porcentaje / 10 > (10 - cont_porcentaje)) {
						Debug.Log("Progreso: " + (int)porcentaje);
						cont_porcentaje--;
					 }
				  }

				  Utilidad_Aux[actor][objetivo_id][i.id] = recompensa.getValor(i, objetivos[objetivo_id], actor) + factor_descuento * value_max[actor][objetivo_id];
			   }
			}
		 }

	  } while (!similares(Utilidad_Aux, Utilidad, 0.2f));

	  Utilidad = Utilidad_Aux;
	  Politica = Politica_Aux;
   }

   public bool similares(float[][][] a, float[][][] b, float delta) {
	  float suma = 0;
	  for (int actor = 0; actor < numero_actores; actor++) {
		 for (int objetivo_id = 0; objetivo_id < objetivos.Count; objetivo_id++) {
			foreach (S i in estados) {
			   suma += Mathf.Pow(a[actor][objetivo_id][i.id] - b[actor][objetivo_id][i.id], 2);
			}
		 }
	  }

	  double rms = Mathf.Sqrt(suma) / estados.Count;
	  Debug.Log(rms);
	  return (rms < delta);
   }

   // Serializacion
   public MDP(SerializationInfo info, StreamingContext ctxt) {
	  estados = info.GetValue("Estados", typeof(List<S>)) as List<S>;
	  acciones = info.GetValue("Acciones", typeof(List<A>)) as List<A>;
	  objetivos = info.GetValue("Objetivos", typeof(List<O>)) as List<O>;
	  numero_actores = info.GetInt16("Numero_Actores");
	  transicion = info.GetValue("Transicion", typeof(T)) as T;
	  recompensa = info.GetValue("Recompensa", typeof(R)) as R;
	  factor_descuento = (float)info.GetDouble("Factor_Descuento");

	  Utilidad = info.GetValue("Utilidad", typeof(float[][][])) as float[][][];
	  Politica = info.GetValue("Politica", typeof(A[][][])) as A[][][];
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Estados", estados);
	  info.AddValue("Acciones", acciones);
	  info.AddValue("Objetivos", objetivos);
	  info.AddValue("Numero_Actores", numero_actores);
	  info.AddValue("Transicion", transicion);
	  info.AddValue("Recompensa", recompensa);
	  info.AddValue("Factor_Descuento", factor_descuento);

	  info.AddValue("Utilidad", Utilidad);
	  info.AddValue("Politica", Politica);
   }
}