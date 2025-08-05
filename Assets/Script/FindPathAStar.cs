using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class PathMarker
{
    public MapLocation location;
    public float G;
    public float H;
    public float F;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(MapLocation I, float g, float h, float f, GameObject marker, PathMarker p)
    {
        location = I;
        G = g;
        H = h;
        F = f;
        this.marker = marker;
        parent = p;
    }



    public bool returnreturnEquals(PathMarker obj)
    {
        Debug.Log(((PathMarker)obj).location.x + " : " + ((PathMarker)obj).location.z);
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return location.x == ((PathMarker)obj).location.x && location.z == ((PathMarker)obj).location.z;
        }
    }

    public bool returnreturnEquals(MapLocation obj)
    {
        Debug.Log(obj.x + " : " + obj.z);
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return location.x == obj.x && location.z == obj.z;
        }
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
public class FindPathAStar : MonoBehaviour
{
    public Maze maze;
    public Material closeMateral;
    public Material openMateral;

    public List<PathMarker> open = new List<PathMarker>();
    public List<PathMarker> closed = new List<PathMarker>();

    public GameObject start;
    public GameObject end;
    public GameObject pathP;

    PathMarker goalNode;
    PathMarker startNode;

    PathMarker lastPos;
    bool done = false;

    GameObject Unit;
    bool isMove = false;
    public List<Vector3> movePath = new List<Vector3>();

    void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (GameObject m in markers)
        {
            Destroy(m);
        }
    }

    void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();

        List<MapLocation> locations = new List<MapLocation>();
        for (int z = 1; z < maze.depth - 1; z++)
        {
            for (int x = 1; x < maze.width - 1; x++)
            {
                if (maze.map[x, z] != 1)
                {
                    MapLocation mapLocation = new MapLocation();
                    mapLocation.x = x;
                    mapLocation.z = z;
                    locations.Add(mapLocation);
                }
            }
        }


        Maze.instance.Shuffle(locations);

        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale);

        MapLocation Startnode = new MapLocation();
        Startnode.x = locations[0].x;
        Startnode.z = locations[0].z;

        startNode = new PathMarker(Startnode, 0, 0, 0, Instantiate(start, startLocation, Quaternion.identity), null);


        MapLocation Goalnode = new MapLocation();
        Goalnode.x = locations[1].x;
        Goalnode.z = locations[1].z;

        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        goalNode = new PathMarker(Goalnode, 0, 0, 0, Instantiate(end, goalLocation, Quaternion.identity), null);

        open.Clear();
        closed.Clear();

        open.Add(startNode);
        lastPos = startNode;

    }

    void Search(PathMarker thisNode)
    {
        if (thisNode.returnreturnEquals(goalNode))
        {
            done = true;
            return;
        }

        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbor = new MapLocation();

            neighbor.x = dir.x + thisNode.location.x;
            neighbor.z = dir.z + thisNode.location.z;


            if (maze.map[neighbor.x, neighbor.z] == 1)
            {
                continue;
            }

            if (neighbor.x < 1 || neighbor.x >= maze.width || neighbor.z < 1 || neighbor.z >= maze.depth)
            {
                continue;
            }

            if (IsClosed(neighbor))
            {
                continue;
            }


            Vector2 thisNodeVector = new Vector2(thisNode.location.x, thisNode.location.z);
            Vector2 neigborVector = new Vector2(neighbor.x, neighbor.z);
            Vector2 goalNodeVector = new Vector2(goalNode.location.x, goalNode.location.z);

            float G = Vector2.Distance(thisNodeVector, neigborVector) + thisNode.G;
            float H = Vector2.Distance(neigborVector, goalNodeVector);
            float F = G + H;
            GameObject pathBlock = Instantiate(pathP, new Vector3(neighbor.x * maze.scale, 0, neighbor.z * maze.scale), Quaternion.identity);

            TextMesh[] values = pathBlock.GetComponentsInChildren<TextMesh>();
            values[0].text = "G : " + G.ToString("0.00");
            values[1].text = "H : " + H.ToString("0.00");
            values[2].text = "F : " + F.ToString("0.00");

            if (!UpdateMarker(neighbor, G, H, F, thisNode))
            {
                open.Add(new PathMarker(neighbor, G, H, F, pathBlock, thisNode));
            }

        }

        open = open.OrderBy(p => p.F).ToList<PathMarker>();

        PathMarker pm = (PathMarker)open.ElementAt(0);

        closed.Add(pm);

        open.RemoveAt(0);

        pm.marker.GetComponent<Renderer>().material = closeMateral;

        lastPos = pm;
    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    {
        foreach (PathMarker p in open)
        {
            if (p.returnreturnEquals(pos))
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }

        return false;
    }

    bool IsClosed(MapLocation marker)
    {
        foreach (PathMarker p in closed)
        {
            if (p.returnreturnEquals(marker))
            {
                return true;
            }
        }
        return false;
    }

    void GetPath()
    {
        RemoveAllMarkers();
        PathMarker begin = lastPos;

        while (!startNode.returnreturnEquals(begin) && begin != null)
        {
            Instantiate(pathP, new Vector3(begin.location.x * maze.scale, 0, begin.location.z * maze.scale), Quaternion.identity);
            begin = begin.parent;
        }

        Instantiate(pathP, new Vector3(begin.location.x * maze.scale, 0, begin.location.z * maze.scale), Quaternion.identity);


    }

    void SetMvoePath()
    {
        PathMarker begin = lastPos;
        while (!startNode.returnreturnEquals(begin) && begin != null)
        {
            movePath.Add(new Vector3(begin.location.x * maze.scale, 0, begin.location.z * maze.scale));
            begin = begin.parent;
        }

        movePath.Add(new Vector3(startNode.location.x * maze.scale, 0, startNode.location.z * maze.scale));
        movePath.Reverse();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            BeginSearch();
        }

        if (Input.GetKeyDown(KeyCode.C) && !done)
        {
            Search(lastPos);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            GetPath();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetMvoePath();
            Unit = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Unit.transform.position = movePath[0];
            isMove = true;
        }

        if (isMove)
        {
            if (movePath.Count > 0)
            {
                if (Vector3.Distance(Unit.transform.position, movePath[0]) > 0.01f)
                {
                    Unit.transform.position = Vector3.MoveTowards(Unit.transform.position, movePath[0], 1.0f * maze.scale * Time.deltaTime);
                }
                else
                {
                    Debug.Log("test");
                    Unit.transform.position = movePath[0];
                    movePath.RemoveAt(0);
                }
            }
            else
            {
                isMove = false;
            }
        }
    }
}

