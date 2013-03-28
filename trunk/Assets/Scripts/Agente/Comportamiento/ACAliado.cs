using UnityEngine;
using Behave.Runtime;
using Tree = Behave.Runtime.Tree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathRuntime;
using UnitySteer;

public class ACAliado : ArbolComportamientoBase, ACIr_A {

   public LayerMask layers_bloqueantes;
   public Vector3 target;
   public Vector3 target_previo;
   public float delta_verificar_movimiento;
   public float minimo_movimiento_esperado;
   public Vector3 ultima_posicion_verificada;
   public float ultimo_tiempo_verificado;

   public GameObject pointer1, pointer2;

   private SteerForPathSimplified steerBPathFollow;
   private SteerForWanderPropio steerWander;
   private Vector3Pathway camino;
   private Vehicle target_agente_aliado;
   private ObjetivoMB target_objetivo;
   private Vehicle agente;

   private JuegoMB juego;
   private JugadorMB jugadormb;

   protected override void inicializacionParticular() {
	  tipo_arbol = BLComportamiento.TreeType.Agentes_Aliado;

	  steerBPathFollow = GetComponent<SteerForPathSimplified>();
	  steerWander = GetComponent<SteerForWanderPropio>();

	  agente = GetComponent<Vehicle>();

	  ultima_posicion_verificada = agente.Position;
	  ultimo_tiempo_verificado = Time.time;

	  target = transform.position;
	  target_previo = target;

	  juego = GameObject.Find("Juego").GetComponent<JuegoMB>();
	  jugadormb = GetComponent<JugadorMB>();

	  pointer1 = Instantiate(pointer1, Vector3.up * 1000, Quaternion.LookRotation(Vector3.up)) as GameObject;
	  pointer1.GetComponent<Pointer>().duracion = 1000;
	  pointer1.GetComponent<Pointer>().eje = Vector3.up;
	  pointer2 = Instantiate(pointer2, Vector3.up * 1000, Quaternion.LookRotation(Vector3.up)) as GameObject;
	  pointer2.GetComponent<Pointer>().duracion = 1000;
	  pointer2.GetComponent<Pointer>().eje = Vector3.up;

	  // Subarbol Ir_a
	  handlers.Add((int)BLComportamiento.ActionType.Existe_camino_directo, Existe_camino_directo);
	  handlers.Add((int)BLComportamiento.ActionType.Existe_camino_indirecto, Existe_camino_indirecto);
	  handlers.Add((int)BLComportamiento.ActionType.Calcular_camino_directo, Calcular_camino_directo);
	  handlers.Add((int)BLComportamiento.ActionType.Calcular_camino_indirecto, Calcular_camino_indirecto);
	  handlers.Add((int)BLComportamiento.ActionType.Ir_a_directo, Ir_a_directo);
	  handlers.Add((int)BLComportamiento.ActionType.Ir_a_indirecto, Ir_a_indirecto);
	  handlers.Add((int)BLComportamiento.ActionType.Verificar_movimiento, Verificar_movimiento);

	  // Patrullar
	  handlers.Add((int)BLComportamiento.ActionType.Calcular_posicion_azar, Calcular_posicion_azar);

	  // Seguir aliados
	  handlers.Add((int)BLComportamiento.ActionType.Existe_aliado_cerca, Existe_aliado_cerca);
	  handlers.Add((int)BLComportamiento.ActionType.Calcular_posicion_aliado, Calcular_posicion_aliado);

	  // Cumplir objetivos
	  handlers.Add((int)BLComportamiento.ActionType.Existe_objetivo_sin_cumplir, Existe_objetivo_sin_cumplir);
	  handlers.Add((int)BLComportamiento.ActionType.Inferir_objetivo_a_cumplir, Inferir_objetivo_a_cumplir);
	  handlers.Add((int)BLComportamiento.ActionType.Puede_cumplir_objetivo, Puede_cumplir_objetivo);
	  handlers.Add((int)BLComportamiento.ActionType.Cumplir_objetivo, Cumplir_objetivo);
   }

