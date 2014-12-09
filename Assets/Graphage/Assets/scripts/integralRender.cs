using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
public class integralRender : MonoBehaviour {

	// Use this for initialization
	private bool active=false;
	private MeshBuilder meshBuilder;
	private bool updateGraph=false;
	private Mesh mesh1;
	public Color positiveColor=new Color(1,1,1);
	public Color negativeColor=new Color(0,0,0);
	public Color zeroColor=new Color(.5f,.5f,.5f);
	private BackgroundWorker bw = new BackgroundWorker();
	private Vector3[] vectors;
	private Vector2[] uvs;
	private int[] triangles;
	private Vector3 pos1;
	private Vector3 pos2;
	private float lowerX;
	private float upperX;
	private float lowerY;
	private float upperY;
	private int resolution;
	void Start () {
		bw.DoWork += new DoWorkEventHandler (bw_DoWork);
		bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler (bw_RunWorkerCompleted);
	}
	//render the integral
	public void activate(bool val)
	{
		renderer.enabled=val;
	}
	//dont render the integral
	private void checkNormalDirection(int index0,int index1, int index2, int index3)
	{
		if(meshBuilder.Vertices[index0].y+meshBuilder.Vertices[index1].y+meshBuilder.Vertices[index2].y+meshBuilder.Vertices[index3].y<0)
		{
			meshBuilder.AddTriangle(index0, index1, index2);
			meshBuilder.AddTriangle(index2, index1, index3);
		}
		else{
			meshBuilder.AddTriangle(index0, index2, index1);
			meshBuilder.AddTriangle(index2, index3, index1);
		}
	}
	//calculate the space for the integral to be rendered
	public void renderIntegral(Mesh mesh,Vector3 pos1, Vector3 pos2,float lowerX, float upperX, float lowerY,float upperY,int resolution)
	{
		if(!active)
		{
			active=true;
			vectors = mesh.vertices;
			uvs = mesh.uv;
			triangles = mesh.triangles;
			this.pos1 = pos1;
			this.pos2 = pos2;
			this.lowerX = lowerX;
			this.lowerY = lowerY;
			this.upperX = upperX;
			this.upperY = upperY;
			this.resolution = resolution;
		
		bw.RunWorkerAsync ();
		}
	}
	private void calculateMesh()
	{
		float dividablePartX=(upperX-lowerX)/(resolution-1);//.2
		int x1 = Mathf.FloorToInt((pos1.x-lowerX) / dividablePartX);
		int x2 = Mathf.FloorToInt((pos2.x-lowerX) / dividablePartX);
		float dividablePartY=(upperY-lowerY)/(resolution-1);//.2
		int y1 = Mathf.FloorToInt((pos1.z-lowerY) / dividablePartY);
		int y2 = Mathf.FloorToInt((pos2.z-lowerY) / dividablePartY);
		meshBuilder=null;
		meshBuilder=new MeshBuilder();
		if(y2<y1)
		{
			int temp=y1;
			y1=y2;
			y2=temp;
		}
		if(x2<x1)
		{
			int temp=x1;
			x1=x2;
			x2=temp;
		}
		for(int i=y1;i<=y2;i++)
		{
			for(int j=x1;j<=x2;j++)
			{
				if((i==y1||i==y2)||(j==x1||j==x2))
				{

					Vector3 vec=vectors[i*resolution+j];
					meshBuilder.Vertices.Add(vec);
					meshBuilder.UVs.Add(uvs[i*resolution+j]);
					meshBuilder.Normals.Add(uvs[i*resolution+j]);
					if(vec.y>0)
					{
						meshBuilder.Colors.Add (positiveColor);
						meshBuilder.Colors.Add (zeroColor);
						vec.y=0.01f;
					} else
					{
						meshBuilder.Colors.Add (negativeColor);
						meshBuilder.Colors.Add (zeroColor);
						vec.y=-0.01f;
					}
					//vec.y=0f;
					meshBuilder.Vertices.Add(vec);
					meshBuilder.UVs.Add(uvs[i*resolution+j]);
					meshBuilder.Normals.Add(uvs[i*resolution+j]);
				

					int baseIndex = meshBuilder.Vertices.Count - 1;
					
					int index0 = baseIndex;
					int index1 = baseIndex - 1;
					int index2;
					int index3;

					if(j!=x1&&i==y1)
					{

						index2=baseIndex-2;
						index3=baseIndex-3;
						//print (meshBuilder.Vertices[index3].y);
					
						checkNormalDirection(index0,index1,index2,index3);
					} else if(i==y2)
					{

						if(j!=x1)
						{
							index0=baseIndex;
							index1=baseIndex-2;
							index2=baseIndex-(((y2-y1))*4+((x2-x1)*2-2));
							index3=index2-2;
							meshBuilder.AddTriangle(index0, index2, index1);
							meshBuilder.AddTriangle(index2, index3, index1);
							index0=baseIndex;
							index1=baseIndex-1;
						}
						if(j==x1)
						{
							index2=baseIndex-4;
							index3=baseIndex-5;
							if(meshBuilder.Vertices[index0].y+meshBuilder.Vertices[index1].y+meshBuilder.Vertices[index2].y+meshBuilder.Vertices[index3].y<0)
							{
								meshBuilder.AddTriangle(index0, index2, index1);
								meshBuilder.AddTriangle(index2, index3, index1);
								
							}
							else{
								meshBuilder.AddTriangle(index0, index1, index2);
								meshBuilder.AddTriangle(index2, index1, index3);
							}
						} else if(j==x2)
						{
							index2=baseIndex-(x2-x1)*2-2;
							index3=index2-1;
							checkNormalDirection(index0,index1,index2,index3);
							index2=baseIndex-2;
							index3=baseIndex-3;
							if(meshBuilder.Vertices[index0].y+meshBuilder.Vertices[index1].y+meshBuilder.Vertices[index2].y+meshBuilder.Vertices[index3].y<0)
							{
								meshBuilder.AddTriangle(index0, index2, index1);
								meshBuilder.AddTriangle(index2, index3, index1);
								
							}
							else{
								meshBuilder.AddTriangle(index0, index1, index2);
								meshBuilder.AddTriangle(index2, index1, index3);
							}

						}
						else
						{
							index2=baseIndex-2;
							index3=baseIndex-3;
							if(meshBuilder.Vertices[index0].y+meshBuilder.Vertices[index1].y+meshBuilder.Vertices[index2].y+meshBuilder.Vertices[index3].y<0)
							{
								meshBuilder.AddTriangle(index0, index2, index1);
								meshBuilder.AddTriangle(index2, index3, index1);

							}
							else{
								meshBuilder.AddTriangle(index0, index1, index2);
								meshBuilder.AddTriangle(index2, index1, index3);
							}
						}

					} else if(i==y1+1)
					{
						if(j==x1)
						{
							index2=baseIndex-(x2-x1)*2-2;
							index3=index2-1;
							if(meshBuilder.Vertices[index0].y+meshBuilder.Vertices[index1].y+meshBuilder.Vertices[index2].y+meshBuilder.Vertices[index3].y<0)
							{
								meshBuilder.AddTriangle(index0, index2, index1);
								meshBuilder.AddTriangle(index2, index3, index1);
								
							}
							else{
								meshBuilder.AddTriangle(index0, index1, index2);
								meshBuilder.AddTriangle(index2, index1, index3);
							}
						} else if(j==x2)
						{
							index2=baseIndex-4;
							index3=index2-1;
							checkNormalDirection(index0,index1,index2,index3);
						}
					} else if(i!=y1)
					{
						index2=baseIndex-4;
						index3=baseIndex-5;
						if(meshBuilder.Vertices[index0].y+meshBuilder.Vertices[index1].y+meshBuilder.Vertices[index2].y+meshBuilder.Vertices[index3].y<0)
						{
							if(j==x2)
							{
								meshBuilder.AddTriangle(index0, index1, index2);
								meshBuilder.AddTriangle(index2, index1, index3);
						
							} else
							{
								meshBuilder.AddTriangle(index0, index2, index1);
								meshBuilder.AddTriangle(index2, index3, index1);
							}
						}
						else{
							if(j==x2)
							{
							meshBuilder.AddTriangle(index0, index2, index1);
							meshBuilder.AddTriangle(index2, index3, index1);
							} else{
								meshBuilder.AddTriangle(index0, index1, index2);
								meshBuilder.AddTriangle(index2, index1, index3);
							}
						}
					}


				}
			}
		}



	}

	private void bw_DoWork(object sender, DoWorkEventArgs e)
	{
		calculateMesh ();
	}
	private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		updateGraph=true;
	}
	public Mesh BuildMesh()
	{
		//Create a new mesh builder:
		
		
		//create the Unity mesh:
		Mesh mesh = meshBuilder.CreateMesh();
		
		//have the mesh calculate its own normals:
		mesh.RecalculateNormals();
		//return the new mesh:
		return mesh;
	}
	
	// Update is called once per frame
	void Update () {
		if (updateGraph) {
			updateGraph = false;
			mesh1 = BuildMesh ();
			MeshFilter filter = GetComponent<MeshFilter> ();
			//If the MeshFilter exists, attach the new mesh to it.
			//Assuming the GameObject also has a renderer attached, our new mesh will now be visible in the scene.
			if (filter != null) {
				filter.sharedMesh = mesh1;
			}
			active=false;

			//print(ObjExporter.MeshToString(filter));
		}
		
	}
}
