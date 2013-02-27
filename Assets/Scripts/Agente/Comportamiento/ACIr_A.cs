using UnityEngine;
using Behave.Runtime;
using Tree = Behave.Runtime.Tree;
using System.Collections;
using System.Collections.Generic;

public interface ACIr_A {
   BehaveResult Existe_camino_directo(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data);
   BehaveResult Existe_camino_indirecto(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data);
   BehaveResult Calcular_camino_directo(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data);
   BehaveResult Calcular_camino_indirecto(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data);
   BehaveResult Ir_a_directo(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data);
   BehaveResult Ir_a_indirecto(Tree sender, string stringParameter, float floatParameter, IAgent agent, object data);
}
