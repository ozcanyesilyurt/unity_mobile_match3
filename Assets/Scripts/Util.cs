using UnityEngine;
using UnityEngine.UI;

public static class Util
{
    public static T GetRandomInArray<T>(T[] array)
    {
        int index = Random.Range(0, array.Length);
        return array[index];
    }
    public static T GetRepeatingElement<T>(T[] array, int index)
    {
        return array[index % array.Length];
    }
    public static void ObjectToGameObject(IPoolable obj, GameObject prefab, Level level)
    {
        Sprite spriteToUse = null;
        if (obj is Obstacle obstacle)
        {
            spriteToUse = obstacle.sprite;
        }
        else if (obj is Tile tile)
        {
            spriteToUse = tile.sprite;
        }
        else if (obj is Background background)
        {
            spriteToUse = background.sprite;
        }
        if (spriteToUse != null)
        {
            prefab.GetComponent<Image>().sprite = spriteToUse;
        }
        prefab.GetComponent<RectTransform>().sizeDelta = new Vector2(level.cellSizeX, level.cellSizeY);
    }

}
