using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{

    //Tilemap the stores the underlying "what kind of tile is here?" information
    public Tilemap placeholderMap;
    //Shows the display based on the dual grid sprite alignment based on 4 placeholders
    public Tilemap displayMap;
    // Holds information about what entities (objects, units, crops) are going to be present at a given tile
    public Tilemap entitiesMap;

    
}
