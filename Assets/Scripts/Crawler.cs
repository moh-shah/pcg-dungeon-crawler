using System.Collections.Generic;
using UnityEngine;

public class Crawler : MonoBehaviour
{
    public GameObject dungeon;
    public int dungeonSize;
    public float rightP, leftP, upP, downP;

    public int roomNumber;
    public int maxX, maxY;
    
    private Vector2 _current;
    private int _currentRoomNumber;
    private List<Vector2> _dungeonCoords=new List<Vector2>();

    private List<GameObject> _currentDungeons=new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
       DoTheThing();
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            DoTheThing();
    }

    void DoTheThing()
    {
        _dungeonCoords.Clear();
        foreach (var d in _currentDungeons)
        {
            Destroy(d);
        }

        _currentRoomNumber = roomNumber;
        
        _current=new Vector2(0,0);
        _dungeonCoords.Add(_current);
        
        while (_currentRoomNumber>0)
        {
            Crawl();
        }
        
        
        foreach (var d in _dungeonCoords)
        {
            var pos = new Vector3(d.x * dungeonSize, 0, d.y * dungeonSize);
            var newDungeon = Instantiate(dungeon, pos, Quaternion.identity);
            newDungeon.GetComponent<MapGenerator>().seed = d.x + d.y + "";
            newDungeon.GetComponent<MapGenerator>().GenerateMap();
            _currentDungeons.Add(newDungeon);
         
        }
    }
    
    
    void Crawl()
    {
        var rnd = Random.Range(0, 100);
        if (0 <= rnd && rnd < rightP)
            _current.x += 1;
        if (rightP <= rnd && rnd < leftP)
            _current.x -= 1;
        if (leftP <= rnd && rnd < upP)
            _current.y += 1;
        if (upP <= rnd && rnd < downP)
            _current.y -= 1;
        AddToDungeons();
    }

    void AddToDungeons()
    {
        if (!_dungeonCoords.Contains(_current))
        {
            _dungeonCoords.Add(_current);
            _currentRoomNumber--;
        }
    }
    
}
