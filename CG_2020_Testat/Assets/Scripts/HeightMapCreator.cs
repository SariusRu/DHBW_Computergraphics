using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapCreator : MonoBehaviour {

    Material _material;

    int _totalSize = 3600;
    int _subMeshSize = 100;

    // Use this for initialization
    void Start () {
        _material = GetComponent<MeshRenderer>().material;

        createMesh(_totalSize, _totalSize);

        gameObject.transform.localScale = new Vector3(0.001f,0.001f,0.001f);
        gameObject.transform.Rotate(Vector3.right, 90);

    }

    // Update is called once per frame
    void Update () {
		
	}

    void createSubMesh(int xSubMeshCount, int ySubMeshCount)
    {
        GameObject meshGameObject = new GameObject();
        meshGameObject.transform.SetParent(gameObject.transform);
        meshGameObject.AddComponent<MeshRenderer>().material = _material;
        meshGameObject.name = "SubMesh("+xSubMeshCount+","+ySubMeshCount+")";
        MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[_subMeshSize * _subMeshSize];
        Vector2[] uvs = new Vector2[_subMeshSize * _subMeshSize];
        int[] triangles = new int[6 * ((_subMeshSize - 1) * (_subMeshSize - 1))];

        int xOffset = xSubMeshCount;
        int yOffset = ySubMeshCount;


        int triangleIndex = 0;
        for (int y = 0; y < _subMeshSize; y++)
        {
            for (int x = 0; x < _subMeshSize; x++)
            {
                int index = (y * _subMeshSize) + x;

                vertices[index] = new Vector3(x+(xSubMeshCount*_subMeshSize)- xOffset, -y-(ySubMeshCount * _subMeshSize)+ yOffset, 0);
                //Debug.Log("vertex index: " + index + ":   x=" + (x + (xSubMeshCount * _subMeshSize)) + "    y=" + (-y - (ySubMeshCount * _subMeshSize)));
                uvs[index] = new Vector2(((float)(xSubMeshCount * _subMeshSize+x) / (float)_totalSize), ((float)(ySubMeshCount * _subMeshSize+y) / (float)_totalSize));

                // Skip the last row/col
                if (x != (_subMeshSize - 1) && y != (_subMeshSize - 1))
                {
                    int topLeft = index;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + _subMeshSize;
                    int bottomRight = bottomLeft + 1;

                    triangles[triangleIndex++] = topLeft;
                    triangles[triangleIndex++] = topRight;
                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = topRight;
                    triangles[triangleIndex++] = bottomRight;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void createMesh(int width, int height)
    {
        for(int xSub=0; xSub<width; xSub += _subMeshSize)
        {
            for (int ySub = 0; ySub < height; ySub += _subMeshSize)
            {
                createSubMesh(xSub / _subMeshSize, ySub / _subMeshSize);
            }
        }
        
    }
}
