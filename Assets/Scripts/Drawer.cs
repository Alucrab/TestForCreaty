using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Drawer : MonoBehaviour
{
    [SerializeField]
    private Slider brushSizeSlider;
    [SerializeField] 
    private ColorSelector colorSelector;
    [SerializeField]
    private int brushSize;
    [SerializeField] 
    private Transform sphereTransform;
    [SerializeField]
    private Renderer sphereRenderer;
    [SerializeField] 
    private Texture2D drawTexture;

    private const string FileName = "SavedDrawTexture";

    private int _brushSizeMultiplier = 10;
    private void Start()
    {
        brushSize = (int)brushSizeSlider.value * _brushSizeMultiplier ;
        brushSizeSlider.onValueChanged.AddListener(OnBrushSizeChanged);
        drawTexture = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
        sphereRenderer.material.mainTexture = drawTexture;
    }

    private void OnBrushSizeChanged(float size)
    {
        brushSize = (int)size * _brushSizeMultiplier ;
    }

    private void Update() 
    {
        if (Input.GetMouseButton(0)) 
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      
            if (Physics.Raycast(ray, out hit)) 
            {
                if(hit.transform.CompareTag("DrawingSphere")) 
                {
                    DrawOnSphere(hit.point);
                }
            }
        }
    }

    private void OnDestroy()
    {
        brushSizeSlider.onValueChanged.RemoveListener(OnBrushSizeChanged);
    }

    private void DrawOnSphere(Vector3 hitPoint)
    {
        Vector2 textureCoord = ConvertToTextureCoord(hitPoint);
        textureCoord.x *= drawTexture.width;
        textureCoord.y *= drawTexture.height;
        
        int xStart = Mathf.Clamp((int)(textureCoord.x - brushSize / 2), 0, drawTexture.width);
        int xEnd = Mathf.Clamp((int)(textureCoord.x + brushSize / 2), 0, drawTexture.width);
        int yStart = Mathf.Clamp((int)(textureCoord.y - brushSize / 2), 0, drawTexture.height);
        int yEnd = Mathf.Clamp((int)(textureCoord.y + brushSize / 2), 0, drawTexture.height);
        
        for (int x = xStart; x < xEnd; x++)
        {
            for (int y = yStart; y < yEnd; y++)
            {
                drawTexture.SetPixel(x, y, colorSelector.SelectedColor);
            }
        }
        drawTexture.Apply();
        sphereRenderer.material.mainTexture = drawTexture;
    }   
    
    private Vector2 ConvertToTextureCoord(Vector3 hitPoint)
    {
        Vector3 localPoint = sphereTransform.InverseTransformPoint(hitPoint);
        
        localPoint = localPoint.normalized;
        
        float phi = Mathf.Atan2(localPoint.z, localPoint.x);
        float theta = Mathf.Acos(localPoint.y);
        
        Vector2 uv;
        uv.y = 1.0f - (theta / Mathf.PI);
        uv.x = (phi / (2.0f * Mathf.PI) + 0.5f) % 1f;
        return uv;
    }

    public void SaveProgress()
    {
        byte[] bytes = drawTexture.EncodeToPNG();
        File.WriteAllBytes(FileName, bytes);
    }

    public void LoadProgress()
    {
        byte[] bytes = File.ReadAllBytes(FileName);
        drawTexture.LoadImage(bytes);
        drawTexture.Apply();
        sphereRenderer.material.mainTexture = drawTexture;
    }

    public void Clear()
    {
        Color[] clearPixels = new Color[drawTexture.width * drawTexture.height];
        
        for(int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = Color.white;
        }
        
        drawTexture.SetPixels(clearPixels);
        drawTexture.Apply();
    }
}
