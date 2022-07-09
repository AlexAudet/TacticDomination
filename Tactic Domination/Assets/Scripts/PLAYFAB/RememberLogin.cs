using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RememberLogin : MonoBehaviour
{
    public Toggle remenberToggle;


    void Awake()
    {
        if (remenberToggle.isOn)
        {
            gameObject.GetComponent<TMP_InputField>().text = PlayerPrefs.GetString(gameObject.name);
        }
    }

    public void SaveChange()
    {
        if (remenberToggle.isOn)
        {
            PlayerPrefs.SetString(gameObject.name, transform.GetComponentInChildren<TMP_InputField>().text);
        }
    }
}
