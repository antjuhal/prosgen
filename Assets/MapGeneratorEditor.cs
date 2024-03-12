using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator)), CanEditMultipleObjects]
public class MapGeneratorEditor : Editor
{
    /// <summary>
    /// This is just for customizing the Inspector window in use. Feel free to modify further but
    /// it probably isn't necessary.
    /// </summary>
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        if(GUILayout.Button ("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}
