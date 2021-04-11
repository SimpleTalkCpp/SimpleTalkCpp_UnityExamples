using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class MyList<T>
{
	public int Count => m_count;
	public int Capacity => m_data.Length;
	public ref T this[int index] => ref m_data[index];

	public void Clear() {
		m_count = 0;
	}

	public ref T Push() {
		Resize(m_count + 1);
		return ref m_data[m_count - 1];
	}

	public void Resize(int n) {
		if (n < 0) n = 0;

		if (n >= m_data.Length) {
			Reserve(n);
		}

		m_count = n;
	}

	public void Reserve(int n) {
		n = NextPow2(n);
		var data = new T[n];

		for (int i = 0; i < m_count; i++)
			data[i] = m_data[i];

		m_data = data;
	}

	public void Copy(MyList<T> src) {
		Clear();
		Append(src);
	}

	public void Append(MyList<T> src) {
		if (src == null) return;
		for (int i = 0; i < src.Count; i++) {
			Push() = src[i];
		}
	}

	static int NextPow2(int v) {
		if (v <= 0) return 0;
		v--;
		v|=v>>1;
		v|=v>>2;
		v|=v>>4;
		v|=v>>8;
		v|=v>>16;
		v++;
		return v;
	}

	int m_count;
	T[] m_data = new T[0];
}


//Half Edge Mesh Structure
[System.Serializable]
public class HEMesh
{
	[System.Serializable]
	public struct Point
	{
		public void Init(int id) {
			m_id = id;
			edgeHead  = -1;
			edgeTail  = -1;
			faceCount = 0;
		}

		int m_id;
		public int id => m_id;

		public int edgeHead;
		public int edgeTail;
		public int faceCount;

		public Vector3 pos;
		public Vector3 normal;
	}
	
	[System.Serializable]
	public struct Edge
	{
		public void Init(int id) {
			m_id = id;

			point0 = -1;
			point1 = -1;

			nextEdge0 = -1;
			nextEdge1 = -1;

			faceEdgeHead = -1;
			faceEdgeTail = -1;
		}

		int m_id;
		public int id => m_id;

		public int point0;
		public int point1;

		public int nextEdge0;
		public int nextEdge1;

		public int faceEdgeHead;
		public int faceEdgeTail;

		public bool IsBoundary() {
			return faceEdgeHead == faceEdgeTail;
		}

		public int OppositePoint(int p0) {
			return p0 == point0 ? point1 : point0;
		}

		public Vector3 Center(HEMesh mesh) {
			var sum = mesh.m_points[point0].pos + mesh.m_points[point1].pos;
			return sum / 2;
		}

		public bool HasPoint(int p0, int p1)
		{
			if (p0 == point0 && p1 == point1) return true;
			if (p1 == point0 && p0 == point1) return true;
			return false;
		}

		public int Next(int pt)
		{
			if (pt == point0) return nextEdge0;
			if (pt == point1) return nextEdge1;
			throw new System.Exception();
		}

		public void SetNext(int pt, int nextEdgeId) {
			if (pt == point0) { nextEdge0 = nextEdgeId; return; }
			if (pt == point1) { nextEdge1 = nextEdgeId; return; }
			throw new System.Exception();
		}

		public void AddFaceEdge(HEMesh mesh, ref FaceEdge faceEdge)
		{
			faceEdge.edge = id;

			if (faceEdgeHead < 0) {
				faceEdgeHead      = faceEdge.id;
				faceEdgeTail      = faceEdge.id;
				faceEdge.adjacent = faceEdge.id;
			} else {
				ref var tail = ref mesh.m_faceEdges[faceEdgeTail];
				tail.adjacent     = faceEdge.id;
				faceEdgeTail      = faceEdge.id;
				faceEdge.adjacent = faceEdgeHead;
			}
		}

		public void AddToPoint(HEMesh mesh, ref Point p0, ref Point p1) {
			point0 = p0.id;
			point1 = p1.id;

			if (p0.edgeHead < 0) {
				p0.edgeHead = id;
			} else {
				mesh.m_edges[p0.edgeTail].SetNext(p0.id, id);
			}

			if (p1.edgeHead < 0) {
				p1.edgeHead = id;
			} else {
				mesh.m_edges[p1.edgeTail].SetNext(p1.id, id);
			}

			p0.edgeTail = id;
			p1.edgeTail = id;

			nextEdge0 = p0.edgeHead;
			nextEdge1 = p1.edgeHead;

			p0.faceCount++;
			p1.faceCount++;
		}
	}

	[System.Serializable]
	public struct FaceEdge
	{
		public void Init(int id) {
			m_id     = id;
			next     = -1;
			adjacent = -1;
			point    = -1;
			edge     = -1;
			face     = -1;
		}

		int m_id;
		public int id => m_id;

		public int next;
		public int adjacent; // FaceEdge shared by the same edge

		public int point;
		public int edge;
		public int face;
	}

	[System.Serializable]
	public struct Face
	{
		public void Init(int id) {
			m_id     = id;
		}

