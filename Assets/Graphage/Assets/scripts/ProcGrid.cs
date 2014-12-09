using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A "terrain" mesh. Or rather, a plane using a random height offset in each vertex.
/// 
/// Note: The “bumpiness” in this mesh is done by assigning a random height to each vertex. 
/// It’s done this way because it makes the code nice and simple, not because it makes a good 
/// looking terrain. If you’re serious about building a terrain mesh, you might try using a 
/// heightmap, or perlin noise, or looking into algorithms such as diamond-square.
/// 
/// http://en.wikipedia.org/wiki/Heightmap
/// http://en.wikipedia.org/wiki/Perlin_noise
/// http://en.wikipedia.org/wiki/Diamond-square_algorithm
/// </summary>
public class ProcGrid : ProcBase
{
	//The width and length of each segment:
	public float m_Width = 1.0f;
	public float m_Length = 1.0f;
	//private List<Vector3> vectors;
	//The maximum height of the mesh:
	public float m_Height = 1.0f;
	
	//The number of segments in each dimension (the plane will be m_SegmentCount * m_SegmentCount in area):
	public int m_SegmentCount = 10;

	public void rebuildMesh(int count)
	{
		m_SegmentCount = count;
		Mesh mesh = BuildMesh();
		
		//Look for a MeshFilter component attached to this GameObject:
		MeshFilter filter = GetComponent<MeshFilter>();
		
		//If the MeshFilter exists, attach the new mesh to it.
		//Assuming the GameObject also has a renderer attached, our new mesh will now be visible in the scene.
		if (filter != null)
		{
			filter.sharedMesh=null;
			filter.sharedMesh = mesh;
		}

	}
	public void Start()
	{
		//vectors = coords;
		Mesh mesh = BuildMesh();
		
		//Look for a MeshFilter component attached to this GameObject:
		MeshFilter filter = GetComponent<MeshFilter>();
		
		//If the MeshFilter exists, attach the new mesh to it.
		//Assuming the GameObject also has a renderer attached, our new mesh will now be visible in the scene.
		if (filter != null)
		{
			filter.sharedMesh = mesh;
		}
		//GetComponent<MeshCollider>().sharedMesh = mesh;
	}
	//Build the mesh:
	public override Mesh BuildMesh()
	{
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();
		
		//Loop through the rows:
		for (int i = 0; i < m_SegmentCount; i++)
		{
			//incremented values for the Z position and V coordinate:
			float z = m_Length * i;
			float v = i%2;
			
			//Loop through the collumns:
			for (int j = 0; j < m_SegmentCount; j++)
			{
				//incremented values for the X position and U coordinate:
				float x = m_Width * j;
				float u = j%2;
				
				//The position offset for this quad, with a random height between zero and m_MaxHeight:
				Vector3 offset = new Vector3(x-(m_SegmentCount-1)/2f, 0, z-(m_SegmentCount-1)/2);

				////Build individual quads:
				//BuildQuad(meshBuilder, offset);
				
				//build quads that share vertices:
				Vector2 uv = new Vector2(u, v);
				bool buildTriangles = i > 0 && j > 0;
				//print ("off"+offset);
				//print (vectors[j*m_SegmentCount+i]);
				BuildQuadForGrid(meshBuilder, offset, uv, buildTriangles, m_SegmentCount);
			}
		}
		
		//create the Unity mesh:
		Mesh mesh = meshBuilder.CreateMesh();
		
		//have the mesh calculate its own normals:
		mesh.RecalculateNormals();
		
		//return the new mesh:
		return mesh;
	}
}
