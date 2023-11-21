using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bSpline {
	List<int> t = new List<int>(); // Skjotvektor
	int d = 3; // Grad	
	int n = 0; // Antall kontrollpunkter
	List<Vector3> c = new List<Vector3>(); // Kontrollpunkter

	bSpline(int grad) {
		d = grad;
	}
	
	int findKnotInterval(float x) {
		int buh = n - 1; // Indeks til siste kontrollpunkt
		while (x < t[buh] &&  buh > d) {
			buh--;
		}
		return buh;
	}
	
	Vector3 evaluateBSpline(float x) {
		int buh = findKnotInterval(x);
		List<Vector3> a = new List<Vector3>();
		for (int j = 0; j <= d; j++) {
			a[d - j] = c[buh - j];
		}
		for (int k = d; k > 0; k--) {
			int j = buh - k;
			for (int i = 0; i < k; i++) {
				j++;
				float w = (x - t[j]) / (t[j + k] - t[j]);
				a[i] = a[i] * (1 - w) + a[i + 1] * w;
			}
		}
		return a[0];
	}
}
