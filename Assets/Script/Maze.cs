using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;
using UnityEngine;

[SerializeField]
public class MapLocation
{
    public int x;
    public int z;


}



public class Maze : MonoBehaviour
{
    public static Maze instance;

    System.Random rng = new System.Random();
    public List<MapLocation> directions = new List<MapLocation>();
    public int width = 30;
    public int depth = 30;
    public byte[,] map;
    public int scale = 6;

    int generateX = 1;
    int generateZ = 1;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseMap();
        SetMapLocation();
        //GenerateList(5, 5);
        Generate(5, 5);

        DrawMap();
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;

        } else
        {
            Destroy(gameObject);
        }
    }

    void InitialiseMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1; //1은 벽, 0은 통로
            }
        }
    }

    void SetMapLocation()
    {
        //오
        MapLocation location0 = new MapLocation();
        location0.x = 1;
        location0.z = 0;
        //클래스 내 함수로 값 등록
        //location0.SetMapLocation(1,0);
        directions.Add(location0);
        //위
        MapLocation location1 = new MapLocation();
        location1.x = 0;
        location1.z = 1;
        //클래스 내 함수로 값 등록
        //location0.SetMapLocation(0,1);
        directions.Add(location1);
        //왼
        MapLocation location2 = new MapLocation();
        location2.x = -1;
        location2.z = 0;
        //클래스 내 함수로 값 등록
        //location0.SetMapLocation(-1,0);
        directions.Add(location2);
        //아래
        MapLocation location3 = new MapLocation();
        location3.x = 0;
        location3.z = -1;
        //클래스 내 함수로 값 등록
        //location0.SetMapLocation(0,-1);
        directions.Add(location3);
    }


    void GenerateList(int x, int z)
    {
        List<MapLocation> mapDatas = new List<MapLocation>();
        map[x, z] = 0;
        MapLocation mapData = new MapLocation();
        mapData.x = x;
        mapData.z = z;
        mapDatas.Add(mapData);

        while (mapDatas.Count > 0)
        {
            MapLocation current = mapDatas[0];
            Shuffle(directions);
            //이동이 가능한 곳인지 확인
            bool moved = false;

            foreach (MapLocation dir in directions)
            {
                int changeX = current.x + dir.x;
                int changeZ = current.z + dir.z;

                if (!(CountSquareNeighbours(changeX, changeZ) >= 2 || map[changeX, changeZ] == 0))
                {
                    map[changeX, changeZ] = 0;
                    MapLocation tempData = new MapLocation();
                    tempData.x = changeX;
                    tempData.z = changeZ;
                    mapDatas.Insert(0, tempData);
                    moved = true;
                    break;
                }
            }
            if (!moved)
            {
                mapDatas.RemoveAt(0);
            }
        }

    }

    void Generate(int x, int z)
    {
        Stack<Vector2Int> mapDatas = new Stack<Vector2Int>();
        int startX = x;
        int StartZ = z;
        map[startX, StartZ] = 0;
        mapDatas.Push(new Vector2Int(startX, StartZ));

        while (mapDatas.Count > 0)
        {
            Vector2Int current = mapDatas.Peek();
            Shuffle(directions);
            //이동이 가능한 곳인지 확인
            bool moved = false;

            foreach (MapLocation dir in directions)
            {
                int changeX = current.x + dir.x;
                int changeZ = current.y + dir.z;

                if (!(CountSquareNeighbours(changeX, changeZ) >= 2 || map[changeX, changeZ] == 0))
                {
                    map[changeX, changeZ] = 0;
                    mapDatas.Push(new Vector2Int(changeX, changeZ));
                    moved = true;
                    break;
                }
            }
            if (!moved)
            {
                mapDatas.Pop();
            }
        }

    }


    public void Shuffle(List<MapLocation> mapLocations)
    {
        int n = mapLocations.Count;
        //리스트의 크기만큼 반복
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            MapLocation value = mapLocations[k];
            mapLocations[k] = mapLocations[n];
            mapLocations[n] = value;
        }
    }

    void DrawMap()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.localScale = new Vector3(scale, scale, scale);
                    wall.transform.position = pos;
                }
            }
        }
    }

    /// <summary>
    ///4방위 복도를 검색한다. 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;
    }

}