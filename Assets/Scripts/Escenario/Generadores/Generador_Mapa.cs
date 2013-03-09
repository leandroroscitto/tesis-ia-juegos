using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;
using Random = UnityEngine.Random;

public class Generador_Mapa : MonoBehaviour {
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

#pragma warning disable 0114
	  public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
		 base.GetObjectData(info, ctxt);

		 info.AddValue("Conexiones", conexiones);
	  }
#pragma warning restore 0114
   }
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

   // Atributos
   [NonSerialized]
   public Mapa mapa;
   public LayerMask mascara;

   // Representacion visual
   public Material mesh_material;
   public Material piso_material;

   // Representacion tiles
   public Tile tile_piso, tile_pared;

   // Representacion interna
   public GameObject mapa_objeto;
   public GameObject mesh_objeto;
   public GameObject piso_objeto;

   // Listas
   public List<Habitacion> habitaciones;
   public List<Pared> paredes;
   public List<Conexion> conexiones_horizontales;
   public List<Conexion> conexiones_verticales;
   public List<Vector2> intersecciones;

   // Operaciones
   public void inicializarListas() {
	  if (habitaciones == null) {
		 habitaciones = new List<Habitacion>();
	  }
	  else {
		 habitaciones.Clear();
	  }

	  if (paredes == null) {
		 paredes = new List<Pared>();
	  }
	  else {
		 paredes.Clear();
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
   }

   public void inicializarRepresentacion() {
	  if (mapa_objeto == null) {
		 mapa_objeto = new GameObject("Mapa");
	  }
	  mapa_objeto.transform.parent = GetComponent<Generador_Escenario>().escenario_objeto.transform;

	  if (mesh_objeto == null) {
		 mesh_objeto = new GameObject("Mesh");
	  }
	  mesh_objeto.layer = mascara.value / 32;
	  mesh_objeto.transform.parent = mapa_objeto.transform;
	  mesh_objeto.transform.eulerAngles = Vector3.zero;
	  mesh_objeto.transform.localPosition = Vector3.zero;

	  while (mesh_objeto.transform.childCount > 0) {
		 DestroyImmediate(mesh_objeto.transform.GetChild(0).gameObject);
	  }

	  if (piso_objeto != null) {
		 DestroyImmediate(piso_objeto);
	  }
   }

   public void inicializarTiles() {
	  if (tile_piso == null) {
		 tile_piso = new Tile(Tile.TTile.PISO, true, ".");
	  }
	  else {
		 tile_piso.inicializar(Tile.TTile.PISO, true, ".");
	  }
	  if (tile_pared == null) {
		 tile_pared = new Tile(Tile.TTile.PARED, false, "#");
	  }
	  else {
		 tile_pared.inicializar(Tile.TTile.PARED, false, "#");
	  }
   }

   public void inicializar(int cx, int cy, Vector3 tam_tile, LayerMask masc, Material mesh_m, Material piso_m) {
	  mapa = new Mapa(cx, cy, tam_tile);

	  mascara = masc;
	  mesh_material = mesh_m;
	  piso_material = piso_m;

	  inicializarListas();
	  inicializarRepresentacion();
	  inicializarTiles();
   }

   // Construccion
   public Habitacion crearHabitacion(int x1, int y1, int x2, int y2, Tile zona) {
	  reordenarValorMayorMenor(ref x1, ref x2);
	  reordenarValorMayorMenor(ref y1, ref y2);

	  bool limite_ancho = (x1 >= 0) && (x2 < Mapa.Mapa_Instancia.cant_x);
	  bool limite_largo = (y1 >= 0) && (y2 < Mapa.Mapa_Instancia.cant_y);

	  if (limite_ancho && limite_largo) {
		 for (int i = x1; i <= x2; i++) {
			for (int j = y1; j <= y2; j++) {
			   mapa.tiles[i + j * Mapa.Mapa_Instancia.cant_x] = zona;
			}
		 }
	  }

	  Habitacion habitacion = new Habitacion(x1, y1, x2, y2);
	  habitaciones.Add(habitacion);

	  return habitacion;
   }

   public Conexion crearConexion(int x1, int y1, int x2, int y2, Tile zona, bool dibujar_en_mapa) {
	  reordenarValorMayorMenor(ref x1, ref x2);
	  reordenarValorMayorMenor(ref y1, ref y2);

	  if (dibujar_en_mapa) {
		 bool limite_ancho = (x1 >= 0) && (x2 < Mapa.Mapa_Instancia.cant_x);
		 bool limite_largo = (y1 >= 0) && (y2 < Mapa.Mapa_Instancia.cant_y);

		 if (limite_ancho && limite_largo) {
			for (int i = x1; i <= x2; i++) {
			   for (int j = y1; j <= y2; j++) {
				  mapa.tiles[i + j * Mapa.Mapa_Instancia.cant_x] = zona;
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
	  reordenarValorMayorMenor(ref x1, ref x2);
	  reordenarValorMayorMenor(ref y1, ref y2);

	  bool limite_ancho = (x1 >= 0) && (x2 < Mapa.Mapa_Instancia.cant_x);
	  bool limite_largo = (y1 >= 0) && (y2 < Mapa.Mapa_Instancia.cant_y);

	  if (limite_ancho && limite_largo) {
		 for (int i = x1; i <= x2; i++) {
			for (int j = y1; j <= y2; j++) {
			   mapa.tiles[i + j * Mapa.Mapa_Instancia.cant_x] = zona;
			}
		 }
	  }
   }

   // Generacion
   public void generarParedes() {
	  paredes.Clear();
	  for (int i = 0; i < Mapa.Mapa_Instancia.cant_x; i++) {
		 int base_j = -1;
		 for (int j = 0; j < Mapa.Mapa_Instancia.cant_y; j++) {
			if (mapa.tiles[i + j * Mapa.Mapa_Instancia.cant_x].transitable) {
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
			if (Mapa.Mapa_Instancia.cant_y - 1 - base_j > 2) {
			   paredes.Add(new Pared(i, base_j, i, (base_j + (Mapa.Mapa_Instancia.cant_y - 1)) / 2));
			   paredes.Add(new Pared(i, (base_j + (Mapa.Mapa_Instancia.cant_y - 1)) / 2 + 1, i, Mapa.Mapa_Instancia.cant_y - 1));
			}
			else {
			   paredes.Add(new Pared(i, base_j, i, Mapa.Mapa_Instancia.cant_y - 1));
			}
		 }
	  }
   }

   public void generarMesh() {
	  if (piso_objeto == null) {
		 piso_objeto = GameObject.CreatePrimitive(PrimitiveType.Cube);
	  }
	  piso_objeto.name = "Piso";
	  piso_material.mainTextureScale = Vector2.right * mapa.tamano_tile.x * Mapa.Mapa_Instancia.cant_x * 1.25f + Vector2.up * mapa.tamano_tile.z * Mapa.Mapa_Instancia.cant_y * 1.25f;
	  piso_material.mainTextureOffset = Vector2.right * (1.0f - (piso_material.mainTextureScale.x % 10) / 10) + Vector2.up * (1.0f - (piso_material.mainTextureScale.y % 10) / 10);
	  piso_material.SetTextureScale("_BumpMap", piso_material.mainTextureScale);
	  piso_material.SetTextureOffset("_BumpMap", piso_material.mainTextureOffset);
	  piso_objeto.renderer.sharedMaterial = piso_material;
	  piso_objeto.transform.parent = mesh_objeto.transform.parent;
	  piso_objeto.transform.localScale = mapa.ancho * Vector3.right + mapa.largo * Vector3.forward + Vector3.up;
	  piso_objeto.layer = mascara / 32;

	  generarParedes();

	  foreach (Pared pared in paredes) {
		 GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
		 cubo.name = "Pared_" + pared.GetHashCode();
		 cubo.transform.parent = mesh_objeto.transform;
		 cubo.renderer.sharedMaterial = mesh_material;
		 float tamano_y = Random.Range(2f, mapa.tamano_tile.y);
		 cubo.transform.localScale = Vector3.right * pared.ancho * mapa.tamano_tile.x + Vector3.up * tamano_y + Vector3.forward * pared.largo * mapa.tamano_tile.z;
		 cubo.transform.position = Vector3.up * (tamano_y / 2f + 0.5f) + Vector3.right * mapa.tamano_tile.x * ((pared.x1 + pared.x2) / 2f - (Mapa.Mapa_Instancia.cant_x - 1) / 2f) + Vector3.forward * mapa.tamano_tile.z * ((pared.y1 + pared.y2) / 2f - (Mapa.Mapa_Instancia.cant_y - 1) / 2f);
		 cubo.layer = mascara / 32;
	  }

	  // Necesario para que se actualizen las colisiones
	  mesh_objeto.transform.position = Vector3.up * 0;
	  mesh_objeto.transform.eulerAngles = Vector3.up * 180;
	  mesh_objeto.layer = mascara / 32;
   }

   public void generarTXT(string nombre_salida) {
	  System.IO.StreamWriter salida = new System.IO.StreamWriter(nombre_salida);

	  salida.Write(' ');
	  salida.Write(' ');
	  for (int i = 0; i < Mapa.Mapa_Instancia.cant_x; i++) {
		 salida.Write(' ');
		 salida.Write(i % 10);
	  }
	  salida.WriteLine();
	  salida.Write(' ');
	  salida.Write(0);

	  for (int j = 0; j < Mapa.Mapa_Instancia.cant_y; j++) {
		 for (int i = 0; i < Mapa.Mapa_Instancia.cant_x; i++) {
			salida.Write(" " + mapa.tiles[i + j * Mapa.Mapa_Instancia.cant_x].representacion2D);
		 }
		 salida.WriteLine();
		 salida.Write(' ');
		 salida.Write((j + 1) % 10);
	  }

	  salida.Close();
   }

   public void generar(Generador_Escenario.Parametros_Mapa parametros_mapa) {
	  VisitadorHabitacion vhab = new VisitadorHabitacion(this, parametros_mapa);
	  VisitadorConexion vcon = new VisitadorConexion(this, parametros_mapa.conexiones_extras, vhab);

	  BSPTree arbol = new BSPTree(0, 0, Mapa.Mapa_Instancia.cant_x, Mapa.Mapa_Instancia.cant_y);
	  arbol.dividirArbolRecursivo(parametros_mapa.recursion, parametros_mapa.max_hab_tam, parametros_mapa.max_hab_tam, parametros_mapa.max_prop_h, parametros_mapa.max_prop_v);

	  rellenarRectangulo(0, 0, Mapa.Mapa_Instancia.cant_x - 1, Mapa.Mapa_Instancia.cant_y - 1, tile_pared);

	  arbol.recorrerOrdenPorNivelInverso(vhab);
	  arbol.recorrerOrdenPorNivelInverso(vcon);

	  generarTXT("./mapa.txt");
	  generarMesh();
   }

   public void borrar() {
	  if (mesh_objeto != null) {
		 while (mesh_objeto.transform.childCount > 0) {
			DestroyImmediate(mesh_objeto.transform.GetChild(0).gameObject);
		 }
	  }

	  DestroyImmediate(mesh_objeto);
	  DestroyImmediate(piso_objeto);
	  DestroyImmediate(mapa_objeto);
   }

   // Utilidades
   public void reordenarValorMayorMenor(ref int e1, ref int e2) {
	  if (e1 > e2) {
		 int temp = e1;
		 e1 = e2;
		 e2 = temp;
	  }
   }
}