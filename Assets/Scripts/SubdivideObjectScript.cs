using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubdivideObjectScript : MonoBehaviour, IDamageable
{
    public GameObject subdivisionCube; // 1x1x1 prefab
    private Vector3 myDimensions = new Vector3(1f, 1f, 1f);
    private int numberOfSubdivisions = 4;
    private Vector3 subdivisionSize;
    private Color _myColor;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        // myDimensions = new Vector3(1f,1f,1f); //transform.localScale;
        // Debug.Log("transform.localScale: " + transform.localScale);
        // Debug.Log("myDimensions " + myDimensions);
        subdivisionSize = myDimensions / numberOfSubdivisions;
        // Debug.Log("subdivisionSize " + subdivisionSize.ToString("F4"));
        _myColor = GetComponent<Renderer>().material.color;
    }

    public void TakeDamage(int damage)
    {
        // any amount of damage causes subdivision
        SubdivideMe();
    }
    public List<GameObject> SubdivideMe()
    {
        // Debug.Log("     SubdivideMe()");
        // Debug.Log("transform.localScale: " + transform.localScale);
        // myDimensions = new Vector3(1, 1, 1);
        // Debug.Log("numberOfSubdivisions " + numberOfSubdivisions);
        // Debug.Log("myDimensions " + myDimensions);
        Vector3 subdivisionSize = myDimensions / (float)numberOfSubdivisions;
        // Debug.Log("subdivisionSize " + subdivisionSize.ToString("F4"));
        int arrayLength = (int)Mathf.Pow(numberOfSubdivisions, 3);
        // Create a list  
        List<GameObject> subdivisionsList = new List<GameObject>();

        for (int x = 0; x < numberOfSubdivisions; x++)
        {
            for (int y = 0; y < numberOfSubdivisions; y++)
            {
                for (int z = 0; z < numberOfSubdivisions; z++)
                {
                    if (Random.Range(0, 4) == 0)
                    {
                        // Create subdivision at x,y,z coordinate
                        float xCoord = transform.position.x - myDimensions.x * 0.5f + subdivisionSize.x * x + subdivisionSize.x * 0.5f;
                        float yCoord = transform.position.y - myDimensions.y * 0.5f + subdivisionSize.y * y + subdivisionSize.y * 0.5f;
                        float zCoord = transform.position.z - myDimensions.z * 0.5f + subdivisionSize.z * z + subdivisionSize.z * 0.5f;
                        // Instantiate subdivision.
                        GameObject subdivision = Instantiate(subdivisionCube, new Vector3(xCoord, yCoord, zCoord), Quaternion.identity);
                        Color color = Random.Range(0, 4) == 0 ? _myColor * 0.85f : _myColor;
                        if (Random.Range(0, 2) == 0)
                        {
                            Renderer m_Renderer = subdivision.GetComponent<Renderer>();
                            m_Renderer.material.color = _myColor;
                        }
                        subdivision.transform.localScale = subdivisionSize;
                        subdivisionsList.Add(subdivision);
                        // Debug.Log("subdivision.parent ==> "+subdivision.transform.root);
                    }
                }
            }
        }
        Destroy(gameObject);
        return subdivisionsList; //.ToArray(); ;
    }
}
