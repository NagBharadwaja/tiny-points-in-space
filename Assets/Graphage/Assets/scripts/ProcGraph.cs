using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.ComponentModel;
using Graphing;
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
public class ProcGraph : ProcBase
{
	private bool tangentXOn = false;
	private bool tangentXOld = false;
	private bool tangentYOn = false;
	private bool tangentYOld = false;
	private bool integralRenderOn = false;
	private bool integralRenderOld = false;
	private bool pointAOn = false;
	private bool pointBOn = false;

	//status fields for displaying debug information. 
	//status text for worker progress update
	private string statusText;
	//flag for letting know that statusText has been changed
	private bool statusUpdate=false;

	//all vectors in a mesh
	private List<Vector3> vectors;

	//flag for letting know that the graph should be rerendered
	private bool updateGraph=false;

	//the resolution variable in this script
	private int m_SegmentCount = 10;

	//define the background worker
	private BackgroundWorker bw = new BackgroundWorker();

	//the thing that builds the visable mesh
	private MeshBuilder meshBuilder;
	//the thing that builds the raycastable mesh
	private MeshBuilder collisionMesh;
	//the damn mesh
	private Mesh mesh;

	//we might not need this anymore, we will see
	private ParticleSystem.Particle[] points;
	private LineRenderer line;
	//this is ray, he's a pretty cool guy, though some times erratic
	private Ray tangentRay;

	//DIS IS DA CAMERA GUYS HEY GUYS. HEY. GUYS. PAY ATTENTION THIS IS THE THING 
	public GameObject curCamera;

	private Vector3 pointA;
	private Vector3 pointB;
	//line renderers for the x and Y tangent lines
	public GameObject xTangentLine;
	public GameObject yTangentLine;

	//integral Renderer for space under a graph
	public GameObject integralRenderer;

	public GameObject pointerA;
	public GameObject pointerB;

	private drawLine xTanScript;
	private drawLine yTanScript;
	private integralRender integralScript;
	private GraphData data;
	//ready set go
	void Start()
	{
		//define settings for the background worker
		bw.WorkerReportsProgress = true;
		bw.DoWork += new DoWorkEventHandler (bw_DoWork);
		bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
		bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler (bw_RunWorkerCompleted);

		//fetch external scripts
		xTanScript=(drawLine)xTangentLine.GetComponent(typeof(drawLine));
		yTanScript =(drawLine)yTangentLine.GetComponent(typeof(drawLine));
		integralScript=(integralRender)integralRenderer.GetComponent(typeof(integralRender));
		line = GetComponent<LineRenderer>();
	}

