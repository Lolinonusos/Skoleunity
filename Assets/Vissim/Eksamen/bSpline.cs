using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class bSpline {
	public int d = 2; // Grad	
	public int n = 0; // Antall kontrollpunkter
	public List<Vector2> controlPoints = new List<Vector2>(); // Kontrollpunkter
	public LineRenderer lineRenderer;
	public List<float> knotVector = new List<float>();

	bSpline(int grad) {
		d = grad;

		int knots = grad + controlPoints.Count + 1;

		for (int i = 0; i < grad + 1; i++) {
			
		}
	}
	
	int findKnotInterval(float x) {
		int buh = n - 1; // Indeks til siste kontrollpunkt
		while (x < knotVector[buh] &&  buh > d) {
			buh--;
		}
		return buh;
	}
	
	Vector3 evaluateBSpline(float x) {
		int buh = findKnotInterval(x);
		List<Vector3> a = new List<Vector3>();
		for (int j = 0; j <= d; j++) {
			a[d - j] = controlPoints[buh - j];
		}
		for (int k = d; k > 0; k--) {
			int j = buh - k;
			for (int i = 0; i < k; i++) {
				j++;
				float w = (x - knotVector[j]) / (knotVector[j + k] - knotVector[j]);
				a[i] = a[i] * (1 - w) + a[i + 1] * w;
			}
		}
		return a[0];
	}

	void drawSpline(int numPointsToDraw, TriangleSurfaceV2 surface) {

		List<Vector3> positions = new List<Vector3>();

		float t = 0;

		for (int i = 0; i < numPointsToDraw; i++) {

			Vector3 pos = evaluateBSpline(t);

			float guh = knotVector[0] - knotVector.ElementAt(knotVector.Count - 1);
			positions.Add(pos);
		}

		lineRenderer.positionCount = numPointsToDraw;
		lineRenderer.SetPositions(positions.ToArray());
	}
}
