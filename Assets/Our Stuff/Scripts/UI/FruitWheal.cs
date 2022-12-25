using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class FruitWheal : MonoBehaviour
{
    // Reference to the canvas object
    [SerializeField] Canvas canvas;

    // Array of images to be displayed on the wheel
    [SerializeField] Sprite[] fruitImages;

    // Radius of the wheel
    [SerializeField] float _radius;

    // Thickness of the lines between images
    [SerializeField]private float _lineThickness;



    [ContextMenu("Start Event Again")]
    void Start()
    {
        // Calculate the angle between each image
        int numImages = fruitImages.Length;
        float angleStep = 360f / numImages;

        // Create a new empty game object to hold all of the wheel elements
        GameObject wheel = new GameObject("Wheel");
        wheel.SetActive(false);
        wheel.transform.SetParent(canvas.transform, false);

        // Create a background image for the wheel
        GameObject backgroundImage = new GameObject("Background", typeof(Image));
        backgroundImage.transform.SetParent(wheel.transform, false);
        Image backgroundComponent = backgroundImage.GetComponent<Image>();
        backgroundComponent.color = new Color(0.9f, 0.9f, 0.9f);
        backgroundComponent.rectTransform.sizeDelta = new Vector2(_radius * 2, _radius * 2);

        // Create an image game object for each image in the array
        for (int i = 0; i < numImages; i++)
        {
            // Create a new image game object
            GameObject image = new GameObject("Image", typeof(Image));

            // Set the parent of the image to the wheel game object
            image.transform.SetParent(wheel.transform, false);

            // Set the position of the image
            float angle = angleStep * i;
            float x = _radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = _radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            image.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            // Set the image sprite and size
            Image imageComponent = image.GetComponent<Image>();
            imageComponent.sprite = fruitImages[i];
            imageComponent.rectTransform.sizeDelta = new Vector2(_radius * 2 / 3, _radius * 2 / 3);

            // Create a line between the center of the wheel and the current image
            GameObject line = new GameObject("Line", typeof(Image));
            line.transform.SetParent(wheel.transform, false);
            Image lineComponent = line.GetComponent<Image>();
            lineComponent.color = Color.black;
            lineComponent.rectTransform.sizeDelta = new Vector2(_lineThickness, Vector2.Distance(Vector2.zero, new Vector2(x, y)));
            lineComponent.rectTransform.pivot = new Vector2(0.5f, 0);
            lineComponent.rectTransform.anchoredPosition = Vector2.zero;
            lineComponent.rectTransform.rotation = Quaternion.Euler(0, 0, angle + 90);

            ActivateUI.UIGameObject = wheel;

        }





    }
}

        
