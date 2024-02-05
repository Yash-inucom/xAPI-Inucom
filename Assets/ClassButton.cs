using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ClassButton : MonoBehaviour
{
    public string uuidinst {get;set;}
    //public string uuidinstpublic = string.Empty;
    public string classNameTag { get;set;}
    //public GameObject buttonFind;

    public GameObject answerButton { get;set;}
    public GameObject questionCanvas { get;set;}

    public string buttonValue { get;set;}

    public List<string> classUUIDDS;

    public void sendIt()
    {
        Debug.Log(uuidinst);
        string keyboard = "Keyboard";
        findSet(keyboard, false);
        
        string InputField = "InputField";
        findSet(InputField, false);

        string classCanvas = "classCanvas";
        findSet(classCanvas, false);

        string questionCanvas = "questionCanvas";
        findSet(questionCanvas, true);

        string submiTest = "TestButtonHolder";
        findSet(submiTest, false);

        //var randomHandler = new System.Random();
        var answerSelection = new List<string>
        {
            "A",
            "B",
            "C",
            "D",
            "E"
        };

        //int answers = randomHandler.Next(answerSelection.Count);
        
        //string otherClass = "";

        for (int i = 0; i < 5; i++)
        {
            /*string findQuestion = "QuestionEx";
            GameObject exampleQuestion = GameObject.FindWithTag(findQuestion);
            Debug.Log(exampleQuestion);
            
            //exampleQuestion.SetActive(true);
            Debug.Log("why");
*/
            buttonValue = answerSelection[i];
            buttonMaker(buttonValue);
            //Debug.Log(buttonValue);

            //testQuizOptions.transform.GetChild(i).gameObject.SetActive(true); 
            //Debug.Log(options.GetComponentInChildren<Option>().value);
        }

        //hide the class buttons until the "test" is done.
        //findSet(classCanvas,true);

        //Debug.Log(uuidinstpublic);


        //get reference to obj, by name, tag, whtvs

        //func call inside the object, sending UUID

    }

    public void findSet(string tagToFind, bool setTag)
    {
        GameObject foundTag = GameObject.FindWithTag(tagToFind);
        for(int i = 0; i<foundTag.transform.childCount; i++)
            foundTag.transform.GetChild(i).gameObject.SetActive(setTag);
    }

    public void buttonMaker(string buttonValue)
    {
        string findTheCanvas = "questionCanvas";
        string findTheButtons = "answerButtons";
        questionCanvas = GameObject.FindWithTag(findTheCanvas);
        //Debug.Log(questionCanvas);
        answerButton = GameObject.FindWithTag(findTheButtons);
        //Debug.Log(answerButton);
        GameObject answButt = Instantiate(answerButton, questionCanvas.transform);
        answButt.GetComponent<AnswerButton>().Draw(buttonValue);
        answButt.name = buttonValue;
        answButt.GetComponent<AnswerButton>().value = buttonValue;
    }



    public void Draw(string myuuid, string myclearText)
    {
        uuidinst = myuuid;
        classNameTag = myclearText;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = myclearText;
        /*uuidinstpublic = uuidinst;
        classUUIDDS.Add(uuidinstpublic);
        Debug.Log(classUUIDDS);*/
    }


}
