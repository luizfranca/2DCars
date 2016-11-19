using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Car : MonoBehaviour {

	public GameObject carSign;
	public GameObject chassi;
	public GameObject wheelHub;
	public GameObject carSignPrefab;
	public GameObject wheelPrefab;
	public Vector3[] vertices;
	private GameObject[] triangles;
	private Genome genome;
	private float score;
	public float time;
	private float timeToDie;
	private Controller controller;


	void Start () {
		controller = Camera.main.GetComponent<Controller> ();
		timeToDie = controller.timeToDie;
		carSign = Instantiate (carSignPrefab);
		carSign.transform.parent = this.gameObject.transform.parent.transform;
		carSign.GetComponent<SmoothFollow> ().target = this.gameObject.transform;
		carSign.GetComponent<TextMesh> ().text = this.gameObject.name;
	}
	
	void Update () {
		if (!controller.GetPause ()) {
			time += Time.deltaTime;
			if (this.gameObject.transform.position.x > score + 0.5f) {
				score = this.gameObject.transform.position.x;
				time = 0;
			}

			if (time >= timeToDie)
				controller.SendMessage ("Died", this.gameObject);
		}

		if (controller.end < this.transform.position.x) {
			controller.ReachedEnd();
		}
	}

	private void GenerateTriangles() {
		triangles = new GameObject[8];
		
		for (int i = 0; i < 8; i++) {
			triangles[i] = new GameObject("Triangle " + (i + 1));
			triangles[i].transform.parent = chassi.transform;
			triangles[i].transform.localPosition = new Vector3(0, 0, 0);
			triangles[i].AddComponent<MeshFilter>();
			triangles[i].AddComponent<MeshRenderer>();
			triangles[i].AddComponent<PolygonCollider2D>();
		}
	}
	
	private void ShapeChassi() {
		for (int i = 0; i < 8; i++) {
			Mesh mesh = new Mesh ();
			mesh.name = "Triangle";
			int index = (i + 2) % 9;
			if (index == 0)
				index ++;
			mesh.vertices = new Vector3[] { vertices [0], vertices [(i + 1) % 9], vertices [index] };
			mesh.triangles = new int[] {0, 1, 2};
			triangles[i].GetComponent<MeshFilter> ().mesh = mesh;

			triangles[i].GetComponent<PolygonCollider2D>().SetPath(0, new Vector2[] {ConvertToVector2(vertices [0]), 
					ConvertToVector2(vertices [(i + 1) % 9]), ConvertToVector2(vertices [index])});
		}
	}

	private Vector2 ConvertToVector2(Vector3 vector) {
		Vector2 v = new Vector2 ();
		v.x = vector.x;
		v.y = vector.y;
		return v;
	}

	private void SetVertices() {
		this.vertices = new Vector3[9];
		
		float[,] v = this.genome.GetAxis ();
		
		for (int i = 0; i < 8; i++) {
			vertices [i + 1].x = v [i, 0];
			vertices [i + 1].y = v [i, 1];
		}
	}

	private void CreateWheels() {
		int numberWheels = this.genome.GetNumberWheels ();
		int[] positionWheels = this.genome.GetPositionWheels ();
		float[] sizeWheels = this.genome.GetSizeWheels ();

		for (int i = 0; i < numberWheels; i++) {
			GameObject aux = (GameObject)Instantiate(wheelPrefab);
			aux.transform.parent = this.wheelHub.transform;
			aux.transform.localPosition = this.vertices[positionWheels[i]];
			aux.transform.Translate(0,0,-1);
			aux.transform.localScale = new Vector3(sizeWheels[i], sizeWheels[i], 0);
			aux.GetComponent<WheelJoint2D>().connectedBody = this.gameObject.GetComponent<Rigidbody2D>();
			aux.GetComponent<WheelJoint2D>().connectedAnchor = ConvertToVector2(this.vertices[positionWheels[i]]);
		}
	}

	public void CreateCar(Genome genome) {
		this.genome = genome;

		Debug.Log (this.genome.ToString () + ", " + this.gameObject.name);

		GenerateTriangles ();

		SetVertices ();

		ShapeChassi ();

		CreateWheels ();
	}

	public float GetScore() {
		return score;
	}

	public Genome GetGenome() {
		return this.genome;
	}

	public void ResetCar() {
		this.score = 0;
		this.time = 0;
		this.gameObject.transform.position = new Vector3 (-2.5f, 0, 0);
	}
}