	/*
	 * void castTangentRay()
	 * casts a ray from the camera, if it hits something it calls the tangent line functions
	 */
	public void castRay(int target)
	{
		//if you uncomment this line it should cast from the direction of the defined camera,
		//this may or may not work as implemented with the oculus
		tangentRay=new Ray(curCamera.transform.position,curCamera.transform.forward);
		//tangentRay= curCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		//hot damn ah hit something!
		if (Physics.Raycast(tangentRay, out hit, 100))
		{
			if(target==0)
			{
				pointerA.SetActive(true); 
				pointerA.transform.position=hit.point;
				pointA=hit.point;
				pointAOn=true;
				//pew pew
				//Debug.DrawLine(tangentRay.origin, hit.point);
				calcTangentLine(hit.point,data.MinX,data.MaxX,data.MinY,data.MaxY);
				if(line.enabled&&tangentXOn)
				{
					sliderTest(pointA.x);
				} else if(line.enabled&&tangentYOn)
				{
					sliderTest2 (pointA.z);
				}
			} else if(target==1)
			{
				pointerB.SetActive(true);
				pointerB.transform.position=hit.point;
				pointB=hit.point;
				pointBOn=true;
			}
			if(integralRenderOn)
			{
				integralScript.renderIntegral(mesh,pointA,pointB,data.MinX,data.MaxX,data.MinY,data.MaxY,m_SegmentCount);
			}


		}

	}
	/*
	 * bool calcTangentLine(Vector3 location,float lowerX,float upperX,float lowerY,float upperY)
	 * calculates both tangent lines at the point closest to location
	 * bounds are required in order to calculate the closest point coordinates
	 */
	public bool calcTangentLine(Vector3 location,float lowerX,float upperX,float lowerY,float upperY)
	{
		//halp something isnt working right
		if(location.x<lowerX||location.x>upperX||location.z<lowerY||location.z>upperY||mesh==null)
		{
			return false;
		}

		//find the iteration size for both dimensions
		float dividablePartX=(upperX-lowerX)/(m_SegmentCount-1);
		float dividablePartY=(upperY-lowerY)/(m_SegmentCount-1);

		//calculate the coordinates on the grid
		int x = Mathf.FloorToInt((location.x-lowerX) / dividablePartX);
		int y = Mathf.FloorToInt((location.z-lowerY) / dividablePartY);

		//retrieve the vector4 tangent and convert to vector3, this is in terms of X
		Vector4 tangent = mesh.tangents[x+(y*m_SegmentCount)];
		Vector3 slope = tangent;

		//find the binormal, or tangent in terms of y
		Vector3 binormal = Vector3.Cross (mesh.normals[x+(y*m_SegmentCount)], slope) * tangent.w;

		//calculate the line endings
		Vector3 positiveX = mesh.vertices[x+(y*m_SegmentCount)]+slope*50;
		Vector3 negativeX = mesh.vertices[x+(y*m_SegmentCount)]-slope*50;
		Vector3 positiveY = mesh.vertices[x+(y*m_SegmentCount)]+binormal*50;
		Vector3 negativeY = mesh.vertices[x+(y*m_SegmentCount)]-binormal*50;

		//status update for tangents, removable
		//statusfieldX.text = slope.ToString ();
		//statusfieldY.text = binormal.ToString ();

		//draw lines
		xTanScript.lineDraw(positiveX,negativeX);
		yTanScript.lineDraw(positiveY,negativeY);

		return true;
	}
/*
Derived from
Lengyel, Eric. "Computing Tangent Space Basis Vectors for an Arbitrary Mesh". Terathon Software 3D Graphics Library, 2001.
[url]http://www.terathon.com/code/tangent.html[/url]
*/
	public void TangentSolve(Mesh theMesh) {
		
		int vertexCount = theMesh.vertexCount;
		Vector3[] vertices = theMesh.vertices;
		Vector3[] normals = theMesh.normals;
		Vector2[] texcoords = theMesh.uv;
		int[] triangles = theMesh.triangles;
		int triangleCount = triangles.Length/3;
		
		Vector4[] tangents = new Vector4[vertexCount];
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		
		int tri = 0;
		
		for (int i = 0; i < (triangleCount); i++) {
			
			int i1 = triangles[tri];
			int i2 = triangles[tri+1];
			int i3 = triangles[tri+2];
			
			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];
			
			Vector2 w1 = texcoords[i1];
			Vector2 w2 = texcoords[i2];
			Vector2 w3 = texcoords[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float r = 1.0f / (s1 * t2 - s2 * t1);
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
			
			tri += 3;
			
		}
		
		
		
		for (int i = 0; i < (vertexCount); i++) {
			
			Vector3 n = normals[i];
			Vector3 t = tan1[i];
			
			// Gram-Schmidt orthogonalize
			Vector3.OrthoNormalize(ref n, ref t);
			
			tangents[i].x  = t.x;
			tangents[i].y  = t.y;
			tangents[i].z  = t.z;
			
			// Calculate handedness
			tangents[i].w = ( Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f ) ? -1.0f : 1.0f;
			
		}      
		
		theMesh.tangents = tangents;
		
	}


	//these are unnecesarry if you remove the sliders
	public void sliderTest(float value)
	{
		if(data!=null)
		{
			generateTraceOfX (value, data.MinX, data.MaxX);
		}
	}
	public void sliderTest2(float value)
	{
		if(data!=null)
		{
			generateTraceOfY (value, data.MinY, data.MaxY);
		}
	}


