using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContentGenerator:MonoBehaviour
{
    public GameObject SmallObject;

    List<MapGenerator.Coord> currentObjectCoords = new List<MapGenerator.Coord>();
    private List<GameObject> currentObjects=new List<GameObject>();
    public float doorsMargin;

    public void GenerateEntranceAndExit(List<MapGenerator.Room> rooms,int width,int height,int [,] map)
    {
        DeleteCurrentObjects();

        var leftMostDoor = new MapGenerator.Coord(width, 0);
        var rightMostDoor = new MapGenerator.Coord(-width, 0);
        var topMostDoor = new MapGenerator.Coord(0 ,-height);
        var downMostDoor = new MapGenerator.Coord(0, height);
        
        for (int roomIndex = 0; roomIndex < rooms.Count ; roomIndex++)
        {
            foreach (var coord in rooms[roomIndex].tiles)
            {
                if (coord.tileX < leftMostDoor.tileX && IsTileInRange(coord.tileY,height/2,5) )
                    leftMostDoor= coord;
                
                if (coord.tileX > rightMostDoor.tileX  && IsTileInRange(coord.tileY,height/2,5) )
                    rightMostDoor = coord;
                
                if (coord.tileY > topMostDoor.tileY && IsTileInRange(coord.tileX,width/2,5) )
                    topMostDoor= coord;
                
                if (coord.tileY < downMostDoor.tileY  && IsTileInRange(coord.tileX,width/2,5) )
                    downMostDoor = coord;
            }
        }

        Vector3 posRight = new Vector3((-width / 2 + rightMostDoor.tileX * 1 + 1 / 2)-doorsMargin, 0,
            -height / 2 + rightMostDoor.tileY * 1 + 1 / 2);    
        currentObjects.Add(Instantiate(SmallObject, posRight, Quaternion.identity));
        
        Vector3 posLeft = new Vector3((-width / 2 + leftMostDoor.tileX * 1 + 1 / 2)+doorsMargin, 0,
            -height / 2 + leftMostDoor.tileY * 1 + 1 / 2);  
        currentObjects.Add(Instantiate(SmallObject, posLeft, Quaternion.identity));
        
        Vector3 posTop = new Vector3(-width / 2 + topMostDoor.tileX * 1 + 1 / 2, 0,
            (-height / 2 + topMostDoor.tileY * 1 + 1 / 2)-doorsMargin);
        currentObjects.Add(Instantiate(SmallObject, posTop, Quaternion.identity));
        
        Vector3 posDown = new Vector3(-width / 2 + downMostDoor.tileX * 1 + 1 / 2, 0,
            (-height / 2 + downMostDoor.tileY * 1 + 1 / 2)+doorsMargin);
        currentObjects.Add(Instantiate(SmallObject, posDown, Quaternion.identity));

    }

    bool IsTileInRange(int tileCoord, int thePoint,int range)
    {
        return thePoint - range <= tileCoord && tileCoord <= thePoint + range;
    }

    void DeleteCurrentObjects()
    {
        foreach (var currentObject in currentObjects)
        {
            Destroy(currentObject);
        }
    }
    public IEnumerator MakeARandomObject(List<MapGenerator.Room> rooms,int width,int height,int [,] map)
    {
        DeleteCurrentObjects();

        //put one object in each room
        for (var index = 0; index < rooms.Count; index++)
        {
            var room = rooms[index];

            var numberOfObjects = room.roomSize / 50;
            print($"room size: {room.roomSize} , creating {numberOfObjects} objects");
         
            for (int j = 0; j < numberOfObjects; j++)
            {
                var rng=new System.Random(j+(int)DateTime.Now.Ticks);
                var rand = rng.Next(0, room.tiles.Count);
                
                var randomTile = room.tiles[rand];
                var i = 0;
                var wallCheckRadius = 2;
                var objCheckRadius = 2;
                
                var isSurrondingWallFree = IsSurrondingCircleFree(randomTile, wallCheckRadius,width,height,map);
                var isSurrondingObject = IsSurrondingCircleContainsObject(randomTile, objCheckRadius);
                
                do
                {
                    
                    rng = new System.Random(i + j+(int)DateTime.Now.Ticks);
                    rand = rng.Next(0, room.tiles.Count);
                    /*print($"random: {rand}");*/
                    randomTile = room.tiles[rand];
                    i++;
                    isSurrondingWallFree = IsSurrondingCircleFree(randomTile, wallCheckRadius,width,height,map);
                    
                    isSurrondingObject = IsSurrondingCircleContainsObject(randomTile, objCheckRadius);
                        
                } while ((!isSurrondingWallFree || isSurrondingObject) && i < 10 );
                
                if (i == 10)
                {
                    Debug.LogError("make is canceled!");
                    continue;
                }

                Vector3 pos = new Vector3(-width / 2 + randomTile.tileX * 1 + 1 / 2, 0,
                    -height / 2 + randomTile.tileY * 1 + 1 / 2);
                currentObjectCoords.Add(randomTile);
                currentObjects.Add(Instantiate(SmallObject, pos, Quaternion.identity));
                
                yield return new WaitForSeconds(.1f);
            }
        }
    }

    bool IsSurrondingCircleContainsObject(MapGenerator.Coord c, int r)
    {
        if (currentObjectCoords.Any(o =>
            o.tileX - r <= c.tileX && c.tileX <= o.tileX + r && o.tileY - r <= c.tileY && c.tileY <= o.tileY + r))
        {
            return true;
        }
        
        return false;
    }
    
    bool IsSurrondingCircleFree(MapGenerator.Coord c, int r,int width,int height,int[,] map)
    {
        for (int i = c.tileX - r; i <= c.tileX + r; i++)
        {
            for (int j = c.tileY - r; j <= c.tileY + r; j++)
            {
                if (i >= 0 && i <= width && j >= 0 && j <= height)
                    if (map[i, j] == 1)
                        return false;
            }
        }
        return true;
    }
}