		public Vector3 Center(HEMesh mesh) {
			var sum = Vector3.zero;
			for (int i = 0; i < faceEdgeCount; i++) {
				var fe = mesh.m_faceEdges[faceEdgeStart + i];
				sum += mesh.m_points[fe.point].pos;
			}

			return sum / faceEdgeCount;
		}

		int m_id;
		public int id => m_id;

		public int faceEdgeStart;
		public int faceEdgeCount;
	}

	public void AddPoint(in Vector3 pos)
	{
		ref var pt = ref m_points.Push();
		pt.Init(m_points.Count - 1);
		pt.pos = pos;
	}

	public void AddFace(int[] pointIndices)
	{
		ref var face = ref m_faces.Push();
		face.Init(m_faces.Count - 1);

		face.faceEdgeStart = m_faceEdges.Count;

		var n = pointIndices.Length;

		for (int i = 0; i < n; i++)
		{
			ref var p0 = ref m_points[pointIndices[i]];
			ref var p1 = ref m_points[pointIndices[(i + 1) % n]];

			ref var faceEdge = ref m_faceEdges.Push();
			faceEdge.Init(m_faceEdges.Count - 1);
			faceEdge.face = face.id;
			faceEdge.next = face.faceEdgeStart + (i + 1) % n;
			faceEdge.point = p0.id;

			var edgeId = findEdge(p0.id, p1.id);
			if (edgeId < 0) {
				// new edge

				ref var edge = ref m_edges.Push();
				edge.Init(m_edges.Count - 1);
				edge.AddFaceEdge(this, ref faceEdge);

				edge.AddToPoint(this, ref p0, ref p1);
			} else {
				m_edges[edgeId].AddFaceEdge(this, ref faceEdge);
			}
		}

		face.faceEdgeCount = pointIndices.Length;
	}

	public int findEdge(int p0, int p1)
	{
		var head = m_points[p0].edgeHead;
		if (head < 0) return -1;

		var edgeId = head;
		do {
			if (m_edges[edgeId].HasPoint(p0, p1))
				return edgeId;

			edgeId = m_edges[edgeId].Next(p0);

		} while (edgeId != head);

		return -1;
	}

	public Mesh CreateHullMesh() {
		var mesh = new Mesh();
		mesh.name = "Hull";

		var sb = new StringBuilder();

		var vertices = new Vector3[m_points.Count];
		for (int i = 0; i < m_points.Count; i++) {
			vertices[i] = m_points[i].pos;
		}

		mesh.vertices = vertices;

		var indices = new List<int>();

		for (int i = 0; i < m_faces.Count; i++) {
			ref var f = ref m_faces[i];
			var n = f.faceEdgeCount;
			for (int j = 0; j < n; j++) {
				var p0 = f.faceEdgeStart + j;
				var p1 = f.faceEdgeStart + (j + 1) % n;

				indices.Add(m_faceEdges[p0].point);
				indices.Add(m_faceEdges[p1].point);
			}
		}

		mesh.SetIndices(indices, MeshTopology.Lines, 0);
		
		return mesh;
	}

	public void Subdiv(HEMesh target)
	{
		target.Clear();

		var newFacePointStart = m_points.Count;
		var newEdgePointStart = m_points.Count + m_faces.Count;

		UpdateSubdivPointPos(target);
	
		var newFacePointIndices = new int[4];

		for (int i = 0; i < m_faces.Count; i++) {
			ref var face = ref m_faces[i];

			int s = face.faceEdgeStart;
			int n = face.faceEdgeCount;
			for (int j = 0; j < n; j++) {
				ref var p0 = ref m_faceEdges[s + j].point;
				ref var p1 = ref m_faceEdges[s + (j + 1) % n].point;     // next point
				ref var p2 = ref m_faceEdges[s + (n + j - 1) % n].point; // prev point

				var e01 = findEdge(p0, p1);
				var e02 = findEdge(p0, p2);

				newFacePointIndices[0] = p0;
				newFacePointIndices[1] = newEdgePointStart + e01; // next edge
				newFacePointIndices[2] = newFacePointStart + i;   // face center
				newFacePointIndices[3] = newEdgePointStart + e02; // prev edge

				target.AddFace(newFacePointIndices);
			}
		}
	}

