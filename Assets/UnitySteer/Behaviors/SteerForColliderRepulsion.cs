// #define ANNOTATE_AVOIDOBSTACLES
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;
using System.Linq;

/// <summary>
/// Steers a vehicle to be repulsed by stationary obstacles
/// </summary>
/// <remarks>
/// </remarks>
[AddComponentMenu("UnitySteer/Steer/... for ColliderRepulsion")]
public class SteerForColliderRepulsion : Steering {
   #region Structs
   #endregion

   #region Private fields
   [SerializeField]
   float _estimationTime = 2;
   [SerializeField]
   LayerMask _layersChecked;
   [SerializeField]
   float _radius;
   [SerializeField]
   float _smooth = 0.25f;
   [SerializeField]
   float _factor = 1.0f;
   #endregion


   public override bool IsPostProcess {
	  get { return true; }
   }


   #region Public properties
   /// <summary>
   /// How far in the future to estimate the vehicle position
   /// </summary>
   public float EstimationTime {
	  get {
		 return this._estimationTime;
	  }
	  set {
		 _estimationTime = value;
	  }
   }
   /// <summary>
   /// Mascaras consideradas en el raycast.
   /// </summary>
   public LayerMask LayersChecked {
	  get {
		 return this._layersChecked;
	  }
	  set {
		 _layersChecked = value;
	  }
   }

   public float Smooth {
	  get {
		 return this._smooth;
	  }

	  set {
		 _smooth = value;
	  }
   }

   public float Radius {
	  get {
		 return this._radius;
	  }

	  set {
		 _radius = value;
	  }
   }

   public float Factor {
	  get {
		 return this._factor;
	  }

	  set {
		 _factor = value;
	  }
   }
   #endregion

   /// <summary>
   /// Calculates the force necessary to avoid the closest colliders
   /// </summary>
   /// <returns>
   /// Force necessary to avoid an obstacle, or Vector3.zero
   /// </returns>
   /// <remarks>
   /// </remarks>
   protected override Vector3 CalculateForce() {
	  Vector3 avoidance = Vector3.zero;

	  /*
	   * While we could just calculate movement as (Velocity * predictionTime) 
	   * and save ourselves the substraction, this allows other vehicles to
	   * override PredictFuturePosition for their own ends.
	   */
	  Vector3 futurePosition = Vehicle.PredictFutureDesiredPosition(_estimationTime);
	  //Vector3 movement = futurePosition - Vehicle.Position;

#if ANNOTATE_AVOIDOBSTACLES
		Debug.DrawLine(Vehicle.Position, futurePosition, Color.cyan);
#endif

	  int steps = 8;
	  float steps_degrees = 360f / steps;
	  int hitcount = 0;
	  float min_radius = _radius / 2;
	  for (int i = 0; i < steps; i++) {
		 RaycastHit hit;
		 float radius = (_radius - min_radius) * Mathf.Pow((2 * i * 1f / steps - 1), 2) + min_radius;
		 Vector3 direction = Vehicle.transform.rotation * (Quaternion.Euler(0, i * steps_degrees, 0) * Vector3.forward);

		 Debug.DrawRay(Vehicle.Position, direction * radius, Color.green);

		 if (Physics.Raycast(Vehicle.Position, direction, out hit, radius, LayersChecked.value)) {
			Vector3 distanceCurrent = Vehicle.Position - hit.point;
			Vector3 distanceFuture = futurePosition - hit.point;
			avoidance += distanceCurrent / distanceFuture.sqrMagnitude;
			hitcount++;
		 }
		 else {
			avoidance += direction * radius;
		 }
	  }

	  if (hitcount > 0)
		 avoidance /= hitcount;
	  else
		 avoidance = Vector3.zero;
	  Debug.DrawRay(Vehicle.Position, avoidance, Color.yellow);
	  //Vector3 newDesired = Vehicle.DesiredVelocity * _smooth + Vector3.Reflect(Vehicle.DesiredVelocity, avoidance) * (1 - _smooth);
	  Vector3 newDesired = avoidance;
	  Debug.DrawRay(Vehicle.Position, newDesired, Color.blue);

#if ANNOTATE_AVOIDOBSTACLES
		Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.green);
		Debug.DrawLine(Vehicle.Position, futurePosition, Color.blue);
		Debug.DrawLine(Vehicle.Position, Vehicle.Position + newDesired, Color.white);
#endif

	  return newDesired * _factor;
   }

#if ANNOTATE_AVOIDOBSTACLES
	void OnDrawGizmos()
	{
		if (Vehicle != null)
		{
			foreach (var o in Vehicle.Radar.Obstacles)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(o.Position, o.ScaledRadius);
			}
		}
	}
#endif
}
