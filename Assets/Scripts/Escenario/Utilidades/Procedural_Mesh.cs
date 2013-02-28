using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Procedural_Mesh {
   // Renderizado
   public Material material;

   // Parametros
   public bool permitir_vertices_duplicados;
   public bool permitir_triangulos_internos;
   public bool generar_collider;
   public float distancia_minima_vertices;

   // Listas
   public List<Vector3> vertices;
   public List<int> triangles;

   // Mesh
   public GameObject padre;
   public MeshFilter mesh_filter;
   public MeshRenderer mesh_render;
   public MeshCollider mesh_collider;
   public Mesh mesh;
   public Mesh mesh_c;

   public Procedural_Mesh(GameObject padre_in, Material material_in) {
	  inicializar(padre_in, material_in);
   }

   public void inicializar(GameObject padre_in, Material material_in) {
	  padre = padre_in;
	  material = material_in;

	  mesh_filter = padre.GetComponent<MeshFilter>();
	  mesh_render = padre.GetComponent<MeshRenderer>();
	  mesh_collider = padre.GetComponent<MeshCollider>();
	  if (mesh_filter == null) {
		 mesh_filter = padre.AddComponent<MeshFilter>();
		 mesh = new Mesh();
		 mesh_filter.sharedMesh = mesh;
	  }
	  else {
		 mesh = mesh_filter.sharedMesh;
		 mesh.Clear();
	  }
	  if (mesh_render == null) {
		 mesh_render = padre.AddComponent<MeshRenderer>();
	  }
	  if (mesh_collider == null) {
		 mesh_collider = padre.AddComponent<MeshCollider>();
	  }

	  mesh_render.sharedMaterial = material;

	  vertices = new List<Vector3>();
	  triangles = new List<int>();
   }

   public void generar() {
	  mesh.vertices = vertices.ToArray();
	  mesh.triangles = triangles.ToArray();
	  //mesh.normals = calcularNormales ();

	  mesh.RecalculateNormals();
	  mesh.Optimize();

	  if (generar_collider) {
		 mesh_collider.sharedMesh = mesh;
		 mesh_collider.smoothSphereCollisions = true;
	  }
   }

   private Vector3[] calcularNormales() {
	  Vector3[] normals = new Vector3[vertices.Count];
	  for (int i = 0; i < triangles.Count / 3; i++) {
		 Vector3 p0 = vertices[triangles[i * 3]];
		 Vector3 p1 = vertices[triangles[i * 3 + 1]];
		 Vector3 p2 = vertices[triangles[i * 3 + 2]];

		 Vector3 normal = Vector3.Cross(p0 - p1, p2 - p1) * -1;
		 normal.Normalize();

		 if (normals[triangles[i * 3]] == default(Vector3)) {
			normals[triangles[i * 3]] = normal;
		 }
		 else {
			normals[triangles[i * 3]] += normal;
			//normals [triangles [i * 3]].Normalize ();
		 }
		 if (normals[triangles[i * 3 + 1]] == default(Vector3)) {
			normals[triangles[i * 3 + 1]] = normal;
		 }
		 else {
			normals[triangles[i * 3 + 1]] += normal;
			//normals [triangles [i * 3 + 1]].Normalize ();
		 }
		 if (normals[triangles[i * 3 + 2]] == default(Vector3)) {
			normals[triangles[i * 3 + 2]] = normal;
		 }
		 else {
			normals[triangles[i * 3 + 2]] += normal;
			//normals [triangles [i * 3 + 2]].Normalize ();
		 }
	  }
	  for (int i = 0; i < normals.Length; i++) {
		 normals[i].Normalize();
	  }

	  return normals;
   }

   public void crearCuboCerrado(Vector3 posicion, Vector3 dimensiones) {
	  crearCara(posicion + Vector3.up * dimensiones.y / 2, Vector3.up, dimensiones.x, dimensiones.z);
	  crearCara(posicion + Vector3.down * dimensiones.y / 2, Vector3.down, dimensiones.x, dimensiones.z);
	  crearCara(posicion + Vector3.right * dimensiones.x / 2, Vector3.right, dimensiones.z, dimensiones.y);
	  crearCara(posicion + Vector3.left * dimensiones.x / 2, Vector3.left, dimensiones.z, dimensiones.y);
	  crearCara(posicion + Vector3.forward * dimensiones.z / 2, Vector3.forward, dimensiones.x, dimensiones.y);
	  crearCara(posicion + Vector3.back * dimensiones.z / 2, Vector3.back, dimensiones.x, dimensiones.y);
   }

   public void crearCara(Vector3 posicion, Vector3 normal, float ancho, float largo) {
	  Quaternion rotacion = Quaternion.LookRotation(normal);
	  rotacion.eulerAngles += new Vector3(90, 0, 0);
	  /*
	  Debug.DrawRay (posicion, rotacion * Vector3.forward, Color.blue, 20);
	  Debug.DrawRay (posicion, rotacion * Vector3.right, Color.red, 20);
	  Debug.DrawRay (posicion, rotacion * Vector3.up, Color.green, 20);
	  */

	  Vector3 p1 = posicion + rotacion * (-Vector3.right * ancho / 2 - Vector3.forward * largo / 2);
	  Vector3 p2 = posicion + rotacion * (Vector3.right * ancho / 2 - Vector3.forward * largo / 2);
	  Vector3 p3 = posicion + rotacion * (-Vector3.right * ancho / 2 + Vector3.forward * largo / 2);
	  Vector3 p4 = posicion + rotacion * (Vector3.right * ancho / 2 + Vector3.forward * largo / 2);

	  p1 = redondearVector3(p1, 4);
	  p2 = redondearVector3(p2, 4);
	  p3 = redondearVector3(p3, 4);
	  p4 = redondearVector3(p4, 4);

	  crearCara(p1, p2, p3, p4);
   }

   public void crearCara(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
	  crearTriangulo(p1, p2, p3);
	  crearTriangulo(p3, p2, p4);
   }

   public void crearTriangulo(Vector3 p1, Vector3 p2, Vector3 p3) {
	  int[] indices = new int[3];

	  indices[0] = crearVertice(p1, permitir_vertices_duplicados, distancia_minima_vertices);
	  indices[1] = crearVertice(p2, permitir_vertices_duplicados, distancia_minima_vertices);
	  indices[2] = crearVertice(p3, permitir_vertices_duplicados, distancia_minima_vertices);

	  int indice_base = 0;
	  if (permitir_triangulos_internos) {
		 triangles.Add(indices[2]);
		 triangles.Add(indices[1]);
		 triangles.Add(indices[0]);
	  }
	  else if (!existeTriangulo(indices[2], indices[1], indices[0], out indice_base)) {
		 triangles.Add(indices[2]);
		 triangles.Add(indices[1]);
		 triangles.Add(indices[0]);
	  }
	  else {
		 triangles.RemoveRange(indice_base, 3);
	  }
   }

   private bool existeTriangulo(int i1, int i2, int i3, out int indice_base) {
	  bool resultado = false;
	  indice_base = -1;
	  for (int i = 0; i < triangles.Count / 3; i++) {
		 int ti1 = triangles[i * 3];
		 int ti2 = triangles[i * 3 + 1];
		 int ti3 = triangles[i * 3 + 2];

		 if (ti1 == i1) {
			if (ti2 == i2) {
			   resultado = (ti3 == i3);
			}
			else if (ti2 == i3) {
			   resultado = (ti3 == i2);
			}
			else {
			   resultado = false;
			}
		 }
		 else if (ti1 == i2) {
			if (ti2 == i1) {
			   resultado = (ti3 == i3);
			}
			else if (ti2 == i3) {
			   resultado = (ti3 == i1);
			}
			else {
			   resultado = false;
			}
		 }
		 else if (ti1 == i3) {
			if (ti2 == i1) {
			   resultado = (ti3 == i2);
			}
			else if (ti2 == i2) {
			   resultado = (ti3 == i1);
			}
			else {
			   resultado = false;
			}
		 }
		 else {
			resultado = false;
		 }

		 if (resultado) {
			indice_base = i * 3;
			return true;
		 }
	  }
	  return false;
   }

   public int crearVertice(Vector3 posicion, bool permitir_duplicados, float tolerancia) {
	  int indice;
	  if (!permitir_duplicados) {
		 float distancia = -1;
		 indice = buscarVerticeCercano(posicion, out distancia);
		 if (indice < 0 || distancia > tolerancia) {
			indice = vertices.Count;
			vertices.Add(posicion);
		 }
	  }
	  else {
		 float distancia = -1;
		 indice = buscarVerticeCercano(posicion, out distancia);
		 if (indice < 0 || distancia > tolerancia) {
			indice = vertices.Count;
			vertices.Add(posicion);
		 }
		 else {
			vertices.Add(vertices[indice]);
			indice = vertices.Count - 1;
		 }
	  }

	  return indice;
   }

   private int buscarVerticeCercano(Vector3 posicion, out float distancia) {
	  int minimo_indice = -1;
	  float minima_distancia = float.MaxValue;
	  for (int i = 0; i < vertices.Count; i++) {
		 float distancia_vertice = Vector3.Distance(posicion, vertices[i]);
		 if (distancia_vertice <= minima_distancia) {
			minimo_indice = i;
			minima_distancia = distancia_vertice;
		 }
	  }
	  distancia = minima_distancia;
	  return minimo_indice;
   }

   private Vector3 redondearVector3(Vector3 v, int decimales) {
	  int potencia = (int)Mathf.Pow(10, decimales);
	  v *= potencia;
	  Vector3 resultado = new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
	  return (resultado / potencia);
   }
}
