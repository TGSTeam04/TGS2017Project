using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Wire : MonoBehaviour { 
	Mesh mesh;

	// Use this for initialization
	void Start()
	{
		mesh = GetComponent<MeshFilter>().mesh;

		var triangleList = mesh.GetIndices(0);
		var triangleNum = triangleList.Length / 3;
		var lineList = new int[triangleNum * 6];
		for (var n = 0; n < triangleNum; ++n)
		{
			var t = n * 3;
			var l = n * 6;
			lineList[l + 0] = triangleList[t + 0];
			lineList[l + 1] = triangleList[t + 1];

			lineList[l + 2] = triangleList[t + 1];
			lineList[l + 3] = triangleList[t + 2];

			lineList[l + 4] = triangleList[t + 2];
			lineList[l + 5] = triangleList[t + 0];
		}
		mesh.SetIndices(lineList, MeshTopology.Lines, 0);

	}
}
