using UnityEngine;
using UnityEngine.UI;





/*
    TODO:
    1. Oyun cerceve
    2. Power up tile
    3. Skor
    4. Ses
    5. Animasyon - Yok olma animasyonu (eslestirince)
    6. Istatistik arastirmasi - kim oynuyor hangi turu. Konsepti degistir.
    7. Parlama efekti isik candy crush
    8. menu kullanimlarini benzet - royal match
    9. ikonlar 3d - royal match
    ++10. tablo boyutu ekran boyutuna gore buyusun
    ++11. arkaplan resmi ekran boyutuna uyumlu olsun
    12. Tutorial level 1.
    13. GENEL - Ana menu, oyunu durdurma pause, level baslangic, level sonu efektleri
    14. Ana menu guclendirmeler, giyindirme, puanlari HARCAMA. ANA MENU INTERAKSYON KARAKTER ORTADA NEFES ALIYOR
    15. Hesaplari olacak. Bunu ogren.
    16. Ses ac kapa

 */
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
