#define MY_BOIDS_ENABLE_CELLS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBoids : MonoBehaviour
{
	List<MySheep> sheepList = new List<MySheep>();

	public Vector3 containerSize = new Vector3(110, 1, 110);

	[Header("Debug")]
	public bool gizmoDrawSheeps = true;
	public bool gizmoDrawCells = true;

	[Header("Spawn Parameters")]
    public MySheep sheepPrefab;
	public int sheepCount = 10;
	public float spawnRadius = 40;

	[Header("Sheep Parameters")]
	public float maxSpeed = 5;
	public float randomSpeed = 1;
    public float randomSteering = 45;
	public float changeRate = 0.1f;

	[Header("Boids Parameters")]
	public float separationRadius = 3;
	public float separation = 1;

	public float alignmentRadius = 10;
	public float alignmentViewAngle = 120;
	float aligmentViewAngleCos;

	public float alignment = 1;

	public float cohesionRadius = 20;
	public float cohesion = 1;

	[Header("Performance")]
	public int alternativeUpdate = 4;
	int alternativeUpdateIndex;
	public float minCellSize = 1;
	public bool updateUseCells = true;


#if MY_BOIDS_ENABLE_CELLS
	class Cell {
		public List<MySheep> sheepList;
		int _x, _y;

		public int x => _x;
		public int y => _y;

		public Cell(int x, int y) {
			sheepList = new List<MySheep>();
			_x = x;
			_y = y;
		}
	}

	Cell[] cellList;
	float cellSize;
	int cellCountX;
	int cellCountY;

	List<MySheep> tmpSheepList = new List<MySheep>();

	Cell cell(int x, int y) {
		if (x < 0 || x >= cellCountX) return null;
		if (y < 0 || y >= cellCountY) return null;
		return cellList[y * cellCountX + x];
	}

	Cell cellByPos(in Vector3 pos) {
		var offset = containerSize / 2;
		var p = (pos + offset) / cellSize;
		return cell(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.z));
	}

	void getSheepsNearby(ref List<MySheep> outList, Vector3 pos) {
		outList.Clear();

		var c = cellByPos(pos);
		if (c == null) return;
		
		{ var t = cell(c.x - 1, c.y - 1); if (t != null) outList.AddRange(t.sheepList); }
		{ var t = cell(c.x    , c.y - 1); if (t != null) outList.AddRange(t.sheepList); }
		{ var t = cell(c.x + 1, c.y - 1); if (t != null) outList.AddRange(t.sheepList); }

		{ var t = cell(c.x - 1, c.y    ); if (t != null) outList.AddRange(t.sheepList); }
		{ var t = cell(c.x    , c.y    ); if (t != null) outList.AddRange(t.sheepList); }
		{ var t = cell(c.x + 1, c.y    ); if (t != null) outList.AddRange(t.sheepList); }

		{ var t = cell(c.x - 1, c.y + 1); if (t != null) outList.AddRange(t.sheepList); }
		{ var t = cell(c.x    , c.y + 1); if (t != null) outList.AddRange(t.sheepList); }
		{ var t = cell(c.x + 1, c.y + 1); if (t != null) outList.AddRange(t.sheepList); }
	}
#endif

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (gizmoDrawSheeps) {
			foreach (var s in sheepList) {
				var pos = s.transform.position;
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(pos, separationRadius);

				Gizmos.color = Color.blue;
				var rotLeft  = Quaternion.Euler(0, -alignmentViewAngle / 2, 0);
				var rotRight = Quaternion.Euler(0,  alignmentViewAngle / 2, 0);

				Gizmos.DrawRay(pos, rotLeft  * s.transform.forward * alignmentRadius);
				Gizmos.DrawRay(pos, rotRight * s.transform.forward * alignmentRadius);
				// Gizmos.DrawWireSphere(pos, alignmentRadius);

				Gizmos.color = new Color(0,1,0, 0.5f);
				Gizmos.DrawWireSphere(pos, cohesionRadius);
			}
		}

		#if MY_BOIDS_ENABLE_CELLS
			if (gizmoDrawCells && cellList != null) {
				var offset = containerSize / 2;
				foreach (var c in cellList) {
					Gizmos.color = Color.white;
					var size = Vector3.one * cellSize;
					var pos = new Vector3(c.x, 0, c.y) * cellSize - offset + size/2;
					Gizmos.DrawWireCube(pos, size);
					UnityEditor.Handles.Label(pos, $"{c.sheepList.Count}");
				}
			}
		#endif
	}
