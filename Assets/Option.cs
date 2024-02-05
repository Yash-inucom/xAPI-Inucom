using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;


[System.Serializable]
public class QuestionFormat : MonoBehaviour
{
    public string questionText;
    public string correctAns;
    public List<string> possibleAnswers;
    public int score;
    public bool isAnswered = false;
    public int questionNumber;
    public bool isAQuestion = false;
        
}


public class Option : MonoBehaviour
{
    
    public QuestionFormat currentQuestion;
    public QuestionFormat question1;
    public QuestionFormat question2;
    public QuestionFormat question3;
    public string value;
    public int questionCount = 3;
    //public int currQuestion;
    
    public void Awake()
    {
    }

    public void Start()
    {
        currentQuestion = new QuestionFormat();
        
        question1 = gameObject.AddComponent(typeof(QuestionFormat)) as QuestionFormat; 
        question2 = gameObject.AddComponent(typeof(QuestionFormat)) as QuestionFormat; 
        question3 = gameObject.AddComponent(typeof(QuestionFormat)) as QuestionFormat; 
        currentQuestion = question1;

        
        /*QuestionFormat question1;
        GameObject firstQuestion = new GameObject("Godwhy");
        question1 = firstQuestion.GetComponent<QuestionFormat>();*/

        /*QuestionFormat question1 = gameObject.AddComponent<QuestionFormat>();*/

        question1.questionText = "In learning to control the airplane in level flight, it is important that the pilot be taught to maintain a _________ on the flight controls using _________.";
        question1.correctAns = "light touch, fingers";
        question1.possibleAnswers = new List<string>
        {
            "light touch, fingers",
            "medium touch, fingers",
            "light touch, palms/hands",
            "medium touch, palms/hands",
            "hard touch, palms/hands"

        };
        question1.isAnswered = false;
        question1.questionNumber = 1;
        question1.isAQuestion = true;

        /*QuestionFormat question2;
        GameObject secondQuestion = new GameObject("question2");
        question2 = secondQuestion.GetComponent<QuestionFormat>();*/

        /*QuestionFormat question2 = gameObject.AddComponent<QuestionFormat>();*/

        question2.questionText = "It is possible to maintain straight flight by simply exerting the necessary _______ with the ___________ independently in the desired direction of correction.";
        question2.correctAns = "pressure ailerons or rudder";
        question2.possibleAnswers = new List<string>
        {
            "pressure elavator or rudder",
            "pressure ailerons or rudder",
            "force ailerons or rudder",
            "force elavator or rudder",
            "force landing gear or rudder"

        };
        question2.isAnswered = false;
        question2.questionNumber = 2;
        question2.isAQuestion = true;




        
        /*QuestionFormat question3;
        GameObject thirdQuestion = new GameObject("question3");
        question3 = thirdQuestion.GetComponent<QuestionFormat>();*/

        /*QuestionFormat question3 = gameObject.AddComponent<QuestionFormat>();*/

        question3.questionText = "Common errors include: Mechanically __________ on the flight controls rather than exerting _________.";
        question3.correctAns = "pushing or pulling, accurate and smooth pressure";
        question3.possibleAnswers = new List<string>
        {
            "pushing or pulling, accurate and smooth pressure",
            "yanking and tugging, rigid and consistent pressure",
            "pushing or pulling, rigid and consistent pressure",
            "yanking and tugging, accurate and smooth pressure  ",
            "pushing or pulling, accurate and smooth pressure"

        };
        question3.isAnswered = false;
        question3.questionNumber = 3;
        question3.isAQuestion = true;

               
        switchQuestion((questionCount-questionCount-1));


    }

    public void switchQuestion(int currentQuestionNumber)
    {
        string testQuestion = "QuestionEx";
        GameObject findQuestion = GameObject.FindWithTag(testQuestion);
        switch (currentQuestion.questionNumber)
        {
            case 1:

                findQuestion.GetComponent<Option>().currentQuestion = currentQuestion;
                findQuestion.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.questionText;
                answerRandomizer(currentQuestion);

                break;
            case 2:
                currentQuestion = question2;
                findQuestion.GetComponent<Option>().currentQuestion = currentQuestion;
                findQuestion.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.questionText;
                answerRandomizer(currentQuestion);
                break;
            case 3:

                findQuestion.GetComponent<Option>().currentQuestion = currentQuestion;
                findQuestion.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.questionText;
                answerRandomizer(currentQuestion);
                break;

        }
    }
    

