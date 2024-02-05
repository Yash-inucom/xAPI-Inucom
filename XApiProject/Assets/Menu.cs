using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  public void MenuEnable() {
    for (int i = 0; i < 4; i++) {
      transform.GetChild(i).gameObject.SetActive(true);
    }
  }
  public void MenuDisable() {
      for (int i = 0; i < 4; i++) {
        transform.GetChild(i).gameObject.SetActive(false);
      }
    }
}