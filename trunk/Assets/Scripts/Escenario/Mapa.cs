using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using PathRuntime;
using Random = UnityEngine.Random;

[Serializable]
public class Mapa : MonoBehaviour {
   [Serializable]
   public class Conexion : Seccion {
	  public Conexion(int x1_in, int y1_in, int x2_in, int y2_in)
		 : base(x1_in, y1_in, x2_in, y2_in) {
	  }
   }

   [Serializable]
   public class Pared : Seccion {
	  public Pared(int x1_in, int y1_in, int x2_in, int y2_in)
		 : base(x1_in, y1_in, x2_in, y2_in) {
	  }
   }

   [Serializable]
   public class Habitacion : Seccion, ISerializable {
	  public List<Conexion> conexiones;

	  public Habitacion(int x1_in, int y1_in, int x2_in, int y2_in)
		 : base(x1_in, y1_in, x2_in, y2_in) {
		 conexiones = new List<Conexion>();
	  }

	  // Serializacion
	  public Habitacion(SerializationInfo info, StreamingContext ctxt)
		 : base(info, ctxt) {
		 conexiones = info.GetValue("Conexiones", typeof(List<Conexion>)) as List<Conexion>;
	  }

	  public new void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
		 base.GetObjectData(info, ctxt);

		 info.AddValue("Conexiones", conexiones);
	  }
   }

   [Serializable]
   public class Seccion : ISerializable {
	  public int x1, y1, x2, y2;
	  protected int id;
	  private static int cont = 0;

	  public int ancho {
		 get {
			return x2 - x1 + 1;
		 }
	  }

	  public int largo {
		 get {
			return y2 - y1 + 1;
		 }
	  }

	  public int area {
		 get {
			return ancho * largo;
		 }
	  }

	  public int perimetro {
		 get {
			return (ancho + largo) * 2;
		 }
	  }

	  public Seccion(int x1_in, int y1_in, int x2_in, int y2_in) {
		 id = Seccion.cont;
		 Seccion.cont++;

		 x1 = x1_in;
		 y1 = y1_in;
		 x2 = x2_in;
		 y2 = y2_in;
	  }

	  public override int GetHashCode() {
		 return id;
	  }

	  public override bool Equals(object obj) {
		 Seccion sec_obj = (Seccion)obj;
		 return (id == sec_obj.id);
	  }

	  // Serializacion
	  public Seccion(SerializationInfo info, StreamingContext ctxt) {
		 x1 = info.GetInt16("X1");
		 x2 = info.GetInt16("X2");
		 y1 = info.GetInt16("Y1");
		 y2 = info.GetInt16("Y2");
		 id = info.GetInt32("Id");

		 Seccion.cont = Mathf.Max(id + 1, cont);
	  }

	  public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
		 info.AddValue("X1", x1);
		 info.AddValue("X2", x2);
		 info.AddValue("Y1", y1);
		 info.AddValue("Y2", y2);
		 info.AddValue("Id", id);
	  }
   }

   // Atributos
   [HideInInspector]
   public Tile[] mapa;
   public static int cant_x, cant_y;
   public Vector3 tamano_zona;
   public LayerMask mascara;

   [HideInInspector]
   public GameObject navigation_objeto;
   [HideInInspector]
   public Navigation navigation;

   [HideInInspector]
   public GameObject mdp_objeto;
   [HideInInspector]
   public ResolucionMDP resolucion_mdp;
   [HideInInspector]
   public Arbol_Estados arbol_e;

   // Representacion visual
   public Material mesh_material;
   public Material piso_material;
   public Material fuente_material;
   public Font fuente_objetivos;

   // Dimensiones
   float ancho, largo;
   float offsetx, offsety;

   // Representacion interna
   private GameObject mesh_objeto;
   private GameObject piso;

   // Listas
   private List<Pared> paredes;
   private List<Habitacion> habitaciones;
   private List<Conexion> conexiones_horizontales;
   private List<Conexion> conexiones_verticales;
   private List<Vector2> intersecciones;

   // Objetivos
   private List<Objetivo> objetivos;
   private List<ObjetoMB<Objetivo>> objetivos_mb;

   // Jugadores
   private List<Jugador> jugadores;
   private List<ObjetoMB<Jugador>> jugadores_mb;

   // Acciones
   private List<Accion> acciones;

   // Operaciones

   public void OnDrawGizmosSelected() {
	  if (conexiones_horizontales != null) {
		 foreach (Conexion conexion in conexiones_horizontales) {
			Gizmos.DrawLine(posicionRepresentacionAReal(new Vector2(conexion.x1, conexion.y1), 1.25f), posicionRepresentacionAReal(new Vector2(conexion.x2, conexion.y2), 1.25f));
		 }
	  }

	  if (conexiones_verticales != null) {
		 foreach (Conexion conexion in conexiones_verticales) {
			Gizmos.DrawLine(posicionRepresentacionAReal(new Vector2(conexion.x1, conexion.y1), 1.25f), posicionRepresentacionAReal(new Vector2(conexion.x2, conexion.y2), 1.25f));
		 }
	  }
   }

   public Mapa(int cx, int cy, Vector3 tz) {
	  inicializarMapa(cx, cy, tz);
   }

   public void inicializarMapa(int cx, int cy, Vector3 tz) {
	  cant_x = cx;
	  cant_y = cy;

	  tamano_zona = tz;

	  mapa = new Tile[cant_x * cant_y];

	  if (habitaciones == null) {
		 habitaciones = new List<Habitacion>();
	  }
	  else {
		 habitaciones.Clear();
	  }

	  if (conexiones_horizontales == null) {
		 conexiones_horizontales = new List<Conexion>();
	  }
	  else {
		 conexiones_horizontales.Clear();
	  }

	  if (conexiones_verticales == null) {
		 conexiones_verticales = new List<Conexion>();
	  }
	  else {
		 conexiones_verticales.Clear();
	  }

	  if (intersecciones == null) {
		 intersecciones = new List<Vector2>();
	  }
	  else {
		 intersecciones.Clear();
	  }

	  if (paredes == null) {
		 paredes = new List<Pared>();
	  }
	  else {
		 paredes.Clear();
	  }

	  if (navigation_objeto == null) {
		 navigation_objeto = new GameObject("Navegacion");
		 navigation_objeto.transform.parent = transform;
		 navigation_objeto.transform.localPosition = Vector3.zero;
		 navigation = navigation_objeto.AddComponent<Navigation>();
	  }
	  else {
		 while (navigation_objeto.transform.childCount > 0) {
			DestroyImmediate(navigation_objeto.transform.GetChild(0).gameObject);
		 }
	  }

	  if (mesh_objeto == null) {
		 mesh_objeto = new GameObject("Mesh");
		 mesh_objeto.layer = mascara.value / 32;
		 mesh_objeto.transform.parent = transform;
		 mesh_objeto.transform.eulerAngles = Vector3.zero;
		 mesh_objeto.transform.localPosition = Vector3.zero;
	  }
	  else {
		 while (mesh_objeto.transform.childCount > 0) {
			DestroyImmediate(mesh_objeto.transform.GetChild(0).gameObject);
		 }
		 mesh_objeto.layer = mascara.value / 32;
		 mesh_objeto.transform.parent = transform;
		 mesh_objeto.transform.eulerAngles = Vector3.zero;
		 mesh_objeto.transform.localPosition = Vector3.zero;
	  }

	  if (piso != null) {
		 DestroyImmediate(piso);
	  }

	  if (objetivos == null) {
		 objetivos = new List<Objetivo>();
		 objetivos_mb = new List<ObjetoMB<Objetivo>>();
	  }
	  else {
		 objetivos.Clear();
		 objetivos_mb.Clear();
	  }

	  if (jugadores == null) {
		 jugadores = new List<Jugador>();
		 jugadores_mb = new List<ObjetoMB<Jugador>>();
	  }
	  else {
		 foreach (ObjetoMB<Jugador> jugador in jugadores_mb) {
			DestroyImmediate(jugador.gameObject);
		 }
		 jugadores.Clear();
		 jugadores_mb.Clear();
	  }

	  if (mdp_objeto == null) {
		 mdp_objeto = new GameObject("MDP");
		 mdp_objeto.transform.parent = transform;
		 //arbol_e = new Arbol_Estados(this, jugadores, acciones, objetivos);
		 arbol_e = mdp_objeto.AddComponent<Arbol_Estados>();
		 arbol_e.escenario_base = this;
		 arbol_e.jugadores = jugadores;
		 arbol_e.acciones = acciones;
		 arbol_e.objetivos = objetivos;
		 //resolucion_mdp = new ResolucionMDP(arbol_e);
		 resolucion_mdp = mdp_objeto.AddComponent<ResolucionMDP>();
		 resolucion_mdp.arbol_estados = arbol_e;
	  }

	  ancho = tamano_zona.x * cant_x;
	  largo = tamano_zona.z * cant_y;
	  offsetx = (ancho - tamano_zona.x) / 2;
	  offsety = (largo - tamano_zona.z) / 2;
   }

   // Construccion

   public Habitacion crearHabitacion(int x1, int y1, int x2, int y2, Tile zona) {
	  if (x1 > x2) {
		 int temp = x1;
		 x1 = x2;
		 x2 = temp;
	  }
	  if (y1 > y2) {
		 int temp = y1;
		 y1 = y2;
		 y2 = temp;
	  }

	  bool limite_ancho = (x1 >= 0) && (x2 < cant_x);
	  bool limite_largo = (y1 >= 0) && (y2 < cant_y);

	  if (limite_ancho && limite_largo) {
		 for (int i = x1; i <= x2; i++) {
			for (int j = y1; j <= y2; j++) {
			   mapa[i + j * cant_x] = zona;
			}
		 }
	  }

	  Habitacion habitacion = new Habitacion(x1, y1, x2, y2);
	  habitaciones.Add(habitacion);

	  return habitacion;
   }

   public Conexion crearConexion(int x1, int y1, int x2, int y2, Tile zona, bool dibujar_en_mapa) {
	  if (x1 > x2) {
		 int temp = x1;
		 x1 = x2;
		 x2 = temp;
	  }
	  if (y1 > y2) {
		 int temp = y1;
		 y1 = y2;
		 y2 = temp;
	  }

	  if (dibujar_en_mapa) {
		 bool limite_ancho = (x1 >= 0) && (x2 < cant_x);
		 bool limite_largo = (y1 >= 0) && (y2 < cant_y);

		 if (limite_ancho && limite_largo) {
			for (int i = x1; i <= x2; i++) {
			   for (int j = y1; j <= y2; j++) {
				  mapa[i + j * cant_x] = zona;
			   }
			}
		 }
	  }

	  // ninguna = -1, horizontal = 1, vertical = 0
	  int direccion = -1;
	  if (x1 == x2) {
		 direccion = 0;
	  }
	  else if (y1 == y2) {
		 direccion = 1;
	  }

	  if (((x1 == x2) || (y1 == y2)) && ((x1 <= x2) && (y1 <= y2))) {
		 Conexion conexion = new Conexion(x1, y1, x2, y2);
		 if (direccion == 0) {
			conexiones_verticales.Add(conexion);
		 }
		 else if (direccion == 1) {
			conexiones_horizontales.Add(conexion);
		 }
		 return conexion;
	  }
	  else {
		 return null;
	  }
   }

   public void rellenarRectangulo(int x1, int y1, int x2, int y2, Tile zona) {
	  if (x1 > x2) {
		 int temp = x1;
		 x1 = x2;
		 x2 = temp;
	  }
	  if (y1 > y2) {
		 int temp = y1;
		 y1 = y2;
		 y2 = temp;
	  }

	  bool limite_ancho = (x1 >= 0) && (x2 < cant_x);
	  bool limite_largo = (y1 >= 0) && (y2 < cant_y);

	  if (limite_ancho && limite_largo) {
		 for (int i = x1; i <= x2; i++) {
			for (int j = y1; j <= y2; j++) {
			   mapa[i + j * cant_x] = zona;
			}
		 }
	  }
   }

   public void calcularParedes() {
	  paredes.Clear();
	  for (int i = 0; i < cant_x; i++) {
		 int base_j = -1;
		 for (int j = 0; j < cant_y; j++) {
			if (mapa[i + j * cant_x].transitable) {
			   if (base_j != -1) {
				  if ((j - 1) - base_j > 2) {
					 paredes.Add(new Pared(i, base_j, i, (base_j + (j - 1)) / 2));
					 paredes.Add(new Pared(i, (base_j + (j - 1)) / 2 + 1, i, j - 1));
				  }
				  else {
					 paredes.Add(new Pared(i, base_j, i, j - 1));
				  }
				  base_j = -1;
			   }
			}
			else {
			   if (base_j == -1) {
				  base_j = j;
			   }
			}
		 }

		 if (base_j != -1) {
			if (cant_y - 1 - base_j > 2) {
			   paredes.Add(new Pared(i, base_j, i, (base_j + (cant_y - 1)) / 2));
			   paredes.Add(new Pared(i, (base_j + (cant_y - 1)) / 2 + 1, i, cant_y - 1));
			}
			else {
			   paredes.Add(new Pared(i, base_j, i, cant_y - 1));
			}
		 }
	  }
   }

   // Generacion

   public void borrar() {
	  DestroyImmediate(navigation_objeto);
	  DestroyImmediate(mesh_objeto);
	  DestroyImmediate(mdp_objeto);

	  foreach (ObjetoMB<Jugador> jugador in jugadores_mb) {
		 DestroyImmediate(jugador.gameObject);
	  }
   }

   public void crearMesh() {
	  if (piso == null) {
		 piso = GameObject.CreatePrimitive(PrimitiveType.Cube);
	  }
	  piso.name = "Piso";
	  piso_material.mainTextureScale = Vector2.right * tamano_zona.x * cant_x * 1.25f + Vector2.up * tamano_zona.z * cant_y * 1.25f;
	  piso_material.mainTextureOffset = Vector2.right * (1.0f - (piso_material.mainTextureScale.x % 10) / 10) + Vector2.up * (1.0f - (piso_material.mainTextureScale.y % 10) / 10);
	  piso_material.SetTextureScale("_BumpMap", piso_material.mainTextureScale);
	  piso_material.SetTextureOffset("_BumpMap", piso_material.mainTextureOffset);
	  piso.renderer.sharedMaterial = piso_material;
	  piso.transform.parent = mesh_objeto.transform.parent;
	  piso.transform.localScale = ancho * Vector3.right + largo * Vector3.forward + Vector3.up;
	  piso.layer = mascara / 32;

	  calcularParedes();

	  foreach (Pared pared in paredes) {
		 GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
		 cubo.name = "Pared_" + pared.GetHashCode();
		 cubo.transform.parent = mesh_objeto.transform;
		 cubo.renderer.sharedMaterial = mesh_material;
		 float tamano_y = Random.Range(2f, tamano_zona.y);
		 cubo.transform.localScale = Vector3.right * pared.ancho * tamano_zona.x + Vector3.up * tamano_y + Vector3.forward * pared.largo * tamano_zona.z;
		 cubo.transform.position = Vector3.up * (tamano_y / 2f + 0.5f) + Vector3.right * tamano_zona.x * ((pared.x1 + pared.x2) / 2f - (cant_x - 1) / 2f) + Vector3.forward * tamano_zona.z * ((pared.y1 + pared.y2) / 2f - (cant_y - 1) / 2f);
		 cubo.layer = mascara / 32;
	  }

	  // Necesario para que se actualizen las colisiones
	  mesh_objeto.transform.position = Vector3.up * 0;
	  mesh_objeto.transform.eulerAngles = Vector3.up * 180;
	  mesh_objeto.layer = mascara / 32;
   }

   public void crearObjetivos(int cant_objetivos) {
	  HashSet<int> habitaciones_usadas = new HashSet<int>();
	  for (int i = 0; i < cant_objetivos * 2; i++) {
		 int indice;
		 Habitacion habitacion;
		 do {
			indice = Random.Range(0, habitaciones.Count);
			habitacion = habitaciones[indice];
		 } while (habitaciones_usadas.Contains(indice));
		 habitaciones_usadas.Add(indice);

		 Waypoint waypoint = new GameObject("Waypoint_" + i + "_Objetivo").AddComponent<Waypoint>();
		 Vector3 posicion = posicionRepresentacionAReal(Vector2.right * Random.Range(habitacion.x1 + 1, habitacion.x2 - 1) + Vector2.up * Random.Range(habitacion.y1 + 1, habitacion.y2 - 1), 1f);
		 waypoint.Tag = "Objetivo";
		 waypoint.Position = posicion;
		 waypoint.transform.parent = navigation_objeto.transform;

		 string nombre_objetivo = ((char)(i / 2 + 65)).ToString();
		 if (i % 2 == 1) {
			nombre_objetivo = objetivos[i - 1].nombre.ToLower();
		 }
		 GameObject zona_objetivo = new GameObject("Objetivo_" + nombre_objetivo);
		 zona_objetivo.transform.position = posicion;
		 zona_objetivo.transform.parent = mesh_objeto.transform;
		 zona_objetivo.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

		 TextMesh texto_objetivo = zona_objetivo.AddComponent<TextMesh>();
		 zona_objetivo.AddComponent<MeshRenderer>();

		 texto_objetivo.text = nombre_objetivo;
		 texto_objetivo.fontSize = 48;
		 texto_objetivo.characterSize = 0.5f;
		 texto_objetivo.anchor = TextAnchor.MiddleCenter;
		 texto_objetivo.alignment = TextAlignment.Center;
		 texto_objetivo.font = fuente_objetivos;
		 zona_objetivo.renderer.sharedMaterial = fuente_material;

		 ObjetoMB<Objetivo> objetivo_mb = texto_objetivo.gameObject.AddComponent<ObjetoMB<Objetivo>>();
		 objetivo_mb.objeto = new Objetivo(i, objetivo_mb, nombre_objetivo, posicionRealARepresentacion(posicion), waypoint);
		 if (i % 2 == 1) {
			objetivo_mb.objeto.agregarComplementario(objetivos[i - 1]);
			objetivos[i - 1].agregarComplementario(objetivo_mb.objeto);
		 }

		 objetivos.Add(objetivo_mb.objeto);
	  }
   }

   public void crearNavegacion() {
	  int i = Navigation.Waypoints.Count;
	  Waypoint waypoint;
	  waypoint = null;

	  foreach (Conexion conexion_v in conexiones_verticales) {
		 foreach (Conexion conexion_h in conexiones_horizontales) {
			int x = conexion_v.x1;
			int y = conexion_h.y1;
			if (mapa[x + y * cant_x].transitable) {
			   if ((x >= conexion_h.x1 && x <= conexion_h.x2) && (y >= conexion_v.y1 && y <= conexion_v.y2)) {
				  intersecciones.Add(new Vector2(x, y));
			   }
			}
		 }
	  }

	  foreach (Habitacion habitacion in habitaciones) {
		 List<Conexion> habitacion_horizontal = new List<Conexion>();
		 List<Conexion> habitacion_vertical = new List<Conexion>();
		 habitacion_horizontal.Add(new Conexion(habitacion.x1 - 1, habitacion.y1, habitacion.x2 + 1, habitacion.y1));
		 habitacion_horizontal.Add(new Conexion(habitacion.x1 - 1, habitacion.y2, habitacion.x2 + 1, habitacion.y2));
		 habitacion_vertical.Add(new Conexion(habitacion.x1, habitacion.y1 - 1, habitacion.x1, habitacion.y2 + 1));
		 habitacion_vertical.Add(new Conexion(habitacion.x2, habitacion.y1 - 1, habitacion.x2, habitacion.y2 + 1));

		 foreach (Conexion conexion_v in conexiones_verticales) {
			foreach (Conexion conexion_h in habitacion_horizontal) {
			   int x = conexion_v.x1;
			   int y = conexion_h.y1;
			   if (mapa[x + y * cant_x].transitable) {
				  if ((x >= conexion_h.x1 && x <= conexion_h.x2) && (y >= conexion_v.y1 && y <= conexion_v.y2)) {
					 intersecciones.Add(new Vector2(x, y));
				  }
			   }
			}
		 }

		 foreach (Conexion conexion_v in habitacion_vertical) {
			foreach (Conexion conexion_h in conexiones_horizontales) {
			   int x = conexion_v.x1;
			   int y = conexion_h.y1;
			   if (mapa[x + y * cant_x].transitable) {
				  if ((x >= conexion_h.x1 && x <= conexion_h.x2) && (y >= conexion_v.y1 && y <= conexion_v.y2)) {
					 intersecciones.Add(new Vector2(x, y));
				  }
			   }
			}
		 }

		 Vector3 posicion = posicionRepresentacionAReal(Vector2.right * (habitacion.x1 + habitacion.ancho / 2) + Vector2.up * (habitacion.y1 + habitacion.largo / 2), 1.25f);

		 waypoint = new GameObject("Waypoint_" + i + "_Habitacion").AddComponent<Waypoint>();
		 waypoint.Tag = "Habitacion";
		 waypoint.Position = posicion;
		 waypoint.transform.parent = navigation_objeto.transform;
		 i++;
	  }

	  foreach (Vector2 interseccion in intersecciones) {
		 Vector3 posicion = posicionRepresentacionAReal(interseccion, 1.25f);

		 Waypoint waypoint_mas_cercano = Navigation.GetNearestNode(posicion);
		 if (waypoint_mas_cercano == null || Vector3.Distance(posicion, waypoint_mas_cercano.Position) > (tamano_zona.x + tamano_zona.z) / 3) {
			waypoint = new GameObject("Waypoint_" + i + "_Interseccion").AddComponent<Waypoint>();
			waypoint.Tag = "Conexion";
			waypoint.Position = posicion;
			waypoint.transform.parent = navigation_objeto.transform;
			i++;
		 }
	  }

	  Navigation.AutoScale(mascara, 0.25f, Mathf.Max(tamano_zona.x, tamano_zona.z) * 0.8f, 0.1f);
	  Navigation.AutoConnect(mascara, 0.25f, Mathf.Max(tamano_zona.x, tamano_zona.z) * 0.8f, 0.1f);
	  //Debug.Log(Navigation.Waypoints.Count);
	  optimizarMallaNavegacion();
	  //Debug.Log(Navigation.Waypoints.Count);
   }

   public void crearJugadores(int cant_jugadores, GameObject jugador_prefab, GameObject companero_prefab, Camara_3Persona camara) {
	  HashSet<int> habitaciones_usadas = new HashSet<int>();
	  int indice;
	  Habitacion habitacion;

	  if (cant_jugadores > 0) {
		 indice = Random.Range(0, habitaciones.Count);
		 habitacion = habitaciones[indice];
		 habitaciones_usadas.Add(indice);

		 Vector3 posicion = posicionRepresentacionAReal(Vector2.right * Random.Range(habitacion.x1 + 1, habitacion.x2 - 1) + Vector2.up * Random.Range(habitacion.y1 + 1, habitacion.y2 - 1), 1.25f);
		 GameObject jugador_objeto = UnityEditor.PrefabUtility.InstantiatePrefab(jugador_prefab) as GameObject;
		 jugador_objeto.transform.position = posicion;
		 jugador_objeto.transform.rotation = Random.rotationUniform;

		 jugador_objeto.GetComponent<Control_Directo>().camara_3persona = camara;

		 ObjetoMB<Jugador> jugador_mb = jugador_objeto.AddComponent<ObjetoMB<Jugador>>();
		 jugador_mb.objeto = new Jugador(0, jugador_objeto.name = "Jugador", '@', posicionRealARepresentacion(posicion), Jugador.TControl.DIRECTO);
		 jugadores.Add(jugador_mb.objeto);
		 jugadores_mb.Add(jugador_mb);
	  }

	  for (int i = 1; i < cant_jugadores; i++) {
		 do {
			indice = Random.Range(0, habitaciones.Count);
			habitacion = habitaciones[indice];
		 } while (habitaciones_usadas.Contains(indice));
		 habitaciones_usadas.Add(indice);

		 Vector3 posicion = posicionRepresentacionAReal(Vector2.right * Random.Range(habitacion.x1 + 1, habitacion.x2 - 1) + Vector2.up * Random.Range(habitacion.y1 + 1, habitacion.y2 - 1), 1.25f);
		 GameObject jugador_objeto = UnityEditor.PrefabUtility.InstantiatePrefab(companero_prefab) as GameObject;
		 jugador_objeto.transform.position = posicion;
		 jugador_objeto.transform.rotation = Random.rotationUniform;

		 ObjetoMB<Jugador> jugador_mb = jugador_objeto.AddComponent<ObjetoMB<Jugador>>();
		 jugador_mb.objeto = new Jugador(i, jugador_objeto.name = "Companero_" + i, '$', posicionRealARepresentacion(posicion), Jugador.TControl.IA);
		 jugadores.Add(jugador_mb.objeto);
		 jugadores_mb.Add(jugador_mb);
	  }
   }

   public void crearMDP() {

   }

   public void crearTXT(string nombre_salida) {
	  System.IO.StreamWriter salida = new System.IO.StreamWriter(nombre_salida);

	  salida.Write(' ');
	  salida.Write(' ');
	  for (int i = 0; i < cant_x; i++) {
		 salida.Write(' ');
		 salida.Write(i % 10);
	  }
	  salida.WriteLine();
	  salida.Write(' ');
	  salida.Write(0);

	  for (int j = 0; j < cant_y; j++) {
		 for (int i = 0; i < cant_x; i++) {
			salida.Write(" " + mapa[i + j * cant_x].representacion2D);
		 }
		 salida.WriteLine();
		 salida.Write(' ');
		 salida.Write((j + 1) % 10);
	  }

	  salida.Close();
   }

   // Utilidades

   public void optimizarMallaNavegacion() {
	  List<Waypoint> a_destruir = new List<Waypoint>();
	  foreach (Waypoint waypoint in Navigation.Waypoints) {
		 if (!a_destruir.Contains(waypoint)) {
			int i = 0;
			while (i < waypoint.Connections.Count) {
			   Connection connection = waypoint.Connections[i];
			   Vector3 direccion = connection.To.Position - connection.From.Position;

			   if (Physics.Raycast(connection.From.Position, direccion, direccion.magnitude, mascara)) {
				  waypoint.RemoveConnection(connection);
			   }
			   else if (!a_destruir.Contains(connection.To) && mismasConexiones(waypoint, connection.To)) {
				  if (connection.To.Tag == "Conexion") {
					 a_destruir.Add(connection.To);
				  }
			   }
			   i++;
			}
		 }
	  }

	  while (a_destruir.Count > 0) {
		 Waypoint waypoint = a_destruir[0];
		 a_destruir.RemoveAt(0);
		 DestroyImmediate(waypoint.gameObject);
	  }

	  int j = habitaciones.Count + objetivos.Count;
	  foreach (Waypoint waypoint in Navigation.Waypoints) {
		 if (waypoint.Tag == "Conexion") {
			waypoint.gameObject.name = "Waypoint_" + j + "_Interseccion";
			j++;
		 }
	  }
   }

   public bool mismasConexiones(Waypoint w1, Waypoint w2) {
	  HashSet<Waypoint> conexiones_w1 = new HashSet<Waypoint>();
	  HashSet<Waypoint> conexiones_w2 = new HashSet<Waypoint>();

	  foreach (Connection conexion in w1.Connections) {
		 conexiones_w1.Add(conexion.To);
	  }
	  foreach (Connection conexion in w2.Connections) {
		 conexiones_w2.Add(conexion.To);
	  }

	  //if (conexiones_w1.Count == conexiones_w2.Count) {
	  if (conexiones_w1.Contains(w2)) {
		 conexiones_w1.Remove(w2);
	  }
	  else {
		 return false;
	  }

	  if (conexiones_w2.Contains(w1)) {
		 conexiones_w2.Remove(w1);
	  }
	  else {
		 return false;
	  }

	  //return (conexiones_w1.SetEquals (conexiones_w2));
	  return (conexiones_w1.IsSupersetOf(conexiones_w2));
	  //} else {
	  //	return false;
	  //}
   }

   public Vector2 posicionRealARepresentacion(Vector3 posicion) {
	  return (Vector2.right * (offsetx - posicion.x) / tamano_zona.x + Vector2.up * (offsety - posicion.z) / tamano_zona.z);
   }

   public Vector3 posicionRepresentacionAReal(Vector2 posicion, float altura) {
	  return (Vector3.right * (offsetx - tamano_zona.x * posicion.x) + Vector3.forward * (offsety - tamano_zona.z * posicion.y) + Vector3.up * altura);
   }

   // Serializacion
   public Mapa(SerializationInfo info, StreamingContext ctxt) {

	  mapa = info.GetValue("Mapa", typeof(Tile[])) as Tile[];
	  cant_x = info.GetInt16("Cant_X");
	  cant_y = info.GetInt16("Cant_Y");
	  tamano_zona = (Vector3)info.GetValue("Tamano_Zona", typeof(Vector3));
	  mascara = (LayerMask)info.GetInt16("Mascara");

	  navigation_objeto = info.GetValue("Navigation_Objeto", typeof(GameObject)) as GameObject;
	  navigation = info.GetValue("Navigation", typeof(Navigation)) as Navigation;

	  mdp_objeto = info.GetValue("MDP_Objeto", typeof(GameObject)) as GameObject;
	  resolucion_mdp = info.GetValue("Resolucion_MDP", typeof(ResolucionMDP)) as ResolucionMDP;
	  arbol_e = info.GetValue("Arbol_E", typeof(Arbol_Estados)) as Arbol_Estados;

	  mesh_material = info.GetValue("Mesh_Material", typeof(Material)) as Material;
	  piso_material = info.GetValue("Piso_Material", typeof(Material)) as Material;
	  fuente_material = info.GetValue("Fuente_Material", typeof(Material)) as Material;
	  fuente_objetivos = info.GetValue("Fuente_Objetivos", typeof(Font)) as Font;

	  ancho = (float)info.GetValue("Ancho", typeof(float));
	  largo = (float)info.GetValue("Largo", typeof(float));
	  offsetx = (float)info.GetValue("Offsetx", typeof(float));
	  offsety = (float)info.GetValue("Offsety", typeof(float));

	  mesh_objeto = info.GetValue("Mesh_Objeto", typeof(GameObject)) as GameObject;
	  piso = info.GetValue("Piso", typeof(GameObject)) as GameObject;

	  paredes = info.GetValue("Paredes", typeof(List<Pared>)) as List<Pared>;
	  habitaciones = info.GetValue("Habitaciones", typeof(List<Habitacion>)) as List<Habitacion>;
	  conexiones_horizontales = info.GetValue("Conexiones_Horizontales", typeof(List<Conexion>)) as List<Conexion>;
	  conexiones_verticales = info.GetValue("Conexiones_Verticales", typeof(List<Conexion>)) as List<Conexion>;
	  intersecciones = info.GetValue("Intersecciones", typeof(List<Vector2>)) as List<Vector2>;

	  objetivos = info.GetValue("Objetivos", typeof(List<Objetivo>)) as List<Objetivo>;

	  jugadores = info.GetValue("Jugadores", typeof(List<Jugador>)) as List<Jugador>;

	  acciones = info.GetValue("Acciones", typeof(List<Accion>)) as List<Accion>;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Mapa", mapa);
	  info.AddValue("Cant_X", cant_x);
	  info.AddValue("Cant_Y", cant_y);
	  info.AddValue("Tamano_Zona", tamano_zona);
	  info.AddValue("Mascara", mascara);

	  info.AddValue("Navigation_Objeto", navigation_objeto);
	  info.AddValue("Navigation", navigation);

	  info.AddValue("MDP_Objeto", mdp_objeto);
	  info.AddValue("Resolucion_MDP", resolucion_mdp);
	  info.AddValue("Arbol_E", arbol_e);

	  info.AddValue("Mesh_Material", mesh_material);
	  info.AddValue("Piso_Material", piso_material);
	  info.AddValue("Fuente_Material", fuente_material);
	  info.AddValue("Fuente_Objetivos", fuente_objetivos);

	  info.AddValue("Ancho", ancho);
	  info.AddValue("Largo", largo);
	  info.AddValue("Offsetx", offsetx);
	  info.AddValue("Offsety", offsety);

	  info.AddValue("Mesh_Objeto", mesh_objeto);
	  info.AddValue("Piso", piso);

	  info.AddValue("Paredes", paredes);
	  info.AddValue("Habitaciones", habitaciones);
	  info.AddValue("Conexiones_Horizontales", conexiones_horizontales);
	  info.AddValue("Conexiones_Verticales", conexiones_verticales);
	  info.AddValue("Intersecciones", intersecciones);

	  info.AddValue("Objetivos", objetivos);

	  info.AddValue("Jugadores", jugadores);

	  info.AddValue("Acciones", acciones);
   }
}