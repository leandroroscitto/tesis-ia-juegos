using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;

/// <summary>
/// Steers a vehicle to wander around
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for WanderPropio")]
public class SteerForWanderPropio : Steering {
   #region Private fields
   /// <summary>
   /// Direccion relativa actual del wandering.
   /// </summary>
   private Vector3 direction;
   /// <summary>
   /// Radio de la esfera centrada en el offset de la posicion del agente.
   /// </summary>
   private float radiusR;
   #endregion

   #region Public properties
   /// <summary>
   /// Determina la intensidad del steering.
   /// 0 no aplica ningun steering, 1 implica maxima fuerza.
   /// </summary>
   public float force = 1;
   /// <summary>
   /// Distancia maxima de cambio de direccion.
   /// 0 no cambia el steering actual, 1 implica una posicion al azar.
   /// </summary>
   public float range = 1;
   /// <summary>
   /// El offset dirige el wandering.
   /// </summary>
   public Vector3 offset = Vector3.forward * 5;
   #endregion


   protected override Vector3 CalculateForce() {
	  Vector3 velocity = Vehicle.Velocity;

	  // Calcula el radio de la esfera de acuerdo al offset actual y la rotacion maxima del agente.
	  // De esta manera la rotacion maxima necesaria del steering resultante no superara la del agente.
	  radiusR = Mathf.Tan((45f / 2f) * Mathf.PI / 180) * offset.magnitude;

	  // Punto en la superficia de la esfera proyectando la direccion actual.
	  Vector3 X0 = offset + (direction * 0.75f + velocity * 0.25f).normalized * radiusR;
	  // Calcula una separacion al azar de acuerdo al rango a partir del punto anterior.
	  Vector3 punto_azar;
	  if (Vehicle.IsPlanar) {
		 punto_azar = Random.insideUnitCircle;
		 punto_azar = new Vector3(punto_azar.x, 0, punto_azar.y);
	  }
	  else
		 punto_azar = Random.onUnitSphere;
	  Vector3 punto = (X0 + punto_azar * range * radiusR);

	  // Determina la direccion del steering a partir del punto calculado y el parametro fuerza.
	  direction = (punto - offset).normalized * radiusR * force;
	  //Debug.DrawRay(Vehicle.Position, direction, Color.blue);

	  return direction;
   }

}

