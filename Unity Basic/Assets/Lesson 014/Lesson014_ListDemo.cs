using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson014_ListDemo : MonoBehaviour
{
    public RectTransform itemTemplate;
    public RectTransform Content;
    public int AddItemCount = 5;

    // Start is called before the first frame update
    void Start()
    {
        if (itemTemplate) {
            itemTemplate.gameObject.SetActive(false);
            var width  = itemTemplate.sizeDelta.x;
            var height = itemTemplate.sizeDelta.y;

            var totalHeight = AddItemCount * height;
            var offset = totalHeight / 2;

            Debug.Log($"height = {height}");

            if (Content) {
                for (int i = 0; i < AddItemCount; i++) {
                    var item = Object.Instantiate(itemTemplate);
                    item.transform.SetParent(Content.transform);
//                  item.transform.localPosition = new Vector3(0, offset - i * height, 0); // will be done by AutoLayout
                    item.gameObject.SetActive(true);
                }

//                Content.sizeDelta = new Vector2(width, totalHeight); // will be done by AutoLayout

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
