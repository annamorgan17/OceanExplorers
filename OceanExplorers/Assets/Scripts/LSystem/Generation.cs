using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;
using System;
using Plant.Utilities;
using System.Linq;
public class Generation : MonoBehaviour { 
    public LSystemVisualData lSystemVisualData; 
    public EditorVar EditorVariables; 

    //List of branches to meshify
    private List<MeshFilter> meshObjectList; 
    //stack of transform locations to push and pop
    private Stack<TransformInfo> transformStack;
    //Gameboy used for location 
    private GameObject turtle;
    //Highest y value for texture calculation
    private float highestY; 
    //leaves
    [Header("Leaves")] 
    private List<Leaf> leaves = new List<Leaf>();
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    //Lsystem specific
    [Header("LSystem")]  
    [HideInInspector] string currentString = string.Empty;
    private float currentLength;
    //Draw a box around its position
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        MeshExstension.DrawCube(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.localScale);
    } 
    public void Rotation() { transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0 + lSystemVisualData.RotationAngle, transform.eulerAngles.z); }
    private void Start() {
        Make();
    }
    public void Clear() {
        //Remove previous validates
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Validate")) {
            Destroy(item);
        } 
    }
    public void Make(){ 
        //set current string to inspector value
        currentString = lSystemVisualData.StartString; 
        //Transform rotation
        transform.rotation = Quaternion.Euler(new Vector3(-90, 0, -90));   
        
        rules = new Dictionary<char, string>(); 
        foreach (var item in lSystemVisualData.dictionary) { 
            Debug.Log(item.Key + " -> " + item.Value);
            if (rules.ContainsKey(item.Key)) {
                Debug.LogError("Duplicate axoim. Duplicate character " + item.Key + " will not be added");
            } else {
                rules.Add(item.Key, item.Value);
            }
        } 
         
        meshObjectList = new List<MeshFilter>(); 
        transformStack = new Stack<TransformInfo>(); 

        //Turtle Transform Info
        turtle = new GameObject("turtle");
        turtle.tag = "Validate";
        turtle.transform.rotation = transform.rotation;
        turtle.transform.position = transform.position;

        //overwrite current string is start string was blank
        if (currentString == "") {
            currentString = lSystemVisualData.dictionary[0].Key.ToString();
        } 
        //generate l system n times
        for (int i = 0; i < lSystemVisualData.generations; i++) {
            new Lsystem(ref currentString, rules, lSystemVisualData.ammendmentChance);
        }

        //add together for pillar generation
        for (int i = 0; i < lSystemVisualData.pillarHeight; i++) {
            currentString += currentString;
        }

        //Draw and make mesh
        Gen();

        MeshExstension.CombineMeshes(meshObjectList, transform);
         
    }
    private void Update(){  
        //rotating the object
        if (lSystemVisualData.rotate)  
            transform.Rotate(Vector3.forward * (Time.deltaTime * 20f)); 
    } 
    //Branch
    private Vector3 Move(Vector3 direction, float length, float Gravity = 0){
        //Moving the turtle forward by a direciton and length with a basic gravity applied.
        turtle.transform.Translate(direction * (length + (UnityEngine.Random.Range(0, lSystemVisualData.lengthVariance * 100f) / 100f)));
        turtle.transform.position = turtle.transform.position + (Vector3.down * Gravity);
        return turtle.transform.position;
    }

    private void DrawBranch(Vector3 pA, Vector3 pB, float length, out TransformInfo ti) {

        GameObject gm = new GameObject("Branch", typeof(MeshFilter), typeof(MeshRenderer));
        Vector3 between = pB - pA;
        float thicknessMultiplier = (float)(UnityEngine.Random.Range(0f, 5f) * (float)lSystemVisualData.randomThickness) / 50f;

        //shape
        if (lSystemVisualData.shape == Shape.PrimitiveCube) {
            gm.GetComponent<MeshFilter>().mesh = MeshExstension.PrimitiveShape(PrimitiveType.Cube);
            gm.transform.localScale = new Vector3((length) / lSystemVisualData.thickness + thicknessMultiplier, (between.magnitude) * 1f, (length) / lSystemVisualData.thickness + thicknessMultiplier);
        } else {
            gm.GetComponent<MeshFilter>().mesh = MeshExstension.PrimitiveShape(PrimitiveType.Cylinder);
            gm.transform.localScale = new Vector3((length) / lSystemVisualData.thickness + thicknessMultiplier, (between.magnitude) * 0.5f, (length) / lSystemVisualData.thickness + thicknessMultiplier);
        }

        //colour
        /*
        if (lSystemVisualData.FlipColour) gm.GetComponent<MeshRenderer>().material.color = GetColour(lSystemVisualData.Col2, lSystemVisualData.Col1);
        else gm.GetComponent<MeshRenderer>().material.color = GetColour(lSystemVisualData.Col1, lSystemVisualData.Col2);

        */
        gm.GetComponent<MeshRenderer>().material.color = lSystemVisualData.Colour.Evaluate(0);
        //transform
        gm.transform.localPosition = pA + (between / 2.0f);
        gm.transform.LookAt(pB);
        gm.transform.Rotate(90, 0, 0);
        gm.tag = "Validate";
        ti = new TransformInfo(new TransformHolder(gm.transform), length);
        meshObjectList.Add(gm.GetComponent<MeshFilter>());

    }
    private void Gen() {
        currentLength = lSystemVisualData.length;

        //Sort out leaf parent
        GameObject a_Leaves = new GameObject("Leaves");
        a_Leaves.tag = "Validate";
        a_Leaves.transform.SetParent(gameObject.transform);

        List<TransformInfo> LeafNodes = new List<TransformInfo>();
        List<char> ignoredWarnings = new List<char> {'(',')','1','2','3','4','5','6','7','8','9','0','.' };  //warning ignore data likely to be passed in variables
        highestY = 1;

        for (int i = 0; i < currentString.Length; i++) {  

            //Current character
            Char c = currentString.ToCharArray()[i];

            //Current values
            TransformInfo current = new TransformInfo(new TransformHolder(turtle.transform), currentLength);
            float l = currentLength + (UnityEngine.Random.Range(0, lSystemVisualData.lengthVariance * 100f) / 100f);
            float ang = lSystemVisualData.angle; 

            //Replace every G with a F. Used in one fractal
            if (c == 'G')
                c = 'F';

            switch (c) { 
                case 'F': //forward and draw 
                    float Gravity = 0; 
                    //Extract length and gravity paramiter
                    if (i + 1 < currentString.ToCharArray().Length) {
                        if (currentString.ToCharArray()[i + 1] == '(') {
                            List<float> Param = Lsystem.ExtractParmiter(currentString, i + 1, current, lSystemVisualData.Variables, lSystemVisualData.angle);
                            if (Param.Count > 0)
                                l = Param[0];
                            if (Param.Count > 1)
                                Gravity = Param[1];
                        }
                    } 
                    //Draw branch 
                    DrawBranch(current.transform.position, Move(Vector3.forward, l, Gravity), currentLength, out current); 
                    break;
                case 'f': //move foward without draw
                    Move(Vector3.forward, l);
                    break;
                case 'b': //move backwards without draw
                    Move(-Vector3.forward, l);
                    break;
                case 'B': // back and draw 
                    DrawBranch(current.transform.position, Move(-Vector3.forward, l), currentLength, out current); 
                    break; 
                case '+':
                    //extract paramiter 
                    if (i + 1 < currentString.ToCharArray().Length) {
                        if (currentString.ToCharArray()[i + 1] == '(') {
                            List<float> Param = Lsystem.ExtractParmiter(currentString, i + 1, current, lSystemVisualData.Variables, lSystemVisualData.angle);
                            if (Param.Count > 0)
                                ang = Param[0];
                        }
                    } 
                    //rotate
                    turtle.transform.Rotate(Vector3.right   * ang); break; //pitch
                case '-': 
                    //extract paramiter
                    if (i + 1 < currentString.ToCharArray().Length) {
                        if (currentString.ToCharArray()[i + 1] == '(') {
                            List<float> Param = Lsystem.ExtractParmiter(currentString, i + 1, current, lSystemVisualData.Variables, lSystemVisualData.angle);
                            if (Param.Count > 0)
                                ang = Param[0];
                        }
                    }  
                    //rotate
                    turtle.transform.Rotate(Vector3.right   * -ang); break; //pitch 
                case '{': turtle.transform.Rotate(Vector3.up      * lSystemVisualData.angle); break; //yaw
                case '}': turtle.transform.Rotate(Vector3.up      * -lSystemVisualData.angle); break; //yaw 
                case '<': turtle.transform.Rotate(Vector3.forward * lSystemVisualData.angle); break; //roll
                case '>': turtle.transform.Rotate(Vector3.forward * -lSystemVisualData.angle); break; //roll   
                case '[':  //push from stack
                    transformStack.Push(new TransformInfo(current.transform, currentLength));

                    //Check if already a leaf location
                    for (int o = 0; o < LeafNodes.Count; o++) 
                        if (current.transform.position == LeafNodes[o].transform.position) 
                            LeafNodes.RemoveAt(o);
                    break;  
                case ']': //pop from stack
                    LeafNodes.Add(current);
                    //Highest Y is used in leaf texture to find max Y value possible
                    if (highestY < current.transform.position.y)
                        highestY = current.transform.position.y;
                    //set to current
                    current = transformStack.Pop();
                    current.SetTransform(ref turtle);
                    currentLength = current.branchLength; 
                    break;  
                default:
                    //create a warning, and not repeat the warning
                    bool warn = true;
                    foreach (var item in ignoredWarnings) 
                        if (item == c) 
                            warn = false; 
                    if (warn) {
                        Debug.LogWarning("Invalid L-tree operation with character: " + c);
                        ignoredWarnings.Add(c);
                    }
                    break;
            } 
        }

        //Generate leaf
        if (lSystemVisualData.LeafGeneration != GenerationType.None) {
            Texture2D[] texture = new Texture2D[2]; 
            //Texture 
            for (int i = 0; i < 2; i++) {
                texture[i] = new Texture2D(128, 128);
                for (int x = 0; x < texture[i].width; x++) 
                    for (int y = 0; y < texture[i].height; y++)
                        texture[i].SetPixel(x, y, Color.black);
            } 
            leaves = new List<Leaf>(); 

            //sort by height
            LeafNodes = LeafNodes.OrderBy(w => w.Y).ToList();
             
            foreach (var item in LeafNodes) {
                //Add leaf
                Leaf leaf = new Leaf(item, ref a_Leaves, lSystemVisualData.LeafGeneration);
                leaves.Add(leaf);

                //draw first texture if mesh generation (top values)
                if (lSystemVisualData.LeafGeneration == GenerationType.Mesh) {
                    leaf.DrawTexture(texture[0], highestY, gameObject.transform.position);
                }
            }
            if (lSystemVisualData.LeafGeneration == GenerationType.Mesh) {
                //Reverse list and draw second texture (bottom values)
                leaves.Reverse();
                foreach (var item in leaves) {
                    item.DrawTexture(texture[1], highestY, gameObject.transform.position);
                }
                //Create mesh
                MeshifyLeaves(texture);
            }
        }
    } 
    /*
    /// <summary>
    /// Used to return a colour gradient or soild colour dependant on inpsector value
    /// </summary>
    /// <param name="Col1">First colour</param>
    /// <param name="Col2"><Second colour/param>
    /// <returns></returns>
    private Color GetColour(Color Col1, Color Col2) { 
        if (lSystemVisualData.SolidColour) return Col1;
        else return Color.Lerp(Col2, Col1, (float)transformStack.Count / lSystemVisualData.generations);
    }
    */
    /// <summary>
    /// Creates a mesh using two textures generated from the leaf array
    /// </summary>
    /// <param name="tex"></param>
    private void MeshifyLeaves(Texture2D[] tex){ 
        //size are variables
        int xSize = tex[1].width;
        int zSize = tex[1].height;
        int mapScale = xSize / 10;

        //declaration
        Vector3[][] verticies = new Vector3[2][];
        verticies[0] = new Vector3[(xSize + 1) * (zSize + 1)];
        verticies[1] = new Vector3[(xSize + 1) * (zSize + 1)];
        int[][] triangles = new int[2][];
        triangles[0] = new int[xSize * zSize * 6];
        triangles[1] = new int[xSize * zSize * 6];  

        //loop through top and bottom map
        for (int t = 0; t < 2; t++) { 
            //loop through x and z
            for (int i = 0, z = 0; z <= zSize; z += 2) {
                for (int x = 0; x <= xSize; x += 2) {
                    //add to verts using colour to determin height
                    float YPos = ((float)tex[t].GetPixel(x, z).g * (highestY + 1));
                    verticies[t][i] = (new Vector3(((float)-x / 5f) + mapScale, YPos, ((float)-z / 5f) + mapScale)); 
                    i ++;
                } 
            } 

            int tris = 0;
            int vert = 0;

            //loop though x and z
            for (int z = 0; z < zSize; z++) {
                for (int x = 0; x < xSize; x++) {
                    //check the surrounding verts are filled
                    int A = vert + 0, B = vert + xSize + 1, C = vert + 1, D = vert + xSize + 2;
                    if (isVertFilled(A, t, ref verticies) & isVertFilled(C, t, ref verticies) & 
                        isVertFilled(B, t, ref verticies) & isVertFilled(D, t, ref verticies)) { 
                        //tri one
                        triangles[t][tris + 0] = A;
                        triangles[t][tris + 1] = B;
                        triangles[t][tris + 2] = C;

                        //tri two
                        triangles[t][tris + 3] = C;
                        triangles[t][tris + 4] = B;
                        triangles[t][tris + 5] = D;
                         
                    }
                    //incriment 
                    vert++;
                    tris += 6;
                }
                //incriment 
                vert++;
            } 
        }

        //declaration. Lists were best to use as tri list has unknown length
        List<Vector3> joinVerts = new List<Vector3>(); 
        List<int> joinTris = new List<int>();

        //add vert positions using data from upper and lower textures
        for (int i = 0; i < verticies[0].Length - 1; i += 1) {
             joinVerts.Add(verticies[0][i]);
             joinVerts.Add(verticies[1][i]); 
        }
        for (int z = 0, ver = 0; z <= zSize -1; z++) {
            for (int x = 0; x <= xSize - 1; x++) {
                //Position variables
                //A represents lower texture, while T represents upper texture. Number represent grid cordinates in relation to i = 1,1
                int row = (xSize + 2);
                int A11 = ver + 0;
                int T11 = ver + 1;
                int A12 = ver + 2;
                int T12 = ver + 3;
                int A21 = ver + row + 0;
                int T21 = ver + row + 1;
                int A22 = ver + row + 2;
                int T22 = ver + row + 3;

                //Back shape
                if (joinVerts[A21].y == 0 & joinVerts[T21].y == 0) {
                    if (joinVerts[A11].y != joinVerts[T11].y) { 
                        joinTris.Add(A11);
                        joinTris.Add(T11);
                        joinTris.Add(A12); 
                        joinTris.Add(T11);
                        joinTris.Add(T12);
                        joinTris.Add(A12);
                    }
                    
                }
                
                //Fount shape
                if (joinVerts[A22].y != joinVerts[T22].y) {
                    if (joinVerts[A12].y == 0 & joinVerts[T12].y == 0)
                    { 
                        joinTris.Add(A22);
                        joinTris.Add(T21);
                        joinTris.Add(A21);
                        joinTris.Add(A22);
                        joinTris.Add(T22);
                        joinTris.Add(T21);
                    }
                }

                //Left shape
                if (joinVerts[A21].y == joinVerts[T21].y | joinVerts[A22].y == joinVerts[T22].y) {
                    if (joinVerts[A21].y != 0 & joinVerts[T21].y != 0 & joinVerts[A11].y != 0 & joinVerts[T11].y != 0) {
                        joinTris.Add(A21);
                        joinTris.Add(T21);
                        joinTris.Add(A11);
                        joinTris.Add(T21);
                        joinTris.Add(T11);
                        joinTris.Add(A11);

                    }
                }
                
                //Right shape
                if (joinVerts[A11].y == joinVerts[T11].y | joinVerts[A21].y == joinVerts[T21].y) {
                    if (joinVerts[A11].y != 0 & joinVerts[T11].y != 0 & joinVerts[A21].y != 0 & joinVerts[T21].y != 0) {
                        joinTris.Add(A11);
                        joinTris.Add(T21);
                        joinTris.Add(A21);
                        joinTris.Add(A11);
                        joinTris.Add(T11);
                        joinTris.Add(T21);
                    }
                } 
                ver += 2;
            }
            ver += 2;
        }

        //Mesh to join between the two sides
        Mesh Mesh3 = new Mesh();
        Mesh3.vertices = joinVerts.ToArray();
        Mesh3.triangles = joinTris.ToArray();
        Mesh3.RecalculateNormals();
        Mesh3.RecalculateBounds(); 
        Mesh3.name = "Texture mesh"; 

        //Asign to gameobject
        GameObject gm3 = new GameObject("LeafMeshSide", typeof(MeshRenderer), typeof(MeshFilter));
        gm3.GetComponent<MeshFilter>().mesh = Mesh3;
        gm3.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard")); 
         
        //Top mesh creation
        Mesh Mesh1 = new Mesh();
        Mesh1.vertices = verticies[0];
        Mesh1.triangles = triangles[0];
        Mesh1.RecalculateNormals();
        Mesh1.RecalculateBounds();
        Mesh1.name = "Texture mesh";
        //Asign to gameobject
        GameObject gm1 = new GameObject("LeafMesh", typeof(MeshRenderer), typeof(MeshFilter));
        gm1.GetComponent<MeshFilter>().mesh = Mesh1;

        //Lower mesh creation
        Mesh Mesh2 = new Mesh();
        Mesh2.vertices = verticies[1];
        Mesh2.triangles = triangles[1].Reverse().ToArray();
        Mesh2.RecalculateNormals();
        Mesh2.RecalculateBounds();
        Mesh2.name = "Texture mesh";
        //Asign to gameobject
        GameObject gm2 = new GameObject("LeafMesh", typeof(MeshRenderer), typeof(MeshFilter));
        gm2.GetComponent<MeshFilter>().mesh = Mesh2;

        //Combine into a mesh
        List<MeshFilter> meshList = new List<MeshFilter> { gm1.GetComponent<MeshFilter>(), gm2.GetComponent<MeshFilter>(), gm3.GetComponent<MeshFilter>()};
        GameObject leaves = MeshExstension.CombineMeshes(meshList);

        //Colour mesh
        foreach (var item in leaves.transform.GetComponentsInChildren<MeshFilter>()) {
            Color[] colors = new Color[item.mesh.vertexCount];
            for (int i = 0; i < colors.Length; i++) {
                float random = Mathf.PerlinNoise(item.mesh.vertices[i].x, item.mesh.vertices[i].z); 
                    colors[i] = colour(63, 122, 77); 
                
            }
            item.mesh.colors = colors;
        } 

        //Transform mesh
        leaves.transform.Rotate(0,180,0);
        leaves.transform.position = new Vector3(-0.5f,0,-0.5f);
        leaves.tag = "Validate"; 
    }
    /// <summary>
    /// translates a 255 colour in a unity colour
    /// </summary>
    /// <param name="r">0 - 255 red</param>
    /// <param name="g">0 - 255 green</param>
    /// <param name="b">0 - 255 blue</param>
    /// <returns></returns>
    private Color colour(int r, int g, int b) {
        return new Color(r / 255f, g / 255f, b / 255f);
    }
    /// <summary>
    /// check if a vertex has height assosiated with it
    /// </summary>
    /// <param name="vert">vert looking at</param>
    /// <param name="t">texture looking at</param>
    /// <param name="verticies">array looking at</param>
    /// <returns></returns>
    private bool isVertFilled(int vert, int t, ref Vector3[][] verticies) { 
        //return if out of bounds
        if (vert < 0 | vert > verticies[t].Length) {
            return false;
        } 
        //return value
        return verticies[t][vert].y != 0;
    }

 }