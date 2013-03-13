using UnityEngine;
using UnitySteer.Helpers;

public class ObjetivoMB : MonoBehaviour {
   public Objetivo objetivo;
   public Radar radar;

   public void inicializar(Objetivo obj, float r, string layer) {
	  objetivo = obj;
	  radar = GetComponent<Radar>();

	  if (radar == null) {
		 radar = gameObject.AddComponent<Radar>();
	  }
	  radar.DetectionRadius = r;
	  radar.DetectDisabledVehicles = true;
	  radar.LayersChecked = 1 << LayerMask.NameToLayer(layer);
	  //radar.OnDetected = OnDetected;
   }

   public System.Action<SteeringEvent<Radar>> OnDetected() {
	  Debug.Log("Detectado!");
	  foreach (Vehicle vehiculo in radar.Vehicles) {
		 JugadorMB jugadormb = vehiculo.gameObject.GetComponent<JugadorMB>();
		 if (jugadormb != null) {
			foreach (Vehicle vehiculo_complementario in objetivo.complementario.objetivo_mb.radar.Vehicles) {
			   JugadorMB jugadormb_complementario = vehiculo_complementario.gameObject.GetComponent<JugadorMB>();
			   if (jugadormb_complementario != null && jugadormb != jugadormb_complementario) {
				  objetivo.cumplido = true;
				  objetivo.complementario.cumplido = true;
				  break;
			   }
			}
		 }
	  }
	  return null;
   }
}