using System.Collections.Generic;
using UnityEngine;
using UnitySteer.Helpers;
using PathRuntime;

public class ObjetivoMB : MonoBehaviour {
   public Objetivo objetivo;
   public Radar radar;
   public GameObject efecto;

   public void inicializar(Objetivo obj, float r, string layer, GameObject efecto_in) {
	  objetivo = obj;
	  radar = GetComponent<Radar>();

	  if (radar == null) {
		 radar = gameObject.AddComponent<Radar>();
	  }
	  radar.DetectionRadius = r;
	  radar.DetectDisabledVehicles = true;
	  radar.LayersChecked = 1 << LayerMask.NameToLayer(layer);
	  radar.OnDetected = JuegoMB.OnRadarDetect;

	  efecto = efecto_in;
   }

   void Start() {
	  radar.OnDetected = JuegoMB.OnRadarDetect;
   }
}