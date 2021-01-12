using UnityEngine;

public class FoVMeshGenScript : MonoBehaviour
{
    public float maxDistance = 10f;
    public LayerMask layerMask;
    public float angleStep = 6f;

    private Mesh _mesh;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        _mesh = meshFilter.mesh;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPosition = transform.parent.position;

        // Will have one mesh vertex around the edge of field of vision (like a circle)
        // But need an extra slot for center and an extra for completing the circle
        Vector3[] vertices = new Vector3[Mathf.CeilToInt(360 / angleStep) + 2];
        // But need to create triangle from the center point
        vertices[0] = Vector3.zero;
        // Generate vertices going around the edge
        for (float angle = 0; angle < 360; angle += angleStep)
        {
            // Raycast from player's position outwards in target direction
            // Starting raycasting to the right, rotate counter clockwise
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(worldPosition, direction, out hit, maxDistance, layerMask))
                vertices[(int) (angle / angleStep) + 1] = direction * hit.distance;
            else
                vertices[(int) (angle / angleStep) + 1] = direction * maxDistance;
        }

        // Complete the circle by duplicating (at the end) the first point in the circle
        vertices[vertices.Length - 1] = vertices[1];

        // Triangles are made using consecutive pair of vertices around the edge
        int[] triangles = new int[(int) (360 / angleStep) * 3];
        for (int i = 2; i < vertices.Length; i++)
        {
            // Make a triangle using the current vertex, previous vertex, and the 0th vertex
            int baseIndex = (i - 2) * 3;
            triangles[baseIndex] = i - 1;
            triangles[baseIndex + 1] = i;
            triangles[baseIndex + 2] = 0;
        }

        // Set normal for each vertex facing upwards
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; i++)
            normals[i] = Vector3.up;

        // Apply changes to mesh
        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.normals = normals;
        _mesh.triangles = triangles;

        // Don't let the generated mesh rotate with the rest of the player
        transform.rotation = Quaternion.identity;
    }

    // Generates a simple square mesh centered on transform.position
    private void GenerateBasicSquareMesh()
    {
        const float squareHalfSideLength = 5f;

        Vector3[] vertices =
        {
            new Vector3(squareHalfSideLength, 0, squareHalfSideLength),
            new Vector3(-squareHalfSideLength, 0, squareHalfSideLength),
            new Vector3(-squareHalfSideLength, 0, -squareHalfSideLength),
            new Vector3(squareHalfSideLength, 0, -squareHalfSideLength)
        };

        int[] triangles =
        {
            0, 2, 1,
            2, 0, 3
        };

        Vector3[] normals =
        {
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up
        };

        _mesh.vertices = vertices;
        _mesh.normals = normals;
        _mesh.triangles = triangles;
    }
}