   protected override void AIUpdate() {
	  tree.Tick();
   }

   public override BehaveResult Tick(Tree sender, bool init) {
	  if (BLComportamiento.IsAction(tree.ActiveID))
		 Debug.Log("Accion no manejada: " + ((BLComportamiento.ActionType)sender.ActiveID).ToString());
	  if (BLComportamiento.IsDecorator(tree.ActiveID))
		 Debug.Log("Decorador no manejado: " + ((BLComportamiento.DecoratorType)sender.ActiveID).ToString());
	  return BehaveResult.Failure;
   }

   // Determina si existe un objetivo sin cumplir.
   public BehaveResult Existe_objetivo_sin_cumplir(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  target_objetivo = null;
	  if (juego.nodo_estado_actual.estado_juego.objetivos_no_cumplidos.Count > 0) {
		 return BehaveResult.Success;
	  }
	  return BehaveResult.Failure;
   }

   // Infiere el objetivo a cumplir por el jugador controlado por el humano, y determina el objetivo actual como el complementario de este.
   public BehaveResult Inferir_objetivo_a_cumplir(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  float[] valor_objetivo;
	  List<ObjetivoMB> objetivosmb = juego.inferirObjetivo(juego.jugadores[0], juego.profundidad_acciones, juego.factor_descuento, out valor_objetivo);

	  if (objetivosmb.Count > 0) {
		 float menor_distancia = float.PositiveInfinity;
		 ObjetivoMB menor_objetivomb = null;
		 foreach (ObjetivoMB objetivomb in objetivosmb) {
			float distancia = Vector3.Distance(juego.jugadores[0].jugador.posicion, objetivomb.objetivo.posicion) + Vector3.Distance(jugadormb.jugador.posicion, objetivomb.objetivo.complementario.posicion);
			if (distancia < menor_distancia) {
			   menor_distancia = distancia;
			   menor_objetivomb = objetivomb;
			}
		 }

		 pointer1.GetComponent<Pointer>().posicion_base = menor_objetivomb.objetivo.posicion;
		 pointer2.GetComponent<Pointer>().posicion_base = menor_objetivomb.objetivo.complementario.posicion;

		 target_objetivo = menor_objetivomb.objetivo.complementario.objetivo_mb;

		 target = target_objetivo.transform.position;
		 target.y = agente.Position.y;
		 return BehaveResult.Success;
	  }
	  else {
		 return BehaveResult.Failure;
	  }
   }

   // Determina si esta a una distancia suficiente para cumplir el objetivo (si el companero esta en el complementario).
   public BehaveResult Puede_cumplir_objetivo(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  if (target_objetivo.radar.Vehicles.Contains(agente)) {
		 return BehaveResult.Success;
	  }
	  else {
		 return BehaveResult.Failure;
	  }
   }

   // En el caso de estar sobre el objetivo a cumplir, no realiza accion hasta que este se haya cumplido.
   public BehaveResult Cumplir_objetivo(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  return BehaveResult.Success;
   }

   // Determina si existe un aliado una distancia detectable. El tag determina el equipo al cual pertenece el agente.
   public BehaveResult Existe_aliado_cerca(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  foreach (Vehicle otro_agente in agente.Radar.Vehicles) {
		 if (otro_agente.tag == agente.tag) {
			target_agente_aliado = otro_agente;
			return BehaveResult.Success;
		 }
	  }
	  target_agente_aliado = null;
	  return BehaveResult.Failure;
   }

   // Determina la posicion futura del aliado para determinar la posicion del target del ir_a.
   public BehaveResult Calcular_posicion_aliado(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  Vector3 direccion = target_agente_aliado.PredictFutureDesiredPosition(0.5f) - agente.Position;
	  if (direccion.magnitude - target_agente_aliado.Radius < agente.ArrivalRadius) {
		 target = agente.Position - direccion.normalized * agente.ArrivalRadius / 2;
	  }
	  else {
		 target = agente.Position + direccion;
	  }
	  return BehaveResult.Success;
   }

