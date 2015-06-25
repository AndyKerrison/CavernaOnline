using System;
using System.Globalization;
using UnityEngine;

namespace Assets.UIScripts
{
    public class DwarfGameObject : MonoBehaviour
    {
        private GameObject _resourceTextMesh;

        private SpriteRenderer _renderer;
        private bool _isUsed;
        private bool _init;
        private string _weaponLevel;

        // Use this for initialization
// ReSharper disable once UnusedMember.Local
        void Start ()
        {
            _renderer = gameObject.GetComponent<SpriteRenderer>();
        }
	
        // Update is called once per frame
// ReSharper disable once UnusedMember.Local
        void Update () {
            if (!_init)
            {
                float x = gameObject.transform.position.x;
                float y = gameObject.transform.position.y;
                //esourceTextMesh = (GameObject)Instantiate(Resources.Load("ResourceTextMesh"));
                //_resourceTextMesh.GetComponent<TextMesh>().text = "TEST";
                //_resourceTextMesh.transform.parent = transform;
                //_resourceTextMesh.transform.position = new Vector2(x-0.1f, y+0.2f);
                //_resourceTextMesh.SetActive(true);
                _init = true;
            }
            if (_init)
            {
                if (!string.IsNullOrEmpty(_weaponLevel) && _weaponLevel != "0")
                {
                    //_resourceTextMesh.GetComponent<TextMesh>().color = Color.white;
                    //_resourceTextMesh.GetComponent<TextMesh>().text = _weaponLevel.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    //_resourceTextMesh.GetComponent<TextMesh>().text = "";
                }
            }
            
            //resourceText.SetActive(newValue != 0);
            if (_isUsed)
            {
                _renderer.color = new Color(0.25f, 0.25f, 0.25f);
            }
            else
            {
                _renderer.color = Color.white;
            }
        }

        public void SetUsed(string isUsed)
        {
            _isUsed = Boolean.Parse(isUsed);
        }

        public void SetWeaponLevel(string weaponLevel)
        {
            _weaponLevel = weaponLevel;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
