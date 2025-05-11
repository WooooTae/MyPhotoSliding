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
