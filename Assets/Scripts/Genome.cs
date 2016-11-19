using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Genome {

	private bool[] wheelNumber;
	private bool[,] axis;
	private bool[,] wheelSize;
	private bool[,] wheelPosition;

	public Genome () {
		this.wheelNumber = new bool[2];
		this.axis = new bool[16, 3];
		this.wheelSize = new bool[4, 3];
		this.wheelPosition = new bool[4, 3];
	}

	public int GetNumberWheels() {
		int aux = 0;

		int factor = 1;
		for (int i = 1; i >= 0; i--) {
			aux += factor * Convert.ToInt32(this.wheelNumber[i]);
			factor *= 2;
		}

		return aux + 1;
	}

	public float[,] GetAxis() {
		float[,] aux = new float[9, 2];

		for (int i = 0; i < 16; i++) {
			int factor = 1;
			int x = i / 2;
			int y = i % 2;

			for (int j = 2; j >= 0; j--) {
				aux[x, y] += Convert.ToInt32(this.axis[i, j]) * factor;
				factor *= 2;
			}
			aux[x, y] *= 0.25f;
			/*
			if(y == 0) { // x axis
				if (x < 4)
					aux[x, y] *= -1;

				if (x == 0 || x == 3 || x == 4 || x == 7)
					aux[x, y] %= 0.75f;
				else if (x == 5 || x == 6)
					aux[x, y] = (aux[x, y] % 1.5f) + 0.75f;
				else if (x == 2 || x == 1)
					aux[x, y] = (aux[x, y] % 1.5f) - 0.75f;
			} else { // y axis
				if (x < 2 || x > 5) 
					aux[x, y] *= -1;

				if (x == 1 || x == 6 || x == 2 || x == 5)
					aux[x, y] %= 0.75f;
				else if (x == 3 || x == 4)
					aux[x, y] = (aux[x, y] % 1.5f) + 0.75f;
				else if (x == 0 || x == 7)
					aux[x, y] = (aux[x, y] % 1.5f) - 0.75f;
			}
			*/
		}

		return aux;
	}

	public float[] GetSizeWheels() {
		float[] aux = new float[4];

		int numberWheels = this.GetNumberWheels ();

		for (int i = 0; i < 4; i++) {
			int factor = 1;

			if (i > numberWheels - 1)
				break;

			for (int j = 2; j >= 0; j--) {
				aux[i] += factor * Convert.ToInt32(this.wheelSize[i, j]);
				factor *= 2;
			}
			aux[i] = 0.25f + 0.25f * aux[i];
		}
		return aux;
	}

	public int[] GetPositionWheels() {
		int[] aux = new int[4];

		for (int i = 0; i < 4; i++) {
			int factor = 1;

			for (int j = 2; j >= 0; j--) {
				aux[i] += factor * Convert.ToInt32(this.wheelPosition[i, j]);
				factor *= 2;
			}
			aux[i]++;
		}
		return aux;
	}

	public void SetGenome(bool[] genome) {
		int skip = 2;

		this.wheelNumber = Slice (genome, 0, 2);

		for (int i = 0; i < 16; i++) {
			for (int j = 0; j < 3; j++) {
				this.axis[i, j] = genome[skip];
				skip++;
			}
		}

		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 3; j++) {
				this.wheelSize[i, j] = genome[skip];
				skip++;
			}
		}

		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 3; j++) {
				this.wheelPosition[i, j] = genome[skip];
				skip++;
			}
		}
	}

	private bool[] Slice(bool[] arr, int begin, int end) {
		bool[] aux = new bool[end - begin];
		int e = 0;
		for (int i = begin; i < end; i++) {
			aux[e] = arr[i];
			e++;
		}

		return aux;
	}

	public bool[] GetGenome() {
		bool[] genome;
		List<bool> aux = new List<bool> ();

		aux.AddRange (this.wheelNumber);

		foreach (bool item in this.axis) {
			aux.Add(item);
		}

		foreach (bool item in this.wheelSize) {
			aux.Add(item);
		}

		foreach (bool item in this.wheelPosition) {
			aux.Add(item);
		}

		genome = aux.ToArray ();
		return genome;
	}

	public override string ToString() {
		string genome = "";

		bool[] aux = this.GetGenome ();

		foreach (bool item in aux) {
			if (item)
				genome += '1';
			else
				genome += '0';
		}

		return genome;
	}

	public int GetSize() {
		return this.GetGenome ().Length;
	}
}
