using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp;
using Newtonsoft.Json;
using UnityEngine.UI;


[Serializable]
public class loginInputs
{
    public string unSubInput = "";
    public string pwSubInput = "";
}


[Serializable]
public class loginTokenResponse
{
    [JsonProperty("access_token")]
    public string accessToken { get; set; }
    
}

/* Depricated token class wrapper, discard before deployment
[Serializable]
public class AuthenticationResults
{
    [JsonProperty("AuthenticationResult")]
    public loginTokenResponse loginToken { get; set; }
}*/

[Serializable]
public class loginRoles
{
    [JsonProperty("isAdmin")]
    public bool isAdmin { get; set; }
    [JsonProperty("isInstructor")]
    public bool isInstructor { get; set; }
    [JsonProperty("isStudent")]
    public bool isStudent { get; set; }
}

[Serializable]
public class studentClasses
{
    [JsonProperty("Student List")]
    public string[] studentList { get; set; }
    [JsonProperty("Instructor List")]
    public string[] instructorList { get; set; }
    [JsonProperty("Class Description")]
    public string classDesc { get; set; }
    [JsonProperty("Start Date")]
    public string startDate { get; set; }
    [JsonProperty("UUID")]
    public string uuid { get; set; }
    [JsonProperty("Class Name")]
    public string classInfo { get; set; }
    [JsonProperty("End Date")]
    public string endDate { get; set; }
}

[Serializable]
public class studentAssignment
{
    [JsonProperty("Due Date")]
    public string dueDate { get; set; }
    [JsonProperty("Student List")]
    public string[] studentList { get; set; }
    [JsonProperty("Instructor List")]
    public string[] instructorList { get; set; }
    [JsonProperty("Assignment Description")]
    public string assignmentDescription { get; set; }
    [JsonProperty("Class UUID")]
    public string classUUID { get; set; }
    [JsonProperty("Platform")]
    public string platform { get; set; }
    [JsonProperty("UUID")]
    public string assignmentUUID { get; set; }
    [JsonProperty("Assignment Name")]
    public string assignmentName { get; set; }
    [JsonProperty("Assignment JSON")]
    public Dictionary<string, string> assignmentJSON { get; set; }
    [JsonProperty("Max Number of attempts")]
    public int maxAttempts { get; set; }

}

[Serializable]
public class studentResponseContent
{
    [JsonProperty("Classes")]
    public List<studentClasses> studentClasses { get; set; }
    [JsonProperty("Assignments")]
    public List<studentAssignment> studentAssignments { get; set; }
}

public class Input_Field : MonoBehaviour
{
    public TextMeshPro txtUser;
    public TextMeshPro txtPass;
    public GameObject classButton;
    public GameObject classCanvas;
    public string retrieveToken;
    public string classUUID;
    public string URL = "https://kcev3cammg.execute-api.us-east-2.amazonaws.com";

    void Start()
    {
        txtUser.text = "";
        txtPass.text = "";
    }

    public async void SubmitHandle()
    {
        loginInputs loginPass = new loginInputs();
        //DO NOT DELETE, THESE ARE THE ACTUAL LOGIN VARIABLES
        //loginPass.unSubInput = txtUser.text;
        //loginPass.pwSubInput = txtPass.text;

        //FOR TESTING, REMOVE BEFORE RELEASE
        loginPass.unSubInput = "notouch@email.com";
        loginPass.pwSubInput = "password";

        string json = JsonUtility.ToJson(loginPass);
        JsonUtility.FromJsonOverwrite(json, loginPass);

        //Adds the JSON output to a file for later
        System.IO.File.WriteAllText(Application.persistentDataPath + "/whateverwewanted.json", json);

        await Submit(loginPass.unSubInput, loginPass.pwSubInput);
    }