	public void UpdateSubdivPointPos(HEMesh target)
	{
		var newFacePointStart = m_points.Count;
		var newEdgePointStart = m_points.Count + m_faces.Count;
		var newPointCount     = m_points.Count + m_faces.Count + m_edges.Count;

		target.m_points.Resize(newPointCount);

		// point at face center
		{
			for (int i = 0; i < m_faces.Count; i++) {
				ref var pt = ref target.m_points[newFacePointStart + i];
				pt.Init(newFacePointStart + i);
				pt.pos = m_faces[i].Center(this);
			}
		}

		// point at edge center
		{
			for (int i = 0; i < m_edges.Count; i++) {
				ref var edge = ref m_edges[i];

				var newPos = Vector3.zero;
				var div = 0;

				newPos += m_points[edge.point0].pos;
				newPos += m_points[edge.point1].pos;
				div += 2;

				if (!edge.IsBoundary()) {
					// adjacent face center
					var fe = edge.faceEdgeHead;
					if (fe >= 0) {
						do {
							ref var faceEdge = ref m_faceEdges[fe];

							newPos += target.m_points[newFacePointStart + faceEdge.face].pos;
							div++;

							fe = faceEdge.adjacent;
						} while (fe != edge.faceEdgeHead);
					}
				}

				ref var pt = ref target.m_points[newEdgePointStart + i];
				pt.Init(newEdgePointStart + i);
				pt.pos = newPos / div;
			}
		}

		// point
		{
			for (int i = 0; i < m_points.Count; i++) {
				ref var srcPt = ref m_points[i];

				float n = 0;

				var		sumEdgePoint = Vector3.zero;
				float	sumEdgeCount = 0;

				var		sumBoundaryEdgePoint = Vector3.zero;
				float	sumBoundaryEdgeCount = 0;

				var		sumFacePoint = Vector3.zero;
				float	sumFaceCount = 0;

				var ei = srcPt.edgeHead;
				if (ei >= 0) {
					do {
						ref var edge = ref m_edges[ei];

						var edgeCenter = (m_points[edge.point0].pos + m_points[edge.point1].pos) / 2;

						if (edge.IsBoundary()) {
							sumBoundaryEdgePoint += edgeCenter;
							sumBoundaryEdgeCount++;
						}

						sumEdgePoint += edgeCenter;
						sumEdgeCount++;
						n++;

						// adjacent face center
						var fe = edge.faceEdgeHead;
						if (fe >= 0) {
							do {
								ref var faceEdge = ref m_faceEdges[fe];

								if (faceEdge.point == srcPt.id) {
									sumFacePoint += target.m_points[newFacePointStart + faceEdge.face].pos;
									sumFaceCount++;
									break;
								}

								fe = faceEdge.adjacent;
							} while (fe != edge.faceEdgeHead);
						}

						ei = edge.Next(srcPt.id);
					} while (ei != srcPt.edgeHead);
				}

				ref var pt = ref target.m_points[i];
				pt.Init(i);

				if (sumBoundaryEdgeCount > 0) {
					pt.pos = (srcPt.pos + sumBoundaryEdgePoint) / (sumBoundaryEdgeCount + 1);

				} else {
					float n2 = n * n;

					pt.pos = (
						srcPt.pos * ((n - 2) / n)
						+ sumEdgePoint / sumEdgeCount / n2
						+ sumFacePoint / sumFaceCount / n2
					);
				}
			}
		}
	}

	public void Clear() {
		m_points.Clear();
		m_edges.Clear();
		m_faceEdges.Clear();
		m_faces.Clear();
	}

	MyList<Point>		m_points    = new MyList<Point>();
	MyList<Edge>		m_edges     = new MyList<Edge>();
	MyList<FaceEdge>	m_faceEdges = new MyList<FaceEdge>();
	MyList<Face>		m_faces     = new MyList<Face>();
}


public class SubdivSurface : MonoBehaviour
{
	HEMesh mesh = new HEMesh();	
	HEMesh[] subdiv;

	public Mesh hullMesh;

	private void Start()
	{
		Application.targetFrameRate = 60;

		//     4-------5
		//    /|      /|
		//   0-------1 |
		//   | 6-----|-7
		//   |/      |/
		//   2-------3
	
		mesh.AddPoint(new Vector3(0,0,0));
		mesh.AddPoint(new Vector3(0,1,0));
		mesh.AddPoint(new Vector3(1,0,0));
		mesh.AddPoint(new Vector3(1,1,0));

		mesh.AddPoint(new Vector3(0,0,1));
		mesh.AddPoint(new Vector3(0,1,1));
		mesh.AddPoint(new Vector3(1,0,1));
		mesh.AddPoint(new Vector3(1,1,1));

		mesh.AddFace(new int[] { 0, 1, 3, 2 });
		mesh.AddFace(new int[] { 1, 5, 7, 3 });
		mesh.AddFace(new int[] { 0, 2, 6, 4 });
		mesh.AddFace(new int[] { 4, 5, 1, 0 });
		mesh.AddFace(new int[] { 7, 6, 2, 3 });
		//		mesh.AddFace(new int[]{5,4,6,7});

		var mf = gameObject.GetComponent<MeshFilter>();
		if (mf) {
			mf.sharedMesh = mesh.CreateHullMesh();
		}

		{
			subdiv = new HEMesh[1];
			var srcMesh = mesh;
			for (int i = 0; i < subdiv.Length; i++) {
				subdiv[i] = new HEMesh();
				srcMesh.Subdiv(subdiv[i]);
				srcMesh = subdiv[i];
			}

			var subObj = new GameObject("subdiv");
			subObj.transform.SetParent(transform);
			subObj.AddComponent<MeshRenderer>();
			mf = subObj.AddComponent<MeshFilter>();
			mf.mesh = srcMesh.CreateHullMesh();
		}
	}
}

