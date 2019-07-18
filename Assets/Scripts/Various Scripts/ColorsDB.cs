using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorsDB : MonoBehaviour {
    public static ColorsDB instance = null;

    public ColorData[] colors;
    public ColorData RandomColor { get { return colors[Random.Range(0, colors.Length)]; } }

    public Texture2D colorPalette;
    public ColorData[] test;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        //GetColorsFromTexture();
    }

    public string GetColorDataByVector(float x, float y) {
        float eps = 0.05f;
        for (int i = 0; i < colors.Length; i++) {
            if (x == 0 && y == 0) {
                return colors[0].name;
            }
            if ((colors[i].textureCoordinates.x + eps > x && colors[i].textureCoordinates.x - eps < x)
                &&
                (colors[i].textureCoordinates.y + eps > y && colors[i].textureCoordinates.y - eps < y))
            {
                //Debug.Log(colors[i].name + x + y + colors[i].textureCoordinates.x + colors[i].textureCoordinates.y);
                return colors[i].name;
            }
        }
        return null;
    }

    public void GetColorsFromTexture() {

        Color[] c= colorPalette.GetPixels();
        test = new ColorData[100];
        int test_count = 0;
        Color color1, color2;
        for (int i = 10; i < c.Length; i+= c.Length / 10) {
            //color1 = colorPalette.GetPixel(c.Length - i, c.Length - i);
            for (int j = 10; j < c.Length; j+= c.Length / 10) {
                color2 = colorPalette.GetPixel(c.Length - i, c.Length - j);
                //if (color2 != color1) {

                    test[test_count].colorExample = color2;
                    test[test_count].textureCoordinates = new Vector2(c.Length - i, c.Length - j);
                    test_count++;
                //}
            }
        }
        //testC.colorExample = colorPalette.GetPixel(c.Length - 24, c.Length - 24);
        //testC.textureCoordinates = new Vector2(c.Length - 24, c.Length - 24);
        //testC2.colorExample = colorPalette.GetPixel(c.Length - 25, c.Length - 25);
        //testC2.textureCoordinates = new Vector2(c.Length - 25, c.Length - 25);
        //testC3.colorExample = colorPalette.GetPixel(c.Length - 26, c.Length - 26);
        //testC3.textureCoordinates = new Vector2(c.Length - 26, c.Length - 26);
        //testC4.colorExample = colorPalette.GetPixel(c.Length - 27, c.Length - 27);
        //testC4.textureCoordinates = new Vector2(c.Length - 27, c.Length - 27);
        //testC5.colorExample = colorPalette.GetPixel(c.Length - 28, c.Length - 28);
        //testC5.textureCoordinates = new Vector2(c.Length - 28, c.Length - 28);
        //Color c2 = colorPalette.GetPixel(10, 10);
    }
}

[System.Serializable]
public struct ColorData
{
    public string name;
    public Color colorExample;
    public Vector2 textureCoordinates;
}
