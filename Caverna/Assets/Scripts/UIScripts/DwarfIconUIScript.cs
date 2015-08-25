using System;
using UnityEngine;
using UnityEngine.UI;

public class DwarfIconUIScript : MonoBehaviour
{
    public GameObject ResourceText;

    private int _weaponLevel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        ResourceText.GetComponent<Text>().text = _weaponLevel.ToString();
        ResourceText.transform.SetParent(transform);
        ResourceText.SetActive(_weaponLevel > 0);
        //ResourceText.transform.position = new Vector3(5, 5);//111 transform.position;
	}

    public void SetUsed(string isUsed)
    {
        bool _isUsed = Boolean.Parse(isUsed);
        if (!_isUsed)
            GetComponent<Image>().canvasRenderer.SetColor(Color.white);
        else
        {
            GetComponent<Image>().canvasRenderer.SetColor(new Color(0.25f, 0.25f, 0.25f));
        }
    }

    public void SetWeaponLevel(string weaponLevel)
    {
        _weaponLevel = Int32.Parse(weaponLevel);
    }
}