    public void answerRandomizer(QuestionFormat question)
    {
        
        var randomHolder = new System.Random();
        List<string> answerSelection;
        answerSelection = question.possibleAnswers;    
        for (int i = question.possibleAnswers.Count-1; i > 0; i--)
        {
            int k = randomHolder.Next(i+1);
            string answer = answerSelection[k];
            answerSelection[k] = answerSelection[i];
            answerSelection[i] = answer;
        }
        //Debug.Log(answerSelection);
        string findTheTag = "questionCanvas";
        GameObject questionCanvas = GameObject.FindWithTag(findTheTag);
        for (int i = 1; i < questionCanvas.transform.childCount; i++)
        {
            //Debug.Log(questionCanvas.transform.childCount-1);
            //Debug.Log(answerSelection[i-1]);
            questionCanvas.transform.GetChild(i).GetComponent<Option>().value = answerSelection[i-1];
            questionCanvas.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = answerSelection[i-1];
        }
        /*int randomAnswer = randomHolder.Next(answerSelection.Count);
        string answerHolder = answerSelection[randomAnswer];*/
        
        //string questionAnswer = answerSelection[randomAnswer];
        //answerHolder = questionAnswer;
        //Debug.Log("Question Answer is " + questionAnswer);
        
        //Debug.Log("Answer Holder is " + answerHolder);

        
        //need to remove
        //questionFinder.GetComponent<Option>().value = answerHolder;
        
        //Debug.Log(questionFinder.GetComponent<Option>().value);

        //questionFinder.GetComponentInChildren<TextMeshProUGUI>().text = "Select " + questionAnswer;
        
        //return answerHolder;
    }


    public async void Selected()
    {
        int startQuestions = 1;
        //QuestionFormat currentQuestion = gameObject.AddComponent<QuestionFormat>();
        /*currentQuestion;
        Debug.Log(theQuestion);*/
        //string correctAnswer = curren
        //int correct = 0;
        //int completed = 0;
        //double score;

        //Dictionary<int, int> ele1 = new Dictionary<int, int>(questionScores);
        //foreach (KeyValuePair<int, int> q  in ele1)

        //score for use with the dict system, will need later
        //int score = 0;



        string testQuestion = "QuestionEx";
        GameObject questionFinder = GameObject.FindWithTag(testQuestion);
        string test = transform.GetComponent<Option>().value.ToString();


        string questionHolder = GameObject.FindWithTag("Question").GetComponent<QuestionFormat>().correctAns;
        //string answerHolder = questionHolder.GetComponent<Option>().value;

        //string answerHolder = currentQuestion.correctAns;
        string answerHolder = currentQuestion.correctAns.ToString();
        Debug.Log("Correct answer is " + answerHolder);
        //string answerHolder = "pushing or pulling, accurate and smooth pressure";

        Debug.Log("Selected button value is " + test);


        Debug.Log("The answer to test against is " + answerHolder);


        //score = correct / (double)questionCount;
        //score = score * 100;
        //Debug.Log("User scored: " + score + "%");

        string accessToken = GameObject.FindWithTag("InputField").GetComponent<Input_Field>().retrieveToken.ToString();
        //string accessToken = GetComponent<Input_Field>().retrieveToken;
        string classUUID = GameObject.FindWithTag("InputField").GetComponent<Input_Field>().classUUID; ;
        if (test == answerHolder)
        {
            await Correct(currentQuestion.questionNumber, accessToken, classUUID);
            /*currentQuestion.isAnswered = true;
            currentQuestion.questionNumber++;
            startQuestions++;
            switchQuestion(startQuestions);*/

        }


        else
        {
            await InCorrect(currentQuestion.questionNumber, accessToken, classUUID);
            /*currentQuestion.isAnswered = true;
            currentQuestion.questionNumber++;
            startQuestions++;
            switchQuestion(startQuestions);*/
        }
        currentQuestion.isAnswered = true;
        currentQuestion.questionNumber++;
        startQuestions++;
        if(currentQuestion != question1)
            currentQuestion = question2;

        else if (currentQuestion != question2 )
            currentQuestion = question2;

        else
        {

        }
        switchQuestion(startQuestions);
        //Debug.Log("Got to here!");
    }


