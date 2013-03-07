﻿using UnityEngine;

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
   }
}