using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question : MonoBehaviour
{
    public string questionText;
    public string correctAns;
    public List<string> possibleAnswers;
    public int score;
    public bool isAnswered = false;
    public int questionNumber;

    
    // Start is called before the first frame update
    void Start()
    {
        /*Question thisQuestion = new Question();
        GameObject findQuestion = GameObject.FindWithTag("Question");
        findQuestion.currentQuestion = thisQuestion;*/

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
