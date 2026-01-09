using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;


public class maze_gen : MonoBehaviour
{

    public Tilemap tilemap;
    public Tile[] tiles;
    public int width = 250;
    public int height = 250;
    public int scale = 6;
    public Vector3Int start_pos = new Vector3Int(15, 15, 0);
    public int seed = 1;
    public Enemy_Manager enemy_Manager;
    // public Vector3Int start_pos = new Vector3Int(width/2,height/2, 0); TODO check if this works with overrides
    Vector3Int[] positions = new Vector3Int[] {
      new Vector3Int( 2, 0, 0 ),
      new Vector3Int(-2, 0, 0),
      new Vector3Int(0,  2, 0),
      new Vector3Int(0, -2, 0)
    };

    void Start()
    {
        enemy_Manager = GetComponentInParent<Enemy_Manager>();
        Random.InitState(seed);
        Debug.Log("seed: " + seed + "\nwidth:" + width + "\nheight: " + height + "\nscale: " + scale);
        Debug.Log(tilemap.GetTile(new Vector3Int(0, 0, 0)));
        // tilemap.BoxFill(new Vector3Int(1, 1, 0), tiles[1], 0, 0, 250, 250);
        // tilemap.FloodFill(new Vector3Int(1, 1, 0), tiles[1]);
        for (int x = -10; x < width+4; x++)
        {
            for (int y = -10; y < height+4; y++)
            {
                if(tilemap.GetTile(new Vector3Int(x, y, 0)) == null){
                    
                  tilemap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                }
            }
        }
        for (int x = start_pos.x-10; x < start_pos.x; x++)
        {
            for (int y = start_pos.y-10; y < start_pos.y; y++)
            {
                // Randomly select a tile from the array
                Tile tile = tiles[0];
                if (tilemap.GetTile(new Vector3Int(x, y, 0)) == null && (x == start_pos.x-10 || y == start_pos.y-10 || x+1 == start_pos.x || y+1 == start_pos.y) ) //&& !(x+2 == start_pos.x && y+1  == start_pos.y))
                {
                    tilemap.SetTile(new Vector3Int(x + 7, y + 5, 0), tiles[2]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x + 7, y + 5, 0), tiles[0]);
                }
            }
        }
        enemy_Manager.SetPositions(walk());
        enemy_Manager.setup();

    }

    ArrayList walk()
    {
        ArrayList possible_spawn_positions = new();
        
        Tile tile = tiles[0];
        Vector3Int next_step = Vector3Int.up;
        Vector3Int curr_pos = start_pos;
        Vector3Int next_pos = (next_step * scale) + curr_pos;
        Debug.Log(next_pos);
        ArrayList somelist = new();
        // LinkedList<Vector3Int> somelist = new();
        // somelist.AddFirst(next_pos);
        somelist.Add(next_pos);

        tilemap.SetTile(curr_pos, tile);
        tilemap.SetTile(curr_pos + Vector3Int.left*2, tile);
        tilemap.SetTile(curr_pos + Vector3Int.left, tile);
        tilemap.SetTile(curr_pos + Vector3Int.right, tile);
        tilemap.SetTile(curr_pos + Vector3Int.right*2, tile);
        int count = 0;
        while (somelist.Count > 0)
        {
            //set tile
            for (int i = 0; i <= scale; i++)

            {
                curr_pos += next_step;
                tilemap.SetTile(curr_pos, tile);
                Vector3Int side_1 = next_step.x == 0 ? Vector3Int.left : Vector3Int.up;
                Vector3Int side_2 = next_step.x == 0 ? Vector3Int.right : Vector3Int.down;
                tilemap.SetTile(curr_pos + side_1 + side_1, tile);
                tilemap.SetTile(curr_pos + side_1, tile);
                tilemap.SetTile(curr_pos + side_2, tile);
                tilemap.SetTile(curr_pos + side_2 + side_2, tile);

            }

            curr_pos -= next_step;

            // curr_pos += next_step;
            // tilemap.SetTile(curr_pos, tile);
            // tilemap.SetTile(curr_pos + Vector3Int.left, tile);
            // tilemap.SetTile(curr_pos + Vector3Int.right, tile);
            //get next pos
            next_step = get_next_step(curr_pos);
            if (next_step == Vector3Int.zero){
              possible_spawn_positions.Add(curr_pos);
            }
            //backtrack
            while (next_step == Vector3Int.zero)
            {
                if (somelist.Count == 0)
                {
                    return possible_spawn_positions;
                }
                int index = Random.Range(0, somelist.Count);
                curr_pos = (Vector3Int)somelist[index];
                somelist.RemoveAt(index);
                next_step = get_next_step(curr_pos);
            }
            somelist.Add((next_step * scale) + curr_pos);
            count++;
        }
      return possible_spawn_positions;
    }

    bool IsInsideBox(Vector3Int pos)
    {
        // Example: box from start_pos to start_pos + (9,9)
        return pos.x >= 0 &&
               pos.x < width &&
               pos.y >= 0 &&
               pos.y < height;
    }

    Vector3Int get_next_step(Vector3Int curr_pos)
    {

        Vector3Int[] steps = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
        int index = Random.Range(0, 3);
        Vector3Int step = steps[index];
        Vector3Int next_pos = (step * scale) + curr_pos;

        if (tilemap.GetTile(next_pos) != tiles[1] || !IsInsideBox(next_pos))
        {
            for (int i = 1; i <= 3; i++)
            {
                step = steps[(i + index) % 4];
                next_pos = (step * scale) + curr_pos;
                if (tilemap.GetTile(next_pos) == tiles[1] && IsInsideBox(next_pos))
                {
                    break;
                }
                step = Vector3Int.zero;
            }
        }
        return step;
    }

    
    public TileBase targetTile;  // the tile type to delete

     public void RemoveAllMatchingTiles()
    {
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);

            if (tile == targetTile)
            {
                tilemap.SetTile(pos, null); // delete tile
            }
        }
    }

  


}