	/*
	 * bool generateTraceOfX(float value,float lowerX,float upperX,int numOfFiller)
	 * generates a particle line across the surface of the mesh.
	 * numOfFiller is the number of particles between actual points on the mesh
	 */
	public bool generateTraceOfX(float value,float lowerX,float upperX)
	{


		if(value<lowerX||value>upperX||mesh==null)
		{
			return false;
		}

		float dividablePart=(upperX-lowerX)/(m_SegmentCount-1);//.2
		int x = Mathf.FloorToInt((value-lowerX) / dividablePart);
		float percentage=((value-lowerX)%dividablePart) / dividablePart;
		//points = new ParticleSystem.Particle[m_SegmentCount*numOfFiller];
	;
		line.enabled=false;
		line.SetVertexCount(m_SegmentCount);
		Vector3 pointOne=mesh.vertices[x];
		Vector3 pointTwo=mesh.vertices[x+1];
		Vector3 newPoint=pointTwo-pointOne;
		Vector3 lowPoint;
		newPoint=pointOne+percentage*newPoint;
//		points[0].position=newPoint;
//		points[0].color=new Color(0f,1f,0f);
//		points[0].size=1f;
		lowPoint=newPoint;
		for(int i=0;i<m_SegmentCount;i++)
		{

			pointOne=mesh.vertices[i*m_SegmentCount+x];
			pointTwo=mesh.vertices[i*m_SegmentCount+x+1];
			newPoint=pointTwo-pointOne;
			newPoint=pointOne+percentage*newPoint;
			//Vector3 highPoint=newPoint;
			//points[i*numOfFiller].position=newPoint;
			//points[i*numOfFiller].color=new Color(0f,1f,0f);
			//points[i*numOfFiller].size=1f;
			//for(int j=1;j<numOfFiller;j++)
			//{
			//	newPoint=highPoint-lowPoint;

			//	newPoint=lowPoint+((1.0f*j))*newPoint;
				//points[i*numOfFiller-j].position=newPoint;
				//points[i*numOfFiller-j].color=new Color(0f,1f,0f);
				//points[i*numOfFiller-j].size=1f;
			//}
			//lowPoint=highPoint;
			line.SetPosition(i,newPoint);
		}
		//particleSystem.SetParticles (points, points.Length);
		line.enabled=true;
		return true;
	}
	/*
	 * bool generateTraceOfY(float value,float lowerX,float upperX,int numOfFiller)
	 * generates a particle line across the surface of the mesh.
	 * numOfFiller is the number of particles between actual points on the mesh
	 */
	public bool generateTraceOfY(float value,float lowerX,float upperX)
	{
		

		if(value<lowerX||value>upperX||mesh==null)
		{
			return false;
		}
		//line = GetComponent<LineRenderer>();
		line.enabled=false;
		line.SetVertexCount(m_SegmentCount);
		float dividablePart=(upperX-lowerX)/(m_SegmentCount-1);
		int x = Mathf.FloorToInt((value-lowerX) / dividablePart);
		float percentage=((value-lowerX)%dividablePart) / dividablePart;
		//points = new ParticleSystem.Particle[m_SegmentCount*numOfFiller];
		Vector3 pointOne=mesh.vertices[x*m_SegmentCount];
		Vector3 pointTwo=mesh.vertices[(x+1)*m_SegmentCount];
		Vector3 newPoint=pointTwo-pointOne;
		Vector3 lowPoint;
		newPoint=pointOne+percentage*newPoint;
		lowPoint=newPoint;
		for(int i=0;i<m_SegmentCount;i++)
		{
			
			pointOne=mesh.vertices[x*m_SegmentCount+i];
			pointTwo=mesh.vertices[(x+1)*m_SegmentCount+i];
			newPoint=pointTwo-pointOne;
			newPoint=pointOne+percentage*newPoint;
			line.SetPosition(i,newPoint);


		}
		//particleSystem.SetParticles (points, points.Length);
		line.enabled=true;
		return true;
		
	}

