using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnswerButton : MonoBehaviour
{
    //public string buttonValue {  get; set; }
    public string value;
    // Start is called before the first frame update
    
    public void Draw(string myValue)
    {
        //NullReferenceException: Object reference not set to an instance of an object
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = myValue;
    }
}
