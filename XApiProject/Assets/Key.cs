using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Key : MonoBehaviour
{
  public bool modifierKey = false;
  public bool caps { get; set; } = false;
  public bool shift { get; set; } = false;
  public string value;
  public TextMeshPro TM;
  public TextMeshPro InputField;
  public GameObject[] Keys;

  private void Awake() 
  {
    TM = GetComponentInChildren<TextMeshPro>();
    InputField = GameObject.FindWithTag("InputField").transform.GetChild(0).GetComponentInChildren<TextMeshPro>();
    value = TM.text;
    Shift(false);
    Keys = (GameObject[])GameObject.FindGameObjectsWithTag("Key");
  }
  public void Shift(bool cap = false) 
  {
    if(cap && !modifierKey) 
    {
      value = value.ToUpper();
    }
    else 
    {
      value = value.ToLower();
    }
    TM.text = value;
  }
    public void Pressed()
    {
        if (!modifierKey)
        {
            InputField.text += value;
            if (shift == true && caps == false)
            {
                for (int i = 0; i < Keys.Length; i++)
                {
                    Keys[i].GetComponent<Key>().shift = false;
                    Keys[i].GetComponent<Key>().Shift(false);
                }
            }
            else if (shift == true && caps == true)
            {
                for (int i = 0; i < Keys.Length; i++)
                {
                    Keys[i].GetComponent<Key>().shift = true;
                    Keys[i].GetComponent<Key>().Shift(true);
                }
            }
            else
            {
                for (int i = 0; i < Keys.Length; i++)
                {
                    Keys[i].GetComponent<Key>().shift = false;
                    Keys[i].GetComponent<Key>().Shift(false);
                }
            }

        }
        else
        {
            value = value.ToLower();
            if (value == "del")
            {
                if (InputField.text != "")
                {
                    InputField.text = InputField.text.Remove(InputField.text.Length - 1, 1);
                }
            }
            if (value == "caps")
            {
                Debug.Log("Clicked Caps");
                for (int i = 0; i < Keys.Length; i++)
                {
                    Debug.Log(Keys[i]);
                    Keys[i].GetComponent<Key>().caps = !caps;
                    Keys[i].GetComponent<Key>().shift = !caps;
                    Keys[i].GetComponent<Key>().Shift(!caps);
                }
            }
            if (value == "next")
            {
                if (InputField.text != "")
                {
                    {
                        InputField = GameObject.FindWithTag("InputField").transform.GetChild(1).GetComponentInChildren<TextMeshPro>();
                        value = "submit";
                        TM.text = value;
                    }
                }

                if (value == "submit")
                {
                    if (InputField.text != "")
                    {
                        {
                            //Need to fix later to reflect changes to the Submit method, needs to be fed inputs later
                            //GameObject.FindGameObjectWithTag("InputField").GetComponent<Input_Field>().Submit();
                            GameObject.FindGameObjectWithTag("OptionPanel").GetComponent<Menu>().MenuEnable();
                            GameObject.FindGameObjectWithTag("InputField").SetActive(false);
                            GameObject.FindGameObjectWithTag("Keyboard").SetActive(false);

                        }
                    }
                    if (value == "shift")
                    {
                        Debug.Log("Clicked Shift");
                        for (int i = 0; i < Keys.Length; i++)
                        {
                            Keys[i].GetComponent<Key>().shift = true;
                            Keys[i].GetComponent<Key>().Shift(true);
                        }
                    }
                }
                if (value == " ")
                {

                }

            }
        }

    }
}
