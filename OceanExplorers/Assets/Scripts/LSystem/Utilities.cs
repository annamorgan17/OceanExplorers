using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Plant.Utilities {
    public static class TextureExstension {
        /// <summary>
        /// Draw a circle onto a texture. Used for leaf mesh generation
        /// </summary>
        /// <param name="tex">Texture being changed</param>
        /// <param name="color">Colour of circle</param>
        /// <param name="x">Mid point x</param>
        /// <param name="y">Mid point Y</param>
        /// <param name="radius">Radius</param>
        public static void DrawCircle(ref Texture2D tex, Color color, int x, int y, int radius = 3) {
            float rSquared = radius * radius;
            //Loop through texture
            for (int u = x - radius; u < x + radius + 1; u++) {
                for (int v = y - radius; v < y + radius + 1; v++) {
                    //if area draw pixels
                    if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared) {
                        tex.SetPixel(u, v, color);
                    }
                }
            }
        }
    }
    public static class MeshExstension {
        /// <summary>
        /// Draw a gizmo cube. used for roation and scale
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 scale) {
            //Position
            position = new Vector3(position.x, position.y + (scale.y / 2), position.z);

            //Apply to matrix and draw
            Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, scale);
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
            Gizmos.matrix *= cubeTransform;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = oldGizmosMatrix;
        }
        /// <summary>
        /// Return a primitive shape as mesh
        /// </summary>
        /// <param name="type">Type to return</param>
        /// <returns></returns>
        public static Mesh PrimitiveShape(PrimitiveType type) {
            GameObject gm = GameObject.CreatePrimitive(type);
            Mesh m = gm.GetComponent<MeshFilter>().mesh;
            gm.AddComponent<Destroy>();
            return m;
        }
        /// <summary>
        /// Draw a sphere. Usefull for debuging where Gizmos cannot be used
        /// </summary>
        /// <param name="pos">Position of sphere</param>
        /// <param name="size">Size of sphere</param>
        /// <param name="c">Colour of sphere</param>
        /// <returns></returns>
        public static GameObject DrawSphere(Vector3 pos, float size, Color c) { 
            GameObject g = new GameObject("Giz " + c, typeof(MeshRenderer), typeof(MeshFilter)); 
            g.GetComponent<MeshFilter>().mesh = MeshExstension.PrimitiveShape(PrimitiveType.Sphere); 
            g.GetComponent<MeshRenderer>().material.color = c; 
            g.transform.position = pos; 
            g.transform.localScale = new Vector3(size, size, size); 
            return g;
        }
        #region combine mesh
        /// <summary>
        /// Combine a list of mesh filters into one mesh (Multiple if out of bounds) to save on unnessiary gameobjects
        /// </summary>
        /// <param name="meshObjectList">List of mesh filters we are combining</param>
        /// <param name="transform">Transform for children to be parented to</param>
        /// <param name="MaterialEmision">Emmision of material</param>
        /// <param name="MaterialShinyness">Shinyness of material</param>
        /// <returns></returns>
        public static GameObject CombineMeshes(List<MeshFilter> filterList, CustomGradient customGadient, float Ymax, Transform transform = null, float MaterialEmision = 1, float MaterialShinyness = 1) {
            //Gameobject setup
            GameObject MeshObject = new GameObject("MeshObject", typeof(MeshFilter), typeof(MeshRenderer));
            MeshObject.tag = "Validate";
            Material material = GetMaterial(MaterialEmision, MaterialShinyness);
            MeshObject.GetComponent<MeshRenderer>().material = material;

            //set parent
            if (transform != null) { 
                MeshObject.transform.SetParent(transform);
            }
            List<Color> colours = new List<Color>();
            CombineInstance[] combine = new CombineInstance[filterList.Count];
            //loop through mesh object list
            int i = 0, VertexPreviousCall = 0;
            while (i < filterList.Count) {
                if (filterList[i] != null) {
                    if (filterList[i].mesh != null) {
                        //add mesh
                        combine[i].mesh = filterList[i].sharedMesh;
                        combine[i].transform = filterList[i].transform.localToWorldMatrix;
                        //add colour mesh
                        for (int d = 0; d < combine[i].mesh.vertexCount; d++) {
                            //colours.Add(filterList[i].GetComponent<MeshRenderer>().material.color);
                            float evalulate = combine[i].mesh.vertices[d].y;
                            Color colour = customGadient.Evaluate(0);
                            //Debug.LogError(evalulate + " : " + combine[i].mesh.vertices[d] + " : " + Ymax);
                            colours.Add(colour);
                            //Debug.LogError(customGadient.Evaluate(0));
                        };
                        //destroy added mesh
                        filterList[i].gameObject.AddComponent<Destroy>();
                        //combine after 65000 to avoid mesh vertex limits
                        if ((i * filterList[0].mesh.vertexCount) > (VertexPreviousCall + 65000)) {
                            VertexPreviousCall += Combine(colours, combine, MeshObject, material);
                            combine = new CombineInstance[filterList.Count];
                            colours = new List<Color>();
                        }
                    }
                }
                i++;
            }
            //combine any remaining
            Combine(colours, combine, MeshObject, material);
            return MeshObject;
        } 
        /// <summary>
        /// Return material and set the emmision and shinyness value to a custom shader
        /// </summary>
        /// <param name="MaterialEmision">Emmision</param>
        /// <param name="MaterialShinyness">Shinyness</param>
        /// <returns></returns>
        private static Material GetMaterial(float MaterialEmision, float MaterialShinyness) {
            Material material = Resources.Load<Material>("Coral");
            //material.SetFloat("_Emission", MaterialEmision);
            //material.SetFloat("_Shininess", MaterialShinyness);
            return material;
        }
        /// <summary>
        /// Individually combine each mesh
        /// </summary>
        /// <param name="colours">Colour vertex array</param>
        /// <param name="combine">what we are combining</param>
        /// <param name="MeshObject">Parent</param>
        /// <param name="material">Material</param>
        /// <returns></returns>
        private static int Combine(List<Color> colours, CombineInstance[] combine, GameObject MeshObject, Material material) {
            //return if no mesh
            if (combine == null | combine.Length == 0) {
                return 0;
            }

            //create object to hold mesh
            GameObject MeshChild = new GameObject("Mesh Child");

            //create mesh from combined meshes, and recalculate
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine);
            combinedMesh.RecalculateBounds();
            combinedMesh.RecalculateNormals();

            //colour
            Color[] col = colours.ToArray();
            if (colours.Count != combinedMesh.vertices.Length) 
                Array.Resize(ref col, combinedMesh.vertices.Length);  
            Array.Resize(ref col, combinedMesh.vertices.Length);

            combinedMesh.colors = col; 

            //rendering components
            MeshChild.AddComponent<MeshFilter>().mesh = combinedMesh;
            MeshChild.AddComponent<MeshRenderer>().material = material;

            //parent
            MeshChild.transform.SetParent(MeshObject.transform);
            MeshChild.tag = "Validate";
            return combinedMesh.vertexCount;
        }
        #endregion
    } 
}
