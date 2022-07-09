using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.EventSystems;

public class swipe  : MonoBehaviour , IPointerExitHandler
{
    public Color[] colors;
    public GameObject tabsHandler;
    public GameObject pagesHandler;

    public ScrollRect scrollRect;
    public Scrollbar horizontalScrollbar;
    public Scrollbar verticalScrollbar;
    public List<int> fixedPageIndex = new List<int>(); 

    private List<RectTransform> fixedPages = new List<RectTransform>();
    private List<Vector2> fixedPagesOrigin = new List<Vector2>();
    private int currentPage;
    private float enterHorizontalSliderValue;
    private float enterVecrticalSliderValue;
    private bool touched;

    private void Start()
    {
        currentPage = 3;
        horizontalScrollbar.value = 0.5f;

        foreach (var rectIndex in fixedPageIndex)
        {
            RectTransform rect = pagesHandler.transform.GetChild(rectIndex).GetChild(1).GetComponent<RectTransform>();
            fixedPages.Add(rect);
            fixedPagesOrigin.Add(rect.anchoredPosition);
        }
          
    }

    private void Update()
    {     
        MovefixedPage();

        if (Input.GetMouseButtonDown(0) /*|| Input.touchCount == 1 && touched ==false*/)
        {
            enterHorizontalSliderValue = horizontalScrollbar.value;
            enterVecrticalSliderValue = verticalScrollbar.value;
            touched = true;
            scrollRect.horizontal = true;
            scrollRect.vertical = true;
        }

        if (Input.GetMouseButtonUp(0) /*|| Input.touchCount == 0 && touched == true*/)
        {
           
            if (Mathf.Abs(horizontalScrollbar.value - enterHorizontalSliderValue) > 0.13f)
            {
                if (horizontalScrollbar.value < 0.13f)
                    ChangePage(0);
                else if (horizontalScrollbar.value < 0.33f)
                    ChangePage(1);
                else if (horizontalScrollbar.value < 0.63f)
                    ChangePage(2);
                else if (horizontalScrollbar.value < 0.83f)
                    ChangePage(3);
                else
                    ChangePage(4);
            }
            else
            {
                ChangePage(currentPage);
            }

            touched = false;
        }

        if (touched)
        {
            ScalePageTab();

           // if (Mathf.Abs(verticalScrollbar.value - enterVecrticalSliderValue) > 0)
           // {
           //    // horizontalScrollbar.value = 0.25f * currentPage;
           //     scrollRect.horizontal = false;
           //     scrollRect.vertical = true;
           // }
           // else if (Mathf.Abs(horizontalScrollbar.value - enterHorizontalSliderValue) > 0)
           // {
           //     scrollRect.horizontal = true;
           //     scrollRect.vertical = false;
           // }
           // else
           // {
           //     scrollRect.horizontal = true;
           //     scrollRect.vertical = true;
           // }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Mathf.Abs(horizontalScrollbar.value - enterHorizontalSliderValue) > 0.13f)
        {
            if (horizontalScrollbar.value < 0.13f)
                ChangePage(0);
            else if (horizontalScrollbar.value < 0.33f)
                ChangePage(1);
            else if (horizontalScrollbar.value < 0.63f)
                ChangePage(2);
            else if (horizontalScrollbar.value < 0.83f)
                ChangePage(3);
            else
                ChangePage(4);
        }
        else
        {
            ChangePage(currentPage);
        }

    }


    public void PressButton(Button button)
    {
        for (int i = 0; i < tabsHandler.transform.childCount; i++)
            if(tabsHandler.transform.GetChild(i) == button.transform)
                ChangePage(i);
    }

    public void ChangePage(int pageIndex)
    {
        float scrollBarValue = 0.25f * pageIndex;

        if (fixedPageIndex.Contains(pageIndex))
        {
            scrollRect.vertical = false;          
        }
        else
        {
            scrollRect.vertical = true;
        }

        GameObject currentTabObject = tabsHandler.transform.GetChild(currentPage).gameObject;
        GameObject nextTabObject = tabsHandler.transform.GetChild(pageIndex).gameObject;

        currentTabObject.GetComponent<Image>().color = colors[0];
        nextTabObject.GetComponent<Image>().color = colors[1];

        currentTabObject.GetComponent<MenuTabHook>().arrows.SetActive(false);
        currentTabObject.GetComponent<MenuTabHook>().text.SetActive(false);
        currentTabObject.GetComponent<MenuTabHook>().glowFrame.SetActive(false);
        nextTabObject.GetComponent<MenuTabHook>().arrows.SetActive(true);
        nextTabObject.GetComponent<MenuTabHook>().text.SetActive(true);
        nextTabObject.GetComponent<MenuTabHook>().glowFrame.SetActive(true);

        currentTabObject.GetComponent<MenuTabHook>().icon.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        nextTabObject.GetComponent<MenuTabHook>().icon.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack);

        RectTransform currentPageRect = currentTabObject.GetComponent<RectTransform>();
        RectTransform pageIndexRect = nextTabObject.GetComponent<RectTransform>();



        DOTween.To(() => currentPageRect.sizeDelta, x => currentPageRect.sizeDelta = x, new Vector2(currentPageRect.rect.width, 180), 0.5f).SetEase(Ease.OutBack);
        DOTween.To(() => pageIndexRect.sizeDelta, x => pageIndexRect.sizeDelta = x, new Vector2(pageIndexRect.rect.width, 220), 0.5f).SetEase(Ease.OutBack);

        DOTween.To(() => horizontalScrollbar.value, x => horizontalScrollbar.value = x, scrollBarValue, 0.3f);
        currentPage = pageIndex;
    }

    void ScalePageTab()
    {
        RectTransform currentTabRect = tabsHandler.transform.GetChild(currentPage).gameObject.GetComponent<RectTransform>();
        float currentScaleAmount = Remap(Mathf.Abs(enterHorizontalSliderValue - horizontalScrollbar.value), 0, 0.25f, 220, 180);
        Vector2 currentTabSize = new Vector2(currentTabRect.rect.width, currentScaleAmount);
        currentTabRect.sizeDelta = currentTabSize;
    }
    void MovefixedPage()
    {
        for (int i = 0; i < fixedPages.Count; i++)
        {
            RectTransform rect = fixedPages[i];
            rect.anchoredPosition = new Vector3(0, Remap(verticalScrollbar.value, 0, 1, 0, fixedPagesOrigin[i].y), 0);
        }

    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


}