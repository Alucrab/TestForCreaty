using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    [SerializeField] 
    private Button[] buttons;
    
    public Color SelectedColor = Color.red;
    void Start()
    {
        foreach (var button in buttons)
        {
            button.onClick.AddListener((() =>
            {
                ColorSelected(button);
            }));
        }
    }

    private void ColorSelected(Button self)
    {
        SelectedColor = self.image.color;
    }

    private void OnDestroy()
    {
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
