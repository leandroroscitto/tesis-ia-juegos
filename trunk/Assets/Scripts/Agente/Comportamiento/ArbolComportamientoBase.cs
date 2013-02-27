using UnityEngine;
using Behave.Runtime;
using Tree = Behave.Runtime.Tree;
using System.Collections;
using System.Collections.Generic;

public abstract class ArbolComportamientoBase : MonoBehaviour, IAgent
{
	public Tree tree;
	public float frecuencia;
	public Dictionary<int, TickForward> handlers;
	public BLComportamiento.TreeType tipo_arbol;

	protected virtual void inicializacionParticular ()
	{

	}

	protected virtual IEnumerator Start ()
	{
		handlers = new Dictionary<int, TickForward> ();
		tipo_arbol = BLComportamiento.TreeType.Unknown;

		inicializacionParticular ();

		tree = BLComportamiento.InstantiateTree (tipo_arbol, this);

		foreach (int id in handlers.Keys) {
			tree.SetTickForward (id, handlers [id]);
		}

		if (tree != null)
			frecuencia = (1.0f / tree.Frequency);

		while (Application.isPlaying && (tree != null)) {
			yield return new WaitForSeconds(frecuencia);
			AIUpdate ();
		}
	}

	protected virtual void AIUpdate ()
	{
		tree.Tick ();
	}

	public virtual BehaveResult Tick (Tree sender, bool init)
	{

		if (BLComportamiento.IsAction (tree.ActiveID))
			Debug.Log ("Accion no manejada: " + ((BLComportamiento.ActionType)sender.ActiveID).ToString ());
		if (BLComportamiento.IsDecorator (tree.ActiveID))
			Debug.Log ("Decorador no manejado: " + ((BLComportamiento.DecoratorType)sender.ActiveID).ToString ());

		return BehaveResult.Failure;
	}

	public virtual void Reset (Tree sender)
	{

	}

	public virtual int SelectTopPriority (Tree sender, params int[] IDs)
	{
		return IDs [0];
	}
}
