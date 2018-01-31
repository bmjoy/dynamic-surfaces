﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshController : MonoBehaviour {

	public int resolution;
	public int width;
	public int length;
	public float height;
	public FunctionOption function;
	public bool recalculateNormals = true;
	public Gradient heatmap;

	private Mesh _mesh;
	private Vector3[] _vertices;
	private Vector3[] _normals;
	private Color[] _colors;
	
	void Start () {
		if (_mesh == null) {
			_mesh = new Mesh();
			_mesh.name = "Surface Mesh";
			GetComponent<MeshFilter>().mesh = _mesh;
		}
		CreateGrid();
		ResetColor();
	}

	void Update() {
		ChangeHeight();
	}

	private void CreateGrid () {
		_mesh.Clear();
		_vertices = new Vector3[(resolution + 1) * (resolution + 1)];
		_colors = new Color[_vertices.Length];
		_normals = new Vector3[_vertices.Length];
		Vector2[] uv = new Vector2[_vertices.Length];
		float stepSize = 1f / resolution;
		for (int v = 0, z = 0; z <= resolution; z++) {
			for (int x = 0; x <= resolution; x++, v++) {
				_vertices[v] = new Vector3(x * stepSize - 0.5f, 0f, z * stepSize - 0.5f);
				_normals[v] = Vector3.up;
				uv[v] = new Vector2(x * stepSize, z * stepSize);
			}
		}
		_mesh.vertices = _vertices;
		_mesh.normals = _normals;
		_mesh.uv = uv;

		int[] triangles = new int[resolution * resolution * 6];
		for (int t = 0, v = 0, y = 0; y < resolution; y++, v++) {
			for (int x = 0; x < resolution; x++, v++, t += 6) {
				triangles[t] = v;
				triangles[t + 1] = v + resolution + 1;
				triangles[t + 2] = v + 1;
				triangles[t + 3] = v + 1;
				triangles[t + 4] = v + resolution + 1;
				triangles[t + 5] = v + resolution + 2;
			}
		}
		_mesh.triangles = triangles;

		transform.localScale = new Vector3(width, height, length);
		transform.localPosition = new Vector3(0, 0, 4.65f);
	}

	public void ChangeHeight() {
        float step = 1/(float)resolution;
        for (int v = 0, y = 0; y <= resolution; y++) {
			for (int x = 0; x <= resolution; x++, v++) {
				if(function == 0)
                	_vertices[v].y = MathFunctions.Sine(x * step);
				else
					_vertices[v].y = .25f + MathFunctions.Sine(x*step, y*step);
			}
        }
		_mesh.vertices = _vertices;
		// _mesh.colors = _colors;
		if(recalculateNormals)
			_mesh.RecalculateNormals();
    }

	public IEnumerator UpdateHeatMap() {
		while (true) {
			for (int v = 0, y = 0; y <= resolution; y++) {
				for (int x = 0; x <= resolution; x++, v++) {
					_colors[v] = heatmap.Evaluate(_vertices[v].y + .5f);
				}
			}
			_mesh.colors = _colors;

			yield return null;
		}
	}

	public void ResetColor() {
		for (int v = 0, y = 0; y <= resolution; y++) {
			for (int x = 0; x <= resolution; x++, v++) {
				_colors[v] = Color.green;
			}
		}
		_mesh.colors = _colors;
	}

	public void ResetNormals() {
		Vector3 zero = Vector3.up;
		for (int v = 0, z = 0; z <= resolution; z++) {
			for (int x = 0; x <= resolution; x++, v++) {
				_normals[v] = zero;
			}
		}
		_mesh.normals = _normals;
	}

}