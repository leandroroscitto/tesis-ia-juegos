using UnityEngine;
using System.Collections;

// Dado un conjunto de luces, las prende y apaga de manera intermitente de acuerdo a un intervalo.
// En el caso de que el intervalo sea 0, se toma un intervalo al azar.
public class Strobe : MonoBehaviour
{
	public Light[] luces;
	// Offset inicial.
	public float inicio;
	// Intervalo de oscilacion.
	public float intervalo;
	private float delta_update;

	void Start ()
	{
		delta_update = inicio + Time.time;
	}

	void Update ()
	{
		if ((intervalo > 0) && (Time.time - delta_update > intervalo)) {
			foreach (Light luz in luces) {
				luz.enabled = !luz.enabled;
			}

			delta_update = Time.time;
		} else if ((intervalo == 0) && (Time.time - delta_update > Random.Range (0, 20 / Time.deltaTime)))
			foreach (Light luz in luces)
				luz.enabled = !luz.enabled;
	}
}
