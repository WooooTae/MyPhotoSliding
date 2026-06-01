using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageLoader : MonoBehaviour
{
    public static ImageLoader Instance
    {
        get;
        private set;
    }

    public Texture2D selectedTexture;
    public Texture2D defaultTexture;

    public Texture2D GetTexture()
    {
        return selectedTexture != null
            ? selectedTexture
            : defaultTexture;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public void LoadImageFromGallery()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 1024);
                if (texture != null)
                {
                    selectedTexture = texture;
                    GameManager.Instance.ApplyTexture(selectedTexture);
                }
            }
        }, "Select a Puzzle Image");
    }
}
