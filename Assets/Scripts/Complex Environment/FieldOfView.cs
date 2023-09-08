using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float radius;
    [Range(0, 360)]
    public float fullAngle;

  /*  private void Start() {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        Vector3[] vertices = new Vector3[3];
        Vector2[] uv = new Vector2[3];
        int[] triangles = new int[3];
        Vector3 og = Vector3.zero;
        vertices[0] = og;
        vertices[1] = og + DirFromAngle3D(fullAngle / 2f, -fullAngle/2f);//new Vector3(20f* Mathf.Sin(-fullAngle/2f * Mathf.Deg2Rad), 0f,20f* Mathf.Cos(-fullAngle / 2f * Mathf.Deg2Rad));//20f*direction(-fullAngle/2f, true);//new Vector3(0f,0f,1f);//20f*new Vector3(Mathf.Cos(fullAngle * Mathf.Deg2Rad), 0f, Mathf.Sin(fullAngle * Mathf.Deg2Rad));
        vertices[2] = og + DirFromAngle3D(fullAngle / 2f, fullAngle / 2f);//new Vector3(20f*Mathf.Sin(fullAngle / 2f * Mathf.Deg2Rad), 0f, 20f* Mathf.Cos(fullAngle / 2f * Mathf.Deg2Rad));//20f*direction(fullAngle/2f, true);//-20f * new Vector3(Mathf.Cos(fullAngle * Mathf.Deg2Rad), 0f, Mathf.Sin(fullAngle * Mathf.Deg2Rad));

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

    }
  */

    public Vector3 DirFromAngle3D(float theta,float phi)
    {
        float y = Mathf.Cos( theta* Mathf.Deg2Rad);
        float x = y * Mathf.Sin(phi  * Mathf.Deg2Rad);
        float z = y * Mathf.Cos(phi * Mathf.Deg2Rad);
        return radius * new Vector3(x, y, z);
    }

    public Vector3 direction(float angle, bool upDir)
    {
        Vector3 f = Vector3.zero;
        if (!upDir)
        {
            f = transform.forward * Mathf.Cos(angle * Mathf.Deg2Rad) + transform.right * Mathf.Sin(angle * Mathf.Deg2Rad);
        }
        else
            f = transform.forward * Mathf.Cos(angle * Mathf.Deg2Rad) + transform.up * Mathf.Sin(angle * Mathf.Deg2Rad);
       Vector3 fn = f.normalized;
       return fn;
    }
}