#endif

	private void Start()
	{
		Application.targetFrameRate = 60;

#if MY_BOIDS_ENABLE_CELLS
		cellSize = Mathf.Max(minCellSize, separationRadius, alignmentRadius, cohesionRadius);

		{
			var div = containerSize / cellSize;
			cellCountX = Mathf.CeilToInt(div.x);
			cellCountY = Mathf.CeilToInt(div.z);
			cellList = new Cell[cellCountX * cellCountY];

			for (int y = 0; y < cellCountY; y++) {
				for (int x = 0; x < cellCountX; x++) {
					cellList[y * cellCountX + x] = new Cell(x, y);
				}
			}
		}
#endif
		if (!sheepPrefab) {
			Debug.LogError("Missing SheepPrefab");
			return;
		}

		for (int i = 0; i < sheepCount; i++) {
			var r = Random.insideUnitCircle * spawnRadius;
			var pos = new Vector3(r.x, 0, r.y);
			var rot = Quaternion.Euler(0, Random.Range(0, 360), 0);

			var o = Instantiate(sheepPrefab.gameObject, pos, rot, transform);
			o.name = "Sheep" + i;
			var s = o.GetComponent<MySheep>();
			sheepList.Add(s);
		}
	}

	private void Update()
	{
		aligmentViewAngleCos = Mathf.Cos(Mathf.Deg2Rad * alignmentViewAngle * 0.5f);

#if MY_BOIDS_ENABLE_CELLS
		foreach (var c in cellList) {
			c.sheepList.Clear();
		}
#endif

		foreach (var s in sheepList) {
			s.lastPosition = s.transform.localPosition;
			s.lastVelocity = s.transform.forward * s.speed;

#if MY_BOIDS_ENABLE_CELLS
			var c = cellByPos(s.lastPosition);
			if (c != null) {
				c.sheepList.Add(s);
			}
#endif
		}

		alternativeUpdateIndex = (alternativeUpdateIndex + 1) % alternativeUpdate;

		for (int i = alternativeUpdateIndex; i < sheepList.Count; i += alternativeUpdate) {
			var s = sheepList[i];

			var nearbySheeps = sheepList;

#if MY_BOIDS_ENABLE_CELLS
			if (updateUseCells) {
				getSheepsNearby(ref tmpSheepList, s.lastPosition);
				nearbySheeps = tmpSheepList;
			}
#endif
			SheepThink(s, Time.deltaTime, nearbySheeps);
		}

	}

	void SheepThink(MySheep sheep, float deltaTime, List<MySheep> nearbySheeps) {

		var separationPos = Vector3.zero;
		int separationCount = 0;

		var alignmentVel = Vector3.zero;
		int alignmentCount = 0;

		var cohesionPos = Vector3.zero;
		int cohesionCount = 0;

		foreach (var s in nearbySheeps) {
			if (s == sheep) continue;

			var d = s.lastPosition - sheep.lastPosition;
			var dis = d.magnitude;
			d /= dis; // normalized

			if (dis < separationRadius) {
				separationPos += s.lastPosition;
				separationCount++;
			}

			if (dis < alignmentRadius) {
				if (Vector3.Dot(d, sheep.transform.forward) > aligmentViewAngleCos) {
					alignmentVel += s.lastVelocity;
					alignmentCount++;
				}
			}

			if (dis < cohesionRadius) {
				cohesionPos += s.lastPosition;
				cohesionCount++;
			}
		}

		var newVel = Vector3.zero;
	
		if (separationCount > 0) {
			separationPos /= separationCount;
			newVel -= (separationPos - sheep.lastPosition).normalized * separation;
		}

		if (alignmentCount > 0) {
			alignmentVel /= alignmentCount;
			newVel += alignmentVel * alignment;
		}

		if (cohesionCount > 0) {
			cohesionPos /= cohesionCount;
			newVel += (cohesionPos - sheep.lastPosition).normalized * cohesion;
		}

		var rate = changeRate * deltaTime;
		newVel = Vector3.Lerp(sheep.lastVelocity, newVel, rate);

		newVel = Quaternion.Euler(0, Random.Range(-randomSteering, randomSteering) * rate, 0) * newVel;

		var newSpeed = newVel.magnitude;
		if (newSpeed > 0.01f) {
			sheep.transform.localRotation = Quaternion.LookRotation(newVel);
		}

		newSpeed += Random.Range(-randomSpeed, randomSpeed);

		newSpeed = Mathf.Lerp(sheep.speed, newSpeed, rate);
		sheep.speed = Mathf.Clamp(newSpeed, 0, maxSpeed);
	}
}