    public async Task Submit(string unSubInput, string pwSubInput)
    {
        var loginOptions = new RestClientOptions("https://keycloak.red-beans.click")
        {
            MaxTimeout = -1,
        };

        //Debug.Log(unSubInput);
        //Debug.Log(pwSubInput);

        var loginClient = new RestClient(loginOptions);
        var loginRequest = new RestRequest("/auth/realms/InternalDEV/protocol/openid-connect/token", Method.Post);

        loginRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        loginRequest.AddParameter("client_id", "InternalDevClient");
        loginRequest.AddParameter("username", unSubInput);
        loginRequest.AddParameter("password", pwSubInput);
        loginRequest.AddParameter("grant_type", "password");
        loginRequest.AddParameter("scope", "email openid profile");

        RestResponse loginResponse = await loginClient.ExecuteAsync(loginRequest);
        string tokenContent = loginResponse.Content;
        //Debug.Log(tokenContent);

        //Lane if you do something that changes the format of the API response I'm going to cry.
        var tokenDeserResult = JsonConvert.DeserializeObject<loginTokenResponse>(tokenContent);
        //Debug.Log(tokenDeserResult);

        string accessToken = tokenDeserResult.accessToken;
        
        retrieveToken = accessToken;

        Debug.Log(accessToken);

        var roleOptions = new RestClientOptions(URL)
        {
            MaxTimeout = -1,
        };
        var roleClient = new RestClient(roleOptions);
        var roleRequest = new RestRequest("/PROD/roles", Method.Get);

        roleRequest.AddHeader("accesstoken", accessToken);

        RestResponse roleResponse = await roleClient.ExecuteAsync(roleRequest);
        /*Debug.Log(roleResponse.StatusCode);
        Debug.Log(roleResponse.ErrorMessage);*/
        //Debug.Log(roleResponse.Content);


        //Role stuff

        string roleContent = roleResponse.Content;
        Debug.Log(roleContent);

        var roleDeserResult = JsonConvert.DeserializeObject<loginRoles>(roleContent);
        bool userIsStudent = roleDeserResult.isStudent;
        if (userIsStudent)
        {
            Debug.Log("User is a student");
            var studentInfoOptions = new RestClientOptions(URL)
            {
                MaxTimeout = -1,
            };
            var studentInfoClient = new RestClient(studentInfoOptions);
            var studentInfoRequest = new RestRequest("/PROD/students/info", Method.Get);
            studentInfoRequest.AddHeader("accesstoken", accessToken);

            RestResponse studentInfoResponse = await studentInfoClient.ExecuteAsync(studentInfoRequest);
            Debug.Log(studentInfoResponse.Content);
            /*Debug.Log(studentInfoResponse.Content.GetType());*/
            string studentInfoContent = studentInfoResponse.Content;

            var studentInfoDeserResult = JsonConvert.DeserializeObject<studentResponseContent>(studentInfoContent);

            for (int i = 0; i < studentInfoDeserResult.studentClasses.Count; i++)
            {

                string classButtonInfo = studentInfoDeserResult.studentClasses[i].classInfo;
                //Debug.Log(classButtonInfo);
                string classButtonUUID = studentInfoDeserResult.studentClasses[i].uuid;
                //Debug.Log(classButtonUUID);

                buttonMaker(classButtonInfo, classButtonUUID);

                //Debug.Log("Over to the options stuff");
            }
        }

        else Debug.Log("User is not a student");
        //would be where admin and teacher functions would take place
        //Debug.Log("Is Working");
    }

    public void buttonMaker(string buttonText, string buttonUUID) 
    {
        GameObject curButt = Instantiate(classButton, classCanvas.transform);
        curButt.GetComponent<ClassButton>().Draw(buttonUUID, buttonText);
        curButt.name = buttonText;
        Debug.Log(buttonText);
        Debug.Log(buttonUUID);

        //curButt.GetComponent<Button>().onClick.AddListener(classButtonClick);
        //curButt.GetComponent<ClassButton>().onClick.AddListener(sendIt);
    }
    
    /*public void classButtonClick()
    {
        Debug.Log("Button was pushed!");
        //Debug.Log(classUUID);
    }*/
}

   