   // Calcula una posicion al azar a partir de los waypoints de la malla de navegacion.
   public BehaveResult Calcular_posicion_azar(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  if (Vector3.Distance(agente.Position, target) < 1.5f * agente.ArrivalRadius) {
		 Waypoint waypoint = Navigation.Waypoints[Random.Range(0, Navigation.Waypoints.Count)];
		 target = waypoint.Position;
		 target.y = agente.Position.y;
	  }

	  return BehaveResult.Success;
   }

   // Subarbol Ir_a

   // Determina si existe un camino directo al target, es decir no existen obstaculos entre el agente y este.
   public BehaveResult Existe_camino_directo(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  Vector3 direccion = target - agente.Position;
	  if (!Physics.Raycast(agente.Position, direccion, direccion.magnitude, layers_bloqueantes.value))
		 return BehaveResult.Success;
	  else
		 return BehaveResult.Failure;
   }

   // Determina si existe un camino al target obtenido por medio de pathfinding. Puede tardar mas de una evaluacion completa del arbol de comportamiento para determinar el camino, por lo tanto se espera.
   public BehaveResult Existe_camino_indirecto(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  if (target_previo != target) {
		 camino = new Vector3Pathway(Generador_Navegacion.getMinimoCamino(agente.Position, target), agente.Radius * 0.75f, false);
		 target_previo = target;
	  }
	  return BehaveResult.Success;
   }

   // El camino directo se compone por el segmento que une al agente con el target.
   public BehaveResult Calcular_camino_directo(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  Vector3Pathway v3path = new Vector3Pathway();
	  v3path.AddPoint(agente.Position);
	  v3path.AddPoint(target + Vector3.up * (agente.Position.y - target.y));

	  steerBPathFollow.Path = v3path;
	  return BehaveResult.Success;
   }

   // El camino indirecto es el obtenido por medio del pathfinding. Se lo transforma a un formato que sea facil de seguir por el steering behavior.
   public BehaveResult Calcular_camino_indirecto(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  steerBPathFollow.Path = camino;
	  return BehaveResult.Success;
   }

   // Se mueve directamente hacia el target.
   public BehaveResult Ir_a_directo(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  // TODO: Chequear por obstaculos.
	  return BehaveResult.Success;
   }

   // Sigue el camino determinado hacia el target.
   public BehaveResult Ir_a_indirecto(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  // TODO: Chequear por obstaculos.
	  return BehaveResult.Success;
   }

   // Verifica cada cierto intervalo si el agente se movio (en el caso de que este siguiendo un target).
   // En caso contrario vueve a calcular el camino al target (de ser necesario), y activa momentaneamente el wander para tartar de destrabarlo.
   public BehaveResult Verificar_movimiento(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data) {
	  if (Time.time - ultimo_tiempo_verificado > delta_verificar_movimiento) {
		 // Si todabia no llego al target, y no se movio mas del delta esperado:
		 Vector3 movimiento = agente.Position - ultima_posicion_verificada;
		 if ((Vector3.Distance(agente.Position, target) > agente.ArrivalRadius) && (movimiento.magnitude < minimo_movimiento_esperado)) {
			// Vuelve a calcular el path al target, por si ese es el problema.
			camino = new Vector3Pathway(Generador_Navegacion.getMinimoCamino(agente.Position, target), agente.Radius * 0.75f, false);
			// Realiza wander por un instante para tratar de destrabarlo.
			steerWander.enabled = true;
			ultimo_tiempo_verificado = Time.time;
			return BehaveResult.Failure;
		 }
		 else {
			steerWander.enabled = false;
			ultimo_tiempo_verificado = Time.time;
			ultima_posicion_verificada = agente.Position;
			return BehaveResult.Success;
		 }
	  }
	  return BehaveResult.Success;
   }
}