    public async Task Correct(int questionNumber, string accessToken, string classUUID)
    {
        //Flashes the correct banner on a right answer, only working in VR
        /*transform.parent.GetChild(4).gameObject.SetActive(true);
        transform.parent.GetComponent<Menu>()?.MenuDisable();*/
        var scoreSubmitOptions = new RestClientOptions("https://kcev3cammg.execute-api.us-east-2.amazonaws.com")
        {
            MaxTimeout = -1,
        };
        var scoreSubmitClient = new RestClient(scoreSubmitOptions);
        var scoreSubmitRequest = new RestRequest("/PROD/students/statements", Method.Post);

        


        //Debug.Log(scoreAccessToken);
        scoreSubmitRequest.AddHeader("accesstoken", accessToken);
        scoreSubmitRequest.AddHeader("Content-Type", "application/json");
        var scoreBody = $@"
            {{
                ""actor"": {{
                    ""mbox"": ""mailto:notouch@email.com"",
                    ""name"": ""test-user"",
                    ""objectType"": ""Agent""
            }},

                ""verb"": {{
                ""id"": ""http://adlnet.gov/expapi/verbs/passed"",
                ""display"": {{
                    ""en-US"": ""passed""
                }}
            }},

                ""context"": {{
                    ""platform"": ""Test Class"",
                    ""registration"": ""9822fd0f-12a6-47b5-a6a0-d554f45ac912"",
                    ""instructor"": {{
                        ""mbox"": ""mailto:chris.conway@betaflixinc.com"",
                        ""name"": ""chris-conway""
                }}
            }},
                ""object"": {{
                    ""id"": ""http://adlnet.gov/expapi/activities/example"",
                    ""definition"": {{
                    ""name"": {{
                    ""en-US"": ""This is a test assignment for validating API functionality.""
                    }},
                ""description"": {{
                    ""en-US"": ""This is a test assignment for validating API functionality.""
                    }}
                }},
                ""objectType"": ""Activity""
            }}
    }}";        

        scoreSubmitRequest.AddStringBody(scoreBody, DataFormat.Json);
        RestResponse scoreSubmitResponse = await scoreSubmitClient.ExecuteAsync(scoreSubmitRequest);
        Debug.Log(scoreSubmitResponse.StatusCode);
        string scoreResponseContent = scoreSubmitResponse.Content;
        Debug.Log(scoreResponseContent.ToString());
        Debug.Log("Correct Answer!");
        //score += 1;

    }



    public async Task InCorrect(int questionNumber, string accessToken, string classUUID)
    {
        //Flashes the incorrect banner on a right answer, only working in VR
        /*transform.parent.GetChild(5).gameObject.SetActive(true);
        transform.parent.GetComponent<Menu>()?.MenuDisable();*/

        var scoreSubmitOptions = new RestClientOptions("https://kcev3cammg.execute-api.us-east-2.amazonaws.com")
        {
            MaxTimeout = -1,
        };
        var scoreSubmitClient = new RestClient(scoreSubmitOptions);
        var scoreSubmitRequest = new RestRequest("/PROD/students/statements", Method.Post);

       
        scoreSubmitRequest.AddHeader("accesstoken", accessToken);
        scoreSubmitRequest.AddHeader("Content-Type", "application/json");
        Debug.Log(accessToken);
        var scoreBody = $@"
            {{
                ""actor"": {{
                    ""mbox"": ""mailto:notouch@email.com"",
                    ""name"": ""test-user"",
                    ""objectType"": ""Agent""
            }},

                ""verb"": {{
                ""id"": ""http://adlnet.gov/expapi/verbs/failed"",
                ""display"": {{
                    ""en-US"": ""failed""
                }}
            }},

                ""context"": {{
                    ""platform"": ""Test Class"",
                    ""registration"": ""9822fd0f-12a6-47b5-a6a0-d554f45ac912"",
                    ""instructor"": {{
                        ""mbox"": ""mailto:chris.conway@betaflixinc.com"",
                        ""name"": ""chris-conway""
                }}
            }},
                ""object"": {{
                    ""id"": ""http://adlnet.gov/expapi/activities/example"",
                    ""definition"": {{
                    ""name"": {{
                    ""en-US"": ""This is a test assignment for validating API functionality.""
                    }},
                ""description"": {{
                    ""en-US"": ""This is a test assignment for validating API functionality.""
                    }}
                }},
                ""objectType"": ""Activity""
            }}
    }}";



        scoreSubmitRequest.AddStringBody(scoreBody, DataFormat.Json);
        RestResponse scoreSubmitResponse = await scoreSubmitClient.ExecuteAsync(scoreSubmitRequest);
        Debug.Log(scoreSubmitResponse.StatusCode);
        string scoreResponseContent = scoreSubmitResponse.Content;
        Debug.Log(scoreResponseContent.ToString());
        Debug.Log("Incorrect Answer!");
        //int score = 0;
    }
}