	/*
	 * this is the progressChanged function which is called when the worker.ReportProgress
	 * function is called in the DoWork function. Mostly its designed for percentage
	 */
	private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		if(e.ProgressPercentage==1)
		{
			statusText="begin generating mesh";
		} else 
		{
			statusText="generating mesh progress:"+e.ProgressPercentage.ToString()+"%";
		}
		statusUpdate = true;
	}
	private void bw_DoWork(object sender, DoWorkEventArgs e)
	{
		//first message
		BackgroundWorker worker = sender as BackgroundWorker;
		worker.ReportProgress(1);

		//define meshBuilders
		meshBuilder = new MeshBuilder();
		collisionMesh= new MeshBuilder();

		//WE MUST FIND THE MINIMUM AND MAXIMUM
		Vector3 minVec= vectors[0];
		Vector3 maxVec=vectors[0];
		for(int i=0;i<vectors.Count;i++)
		{
			if(vectors[i].y!=Mathf.Infinity)
			{
				maxVec=Vector3.Max(maxVec,vectors[i]);
				minVec=Vector3.Min(minVec,vectors[i]);
			}
		}

		//begin the mesh creation loop of doooooooooooooooom
		for (int i = 0; i < m_SegmentCount; i++)
		{
			worker.ReportProgress(m_SegmentCount-(m_SegmentCount-i));

			for (int j = 0; j < m_SegmentCount; j++)
			{
				
				//assume the position!
				Vector3 pos=vectors[j*m_SegmentCount+i];

				//the zmax is currently hardcoded, we need to modify this
				if(pos.y>data.MaxZ&&pos.y!=Mathf.Infinity)
				{
					pos.y=data.MaxZ;
					maxVec.y=data.MaxZ;
				}
				if(pos.y<data.MinZ)
				{
					pos.y=data.MinZ;
					minVec.y=data.MinZ;
				}

				Color color=HSVToRGB(convertRange (minVec.y,maxVec.y,0f,1f,pos.y),.9f,.9f);

				//since uvmaps tile infinitly by default, we can just use the x and y coords to make the grid correct
				Vector2 uv = new Vector2(pos.x, pos.z);

				bool buildTriangles = i > 0 && j > 0;

				//build some triangles
				BuildQuadForGraph(meshBuilder, pos, uv, buildTriangles, m_SegmentCount,color);

				//check if collision mesh generation
				if(i%5==0&&(j%5==0))
				{

					BuildQuadForGraph(collisionMesh, pos, uv, buildTriangles, m_SegmentCount/5+1,color);

				}
			}
		}
	}
	//converts the range of the min and max graph values to be the scale that is used in HSV
	private float convertRange(float a, float b, float c, float d, float x)
	{
		return c+(d-c)*(x-a)/(b-a);
	}

	//triggered when the background worker is completed
	private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		updateGraph = true;
	}

	//used to call the mesh from other scripts
	public void createMesh(List<Vector3> coords,GraphData newData)
	{
		data = newData;
		m_SegmentCount = data.Res;
		vectors = coords;
		bw.RunWorkerAsync ();

	
	}
	void Update()
	{
		if (Input.GetButton ("Fire1")) {
			castRay(0);
		} else if (Input.GetButton ("Fire2")) {
			castRay(1);
		}
		if(Input.GetButtonDown ("Fire3"))
		{
			tangentXOn=!tangentXOn;
		}
		if(Input.GetButtonDown ("Jump"))
		{
			tangentYOn=!tangentYOn;
		}
		if(Input.GetButtonDown ("LeftBumper"))
		{

			if(pointAOn&&pointBOn&&!integralRenderOn)
			{
				integralScript.renderIntegral(mesh,pointA,pointB,data.MinX,data.MaxX,data.MinY,data.MaxY,m_SegmentCount);
				integralRenderOn=!integralRenderOn;
			} else if(integralRenderOn)
			{
				integralRenderOn=!integralRenderOn;
			}

		}
		if (updateGraph) {
			updateGraph = false;
			tangentXOn=false;
			tangentYOn=false;
			pointerB.SetActive(false);
			pointerA.SetActive(false); 
			pointAOn=false;
			pointBOn=false;
			integralRenderOn=false;
			line.enabled=false;

			mesh = BuildMesh ();

			//Look for a MeshFilter component attached to this GameObject:
			MeshFilter filter = GetComponent<MeshFilter> ();
			//If the MeshFilter exists, attach the new mesh to it.
			//Assuming the GameObject also has a renderer attached, our new mesh will now be visible in the scene.
			if (filter != null) {
					filter.sharedMesh = mesh;
			}

			collisionMesh=flipNormals(collisionMesh);
			GetComponent<MeshCollider> ().sharedMesh=null;
			GetComponent<MeshCollider> ().sharedMesh =collisionMesh.CreateMesh();
			transform.renderer.enabled=true;
		 	//integralScript.renderIntegral(mesh,mesh.vertices[0],mesh.vertices[8000],-10,10,-10,10,m_SegmentCount);
		}
		if(statusUpdate)
		{
			statusUpdate=false;
		//	statusfield.text=statusText;
            print(statusText);
		}
		if(tangentXOn!=tangentXOld)
		{
			xTanScript.enableLine(tangentXOn);
			tangentXOld=tangentXOn;

			if(!tangentXOn&&!tangentYOn)
			{
				line.enabled=false;
			} else if(!tangentYOn)
			{
				sliderTest(pointA.x);
			} else if(tangentXOn)
			{
                sliderTest(pointA.x);
				tangentYOn=false;
			}
		}
		if(tangentYOn!=tangentYOld)
		{
			yTanScript.enableLine(tangentYOn);
			tangentYOld=tangentYOn;
			if(!tangentYOn&&!tangentXOn)
			{
				line.enabled=false;
			} else if(!tangentXOn)
			{
				sliderTest2(pointA.z);
			} else if(tangentYOn)
			{
                sliderTest2(pointA.z);
				tangentXOn=false;
			}
		}
		if(integralRenderOn!=integralRenderOld)
		{
			integralScript.activate(integralRenderOn);
			integralRenderOld=integralRenderOn;
		}

	}

	//this is for the collisionMesh, it generates an inverted mesh and adds it to the existing meshbuilder 
	private MeshBuilder flipNormals(MeshBuilder builder)
	{

	
		List<int> triangles = builder.m_Indices;
		triangles.AddRange (triangles);
		for (int i=triangles.Count/2;i<triangles.Count;i+=3)
		{
			int temp = triangles[i + 0];
			triangles[i + 0] = triangles[i + 1];
			triangles[i + 1] = temp;
		}
		return builder;
	}

	//Build the mesh
	public override Mesh BuildMesh()
	{
		//Create a new mesh builder:

		
		//create the Unity mesh:
		Mesh mesh = meshBuilder.CreateMesh();
		
		//have the mesh calculate its own normals:
		mesh.RecalculateNormals();
		TangentSolve (mesh);
		//return the new mesh:
		return mesh;
	}

	//color converter
	public static Color HSVToRGB(float H, float S, float V)
	{
		if (S == 0f)
			return new Color(V,V,V);
		else if (V == 0f)
			return Color.black;
		else
		{
			Color col = Color.black;
			float Hval = H * 6f;
			int sel = Mathf.FloorToInt(Hval);
			float mod = Hval - sel;
			float v1 = V * (1f - S);
			float v2 = V * (1f - S * mod);
			float v3 = V * (1f - S * (1f - mod));
			switch (sel + 1)
			{
			case 0:
				col.r = V;
				col.g = v1;
				col.b = v2;
				break;
			case 1:
				col.r = V;
				col.g = v3;
				col.b = v1;
				break;
			case 2:
				col.r = v2;
				col.g = V;
				col.b = v1;
				break;
			case 3:
				col.r = v1;
				col.g = V;
				col.b = v3;
				break;
			case 4:
				col.r = v1;
				col.g = v2;
				col.b = V;
				break;
			case 5:
				col.r = v3;
				col.g = v1;
				col.b = V;
				break;
			case 6:
				col.r = V;
				col.g = v1;
				col.b = v2;
				break;
			case 7:
				col.r = V;
				col.g = v3;
				col.b = v1;
				break;
			}
			col.r = Mathf.Clamp(col.r, 0f, 1f);
			col.g = Mathf.Clamp(col.g, 0f, 1f);
			col.b = Mathf.Clamp(col.b, 0f, 1f);
			return col;
		}
	}

}
