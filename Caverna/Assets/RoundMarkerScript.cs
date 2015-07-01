using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoundMarkerScript : MonoBehaviour
{

    public Sprite Round1Sprite;
    public Sprite Round2Sprite;
    public Sprite Round3Sprite;
    public Sprite Round4Sprite;
    public Sprite Round5Sprite;
    public Sprite Round6Sprite;
    public Sprite Round7Sprite;
    public Sprite Round8Sprite;
    public Sprite Round9Sprite;
    public Sprite Round10Sprite;
    public Sprite Round11Sprite;
    public Sprite Round12Sprite;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetRound(int round)
    {
        if (round == 1)
            GetComponent<Image>().sprite = Round1Sprite;
        if (round == 2)
            GetComponent<Image>().sprite = Round2Sprite;
        if (round == 3)
            GetComponent<Image>().sprite = Round3Sprite;
        if (round == 4)
            GetComponent<Image>().sprite = Round4Sprite;
        if (round == 5)
            GetComponent<Image>().sprite = Round5Sprite;
        if (round == 6)
            GetComponent<Image>().sprite = Round6Sprite;
        if (round == 7)
            GetComponent<Image>().sprite = Round7Sprite;
        if (round == 8)
            GetComponent<Image>().sprite = Round8Sprite;
        if (round == 9)
            GetComponent<Image>().sprite = Round9Sprite;
        if (round == 10)
            GetComponent<Image>().sprite = Round10Sprite;
        if (round == 11)
            GetComponent<Image>().sprite = Round11Sprite;
        if (round == 12)
            GetComponent<Image>().sprite = Round12Sprite;
    }
}
