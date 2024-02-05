using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// This record is used when creating xAPI statements. Each parameter must contain a value, even if that value is null.
/// Failure to do so will cause the values provided to potentially misallign and be sent in the xAPI statement incorrectly.
/// </summary>
/// <param name="Verb">Passed as a string, this parameter represents the verb of the xAPI statement, 
///                    and therefore the type of statement that is sent.
///                    Currently, valid verbs include: 
///                     "initialized", 
///                     "exited", 
///                     "completed", 
///                     "incorrect", 
///                     "correct", 
///                     "passed", 
///                     "failed".
///                    </param>
/// <param name="AssignmentUuid">Passed as a string, this parameter represents the UUID of an assignment.</param>
/// <param name="AssignmentName">Passed as a string, this parameter represents the name of an assignment.</param>
/// <param name="Answer">Passed as a string, this parameter represents the answer that the user has selected for a given question.</param>
/// <param name="CorrectAnswer">Passed as a string, this parameter represents the correct answer for a given question.</param>
/// <param name="QuestionNum">Passed as a string, this parameter represents the question number of a given question.</param>
/// <param name="Question">Passed as a string, this parameter represents the content of a given question.</param>
/// <param name="ObjectDesc">Passed as a string, this parameter represents a description of the object of the xAPI statement.</param>
/// <param name="ClassUuid">Passed as a string, this parameter represents the UUID of a class.</param>
/// <param name="Success">Passed as a string, but treated as a boolean, with valid values being "true" or "false".
///                       This parameter represents whether or not the user successfully passed a given assignment.</param>
/// <param name="RawScore">Passed as a string, but treated as a decimal, this parameter represents the final score of a given assignment.</param>
public struct XapiStatementBody {
    public string Verb;
    public string AssignmentUuid;
    public string AssignmentName;
    public string Answer;
    public string CorrectAnswer;
    public string QuestionNum;
    public string Question;
    public string ObjectDesc;
    public string ClassUuid;
    public string Success;
    public string RawScore;
}

[Serializable]
public class LoginTokenResponse {
  [JsonProperty("access_token")] public string AccessToken { get; set; }
}

// This currently is not used and can be ignored
//[Serializable]
//public class LoginRoles {
//  [JsonProperty("isAdmin")] public bool IsAdmin { get; set; }
//  [JsonProperty("isInstructor")] public bool IsInstructor { get; set; }
//  [JsonProperty("isStudent")] public bool IsStudent { get; set; }
//}

[Serializable]
public class StudentAssignmentJSON {
  [JsonProperty("AssignmentDetailUUID")] public string AssignmentDetailUUID;
  [JsonProperty("AssignmentName")] public string AssignmentName;
  [JsonProperty("AssignmentDescription")] public string AssignmentDescription;
  [JsonProperty("AssignmentURL")] public string AssignmentURL;
  [JsonProperty("AssignmentJSON")] public string AssignmentJSON;
  [JsonProperty("AssignmentType")] public string AssignmentType;
  [JsonProperty("ClassUUID")] public string ClasstUUID;
  [JsonProperty("AssignmentUUID")] public string AssignmentUUID;
  [JsonProperty("DueDate")] public string DueDate;
  [JsonProperty("AttemptsRemaining")] public int AttemptsRemaining;
  [JsonProperty("Completed")] public int Completed;
  [JsonProperty("Grade")] public float Grade;
}

public static class XapiHelper {
  // THESE URLs SHOULD NOT BE USED BY DEFAULT
  public const string URL_EXECUTE = "http://138.47.1.17:8080";
  public const string ASSIGNMENT_URL = "http://138.47.1.17/api";
  public const string URL_KEYCLOAK = "http://138.47.1.17:8081";

  public static string Token { get; private set; } = null;
  public static bool IsLoggedIn { get { return !string.IsNullOrEmpty(Token); } }
  public static StudentAssignmentJSON[] StudentAssignments { get; private set; } = null;
  public static XapiStatementBody GeneralBody { get; set; }

  public static string username;

  /// <summary>
  /// THIS METHOD SHOULD NOT BE USED BY DEFAULT
  /// 
  /// This method is an alternative version to the one below, if no URL is passed when calling the method, this version will be called instead.
  /// When called a default value for the URL is passed and the actual method is called.
  /// </summary>
  /// <param name="OnFinished">A boolean parameter which is given a value upon completion of the method. 
  ///                          You can use this boolean to perform certain actions after success or failure of the method.</param>
  /// <returns></returns>
  public static async Task<bool> Login(string username, string password, Action<bool> OnFinished = null) {
    return await Login(URL_KEYCLOAK, username, password,(success) => {
      if (success) {
        OnFinished?.Invoke(true);
      }
      else {
        OnFinished?.Invoke(false);
      }
    });
  }

  /// <summary>
  /// This method is used to log the user into the application by verifying the username and password using KeyCloak.
  /// Once verified, a token is provided and the token and the username are saved as variables to be used in other methods.
  /// This method is intended to be the first method called, and all other methods will not work without a valid login.
  /// </summary>
  /// <param name="URL">Passed as a string, this parameter represents the endpoint that will be contacted to validate the login</param>
  /// <param name="username">Passed as a string, typically an email, but does not have to be.</param>
  /// <param name="password">Passed as a string.</param>
  /// <param name="OnFinished">A boolean parameter which is given a value upon completion of the method. 
  ///                          You can use this boolean to perform certain actions after success or failure of the method.</param>
  /// <returns></returns>
  public static async Task<bool> Login(string URL, string username, string password, Action<bool> OnFinished = null) {
    try {
      RestClientOptions loginOptions = new(URL) {
        MaxTimeout = -1,
      };



      RestClient loginClient = new(loginOptions);
      RestRequest loginRequest = new RestRequest("/realms/test/protocol/openid-connect/token", Method.Post);
      loginRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      loginRequest.AddParameter("client_id", "lrs_user_client");
      loginRequest.AddParameter("username", username);
      loginRequest.AddParameter("password", password);
      loginRequest.AddParameter("grant_type", "password");
      loginRequest.AddParameter("scope", "email openid profile lrs:statements/read/mine lrs:statements/write");

      RestResponse loginResponse = await loginClient.ExecuteAsync(loginRequest);
      string tokenContent = loginResponse.Content;
      Debug.Log(tokenContent);

      Token = JsonConvert.DeserializeObject<LoginTokenResponse>(tokenContent)?.AccessToken?.Trim();
      if (string.IsNullOrEmpty(Token)) {
        Token = null;
        Debug.Log($"Incorrect username or password");
        OnFinished?.Invoke(false);
        return false;
      }
      else {
        Debug.Log($"Got Token: '{Token}'");
        XapiHelper.username = username;
        Debug.Log(username);

        await GrabStudentAssignments();

        OnFinished?.Invoke(true);
        return true;
      }
    }
    catch (Exception e) {
      Debug.LogError($"Error while trying to submit score via xAPI: {e}");
      OnFinished?.Invoke(false);
      return false;
    }
  }

  /// <summary>
  /// THIS METHOD SHOULD NOT BE USED BY DEFAULT
  /// 
  /// This method is an alternative version to the one below, if no URL is passed when calling the method, this version will be called instead.
  /// When called a default value for the URL is passed and the actual method is called.
  /// </summary>
  /// <param name="OnFinished">A boolean parameter which is given a value upon completion of the method. 
  ///                          You can use this boolean to perform certain actions after success or failure of the method.</param>
  /// <returns></returns>
  public static async Task<bool> GrabStudentAssignments(Action<bool> OnFinished = null) {
    return await GrabStudentAssignments(ASSIGNMENT_URL, (success) => {
      if (success) {
        OnFinished?.Invoke(true);
      }
      else {
        OnFinished?.Invoke(false);
      }
    });
  }

  /// <summary>
  /// This method is used to grab all assignments that are available to the user by sending the username in the 
  /// login method to the LRS using a specific route. The content that is returned from the LRS is and saved to 
  /// the StudentAssignments variable which can then be accessed even outside of this file
  /// </summary>
  /// <param name="URL">Passed as a string, this parameter represents the endpoint that will be contacted to grab the assignments.</param>
  /// <param name="OnFinished">A boolean parameter which is given a value upon completion of the method. 
  ///                          You can use this boolean to perform certain actions after success or failure of the method.</param>
  /// <returns></returns>
  public static async Task<bool> GrabStudentAssignments(string URL, Action<bool> OnFinished = null) {
    try {
      var studentAssignments = new RestClientOptions(URL) {
        MaxTimeout = -1,
      };
      var assignmentsClient = new RestClient(studentAssignments);
      var assignmentsRequest = new RestRequest("/assignmentsByStudent", Method.Post);

      var data = new { studentEmail = username };

      assignmentsRequest.AddHeader("accessToken", Token);
      assignmentsRequest.AddBody(JsonConvert.SerializeObject(data));

      RestResponse assignmentsResponse = await assignmentsClient.ExecuteAsync(assignmentsRequest);
      Debug.Log($"{nameof(assignmentsResponse.Content)}={assignmentsResponse.Content}");
      string assignmentsContent = assignmentsResponse.Content;
      Debug.Log($"{nameof(assignmentsContent)}={assignmentsContent}");
      StudentAssignments = JsonConvert.DeserializeObject<StudentAssignmentJSON[]>(assignmentsContent);

      await GrabFile(ASSIGNMENT_URL + "/3ef9412b-6041-4420-835d-27265305c7cd");

      OnFinished?.Invoke(true);
      return true;
    }
    catch (Exception e) {
      Debug.LogError($"Error while trying to grab assignments from LRS: {e}");
      OnFinished?.Invoke(false);
      return false;
    }
  }

  /// <summary>
  /// THIS METHOD SHOULD NOT BE USED BY DEFAULT
  /// 
  /// This method is an alternative version to the one below, if no URL is passed when calling the method, this version will be called instead.
  /// When called a default value for the URL is passed and the actual method is called.
  /// </summary>
  /// <param name="OnFinished">A boolean parameter which is given a value upon completion of the method. 
  ///                          You can use this boolean to perform certain actions after success or failure of the method.</param>
  /// <returns></returns>
  public static async Task<bool> GrabFile(string nameOfFile, Action<bool> OnFinished = null) {
    return await GrabFile(ASSIGNMENT_URL, nameOfFile, (success) => {
      if (success) {
        OnFinished?.Invoke(true);
      }
      else {
        OnFinished?.Invoke(false);
      }
    });
  }

  /// <summary>
  /// This method is currently in development
  /// This method is used to download a file from the LRS
  /// </summary>
  /// <param name="URL">Passed as a string, this parameter represents the endpoint that will be contacted to download the file.</param>
  /// <param name="nameOfFile">Passed as a string, this parameter is the name of the file to be downloaded.</param>
  /// <param name="OnFinished">A boolean parameter which is given a value upon completion of the method. 
  ///                          You can use this boolean to perform certain actions after success or failure of the method.</param>
  /// <returns></returns>
  public static async Task<bool> GrabFile(string URL, string nameOfFile, Action<bool> OnFinished = null) {
    try {
      var studentAssignments = new RestClientOptions(URL) {
        MaxTimeout = -1,
      };
      var fileGrabClient = new RestClient(studentAssignments);
      var fileGrabRequest = new RestRequest("/downloadFile", Method.Post);

      var data = new { fileName = nameOfFile };

      fileGrabRequest.AddHeader("accessToken", Token);
      fileGrabRequest.AddBody(JsonConvert.SerializeObject(data));

      RestResponse fileGrabResponse = await fileGrabClient.ExecuteAsync(fileGrabRequest);
      Debug.Log($"{nameof(fileGrabResponse.Content)}={fileGrabResponse.Content}");
      string fileGrabContent = fileGrabResponse.Content;
      Debug.Log($"{nameof(fileGrabContent)}={fileGrabContent}");

      OnFinished?.Invoke(true);
      return true;
    }
    catch (Exception e) {
      Debug.LogError($"Error while trying to grab file from LRS: {e}");
      OnFinished?.Invoke(false);
      return false;
    }
  }

  /// <summary>
  /// THIS METHOD SHOULD NOT BE USED BY DEFAULT
  /// 
  /// This method is an alternative version to the one below, if no URL is passed when calling the method, this version will be called instead.
  /// When called a default value for the URL is passed and the actual method is called.
  /// </summary>
  /// <param name="OnFinished">A boolean parameter which is given a value upon completion of the method. 
  ///                          You can use this boolean to perform certain actions after success or failure of the method.</param>
  /// <returns></returns>
  public static async Task<bool> UpdateGrade(string assignmentUUID, decimal assignmentGrade, Action<bool> OnFinished = null) {
    return await UpdateGrade(ASSIGNMENT_URL, assignmentUUID, assignmentGrade, (success) => {
      if (success) {
        OnFinished?.Invoke(true);
      }
      else {
        OnFinished?.Invoke(false);
      }
    });
  }

  /// <summary>
  /// This method is used to update an assignment and give it a new grade
  /// </summary>
  /// <param name="URL">Passed as a string, this parameter represents the endpoint that will be contacted to update the assignment.</param>
  /// <param name="assignmentUUID">Passed as a string, this parameter is the UUID of the assignment to be updated.</param>
  /// <param name="assignmentGrade">Passed as a decimal, this parameter is the new grade to be given to the updated assignment.</param>
  /// <param name="OnFinished">A boolean parameter which is given a value upon completion of the method. 
  ///                          You can use this boolean to perform certain actions after success or failure of the method.</param>
  /// <returns></returns>
  public static async Task<bool> UpdateGrade(string URL, string assignmentUUID, decimal assignmentGrade, Action<bool> OnFinished = null) {
    try {
      var scoreSubmitOptions = new RestClientOptions(URL) {
        MaxTimeout = -1,
      };
      var scoreSubmitClient = new RestClient(scoreSubmitOptions);
      var scoreSubmitRequest = new RestRequest("/updateAssignment", Method.Put);

      bool foundUUIDinAssignments = false;

      for (int i = 0; i < StudentAssignments.Length; i++) {
        if (StudentAssignments[i].AssignmentUUID == assignmentUUID) {
          if (StudentAssignments[i].AttemptsRemaining - 1 < 0) {
            Debug.Log("Cannot submit score for assignment with no attempts remaining");
            OnFinished?.Invoke(false);
            return false;
          }
          else {
            foundUUIDinAssignments = true;

            var data = new {
              studentEmail = username,
              assignmentDetailID = StudentAssignments[i].AssignmentDetailUUID,
              UUID = assignmentUUID,
              dueDate = StudentAssignments[i].DueDate,
              attemptsRemaining = StudentAssignments[i].AttemptsRemaining - 1,
              completed = 1,
              grade = assignmentGrade,
            };

            scoreSubmitRequest.AddBody(JsonConvert.SerializeObject(data));
            scoreSubmitRequest.AddHeader("accessToken", Token);
          }
          break;
        }
      }

      if (foundUUIDinAssignments) {
        RestResponse scoreSubmitResponse = await scoreSubmitClient.ExecuteAsync(scoreSubmitRequest);
        Debug.Log($"Status code for assignment statement submission: {scoreSubmitResponse.StatusCode}");
        Debug.Log($"Assignment statement submission response: {scoreSubmitResponse.Content}");
        OnFinished?.Invoke(true);
        return true;
      }
      else {
        Debug.Log("Invalid UUID passed, could not be found among student's assignments");
        OnFinished?.Invoke(false);
        return false;
      }
    }
    catch (Exception e) {
      Debug.LogError($"Error while trying to submit assignment statement via xAPI: {e}");
      OnFinished?.Invoke(false);
      return false;
    }
  }

  /// <summary>
  /// THIS METHOD SHOULD NOT BE USED BY DEFAULT
  /// 
  /// This method is an alternative version to the one below, if no URL is passed when calling the method, this version will be called instead.
  /// When called a default value for the URL is passed and the actual method is called.
  /// </summary>
  /// <param name="OnFinished">A boolean parameter which is given a value upon completion of the method. 
  ///                          You can use this boolean to perform certain actions after success or failure of the method.</param>
  /// <returns></returns>
  public static async Task<bool> SubmitXapiStatement(XapiStatementBody sb, Action<bool> OnFinished = null) {
    return await SubmitXapiStatement(URL_EXECUTE, sb, (success) => {
      if (success) {
        OnFinished?.Invoke(true);
      }
      else {
        OnFinished?.Invoke(false);
      }
    });
  }

  /// <summary>
  /// This method is used to submit a statement using xAPI
  /// </summary>
  /// <param name="sb">Passed as a XapiStatementBody, the statement body contains the necessary information 
  ///                  to be sent in the xAPI statement. See XapiStatementBody above to know how to populate it's values.</param>
  /// <param name="OnFinished">A boolean parameter which is given a value upon completion of the method. 
  ///                          You can use this boolean to perform certain actions after success or failure of the method.</param>
  /// <returns></returns>
  public static async Task<bool> SubmitXapiStatement(string URL, XapiStatementBody sb, Action<bool> OnFinished = null) {
    try {
      var scoreSubmitOptions = new RestClientOptions(URL) {
        MaxTimeout = -1,
      };
      var scoreSubmitClient = new RestClient(scoreSubmitOptions);
      var scoreSubmitRequest = new RestRequest("/xapi/statements", Method.Post);
      scoreSubmitRequest.AddHeader("X-Experience-API-Version", "1.0.3");
      scoreSubmitRequest.AddHeader("Content-Type", "application/json");
      scoreSubmitRequest.AddHeader("Authorization", $"Bearer {Token}");
      var scoreBody = GenStructureBody(sb);

      Debug.Log(scoreBody);

      scoreSubmitRequest.AddStringBody(scoreBody, DataFormat.Json);
      RestResponse scoreSubmitResponse = await scoreSubmitClient.ExecuteAsync(scoreSubmitRequest);
      Debug.Log($"Status code for assignment statement submission: {scoreSubmitResponse.StatusCode}");
      Debug.Log($"Assignment statement submission response: {scoreSubmitResponse.Content}");
      OnFinished?.Invoke(true);
      return true;
    }
    catch (Exception e) {
      Debug.LogError($"Error while trying to submit assignment statement via xAPI: {e}");
      OnFinished?.Invoke(false);
      return false;
    }
  }

  /// <summary>
  /// This method is used primarily by the SubmitXapiStatement method, it is used to take the data from a 
  /// XapiStatementBody, which contains the values to be entered into the statement, and turns it into the
  /// complete statement.
  /// </summary>
  /// <param name="sb">Passed as a XapiStatementBody, the statement body contains the necessary information 
  ///                  to be sent in the xAPI statement. See XapiStatementBody above to know how to populate it's values.</param>
  /// <returns>Returns an xAPI statement as a string.</returns>
  public static string GenStructureBody(XapiStatementBody sb) {
    switch (sb.Verb) {
      case "initialized":
      case "exited":
      case "completed": {
          return $@"
                    {{
                        ""actor"": {{
                            ""account"": {{
                                ""name"": ""{username}"",
                                ""homePage"": ""http://138.47.1.17:8081/realms/test""
                            }},
                            ""objectType"": ""Agent""
                        }},
                        ""verb"": {{
                            ""id"": ""http://adlnet.gov/expapi/verbs/{sb.Verb}"",
                            ""display"": {{
                                ""en-US"": ""{sb.Verb}""
                            }}
                        }},
                        ""object"": {{
                            ""id"": ""http://betaflixinc.com/fuelcell/activities/{sb.AssignmentUuid}"",
                            ""definition"": {{
                                ""name"": {{
                                    ""en-US"": ""{sb.AssignmentName}""
                                }}
                            }},
                            ""objectType"": ""Activity""
                        }},
                        ""context"": {{
                            ""registration"": ""{sb.ClassUuid}"",
                            ""platform"": ""Unity XR""
                        }}
                    }}";
        }

      case "incorrect":
      case "correct": {
          return $@"
                    {{
                        ""actor"": {{
                            ""account"": {{
                                ""name"": ""{username}"",
                                ""homePage"": ""http://138.47.1.17:8081/realms/test""
                            }},
                            ""objectType"": ""Agent""
                        }},
                        ""verb"": {{
                            ""id"": ""http://adlnet.gov/expapi/verbs/{sb.Verb}"",
                            ""display"": {{
                                ""en-US"": ""{sb.Verb}""
                            }}
                        }},
                        ""object"": {{
                            ""id"": ""http://betaflixinc.com/fuelcell/activities/{sb.AssignmentUuid}"",
                            ""definition"": {{
                                ""name"": {{
                                    ""en-US"": ""{sb.Answer}""
                                }},
                                ""description"": {{
                                    ""en-US"": ""Question {sb.QuestionNum}: {sb.Question}\nAnswer: {sb.CorrectAnswer}""
                                }}
                            }},
                            ""objectType"": ""Activity""
                        }},
                        ""context"": {{
                            ""registration"": ""{sb.ClassUuid}"",
                            ""platform"": ""Unity XR""
                        }}
                    }}";
        }

      case "passed":
      case "failed": {
          return $@"
                    {{
                        ""actor"": {{
                            ""account"": {{
                                ""name"": ""{username}"",
                                ""homePage"": ""http://138.47.1.17:8081/realms/test""
                            }},
                            ""objectType"": ""Agent""
                        }},
                        ""verb"": {{
                            ""id"": ""http://adlnet.gov/expapi/verbs/{sb.Verb}"",
                            ""display"": {{
                                ""en-US"": ""{sb.Verb}""
                            }}
                        }},
                        ""object"": {{
                            ""id"": ""http://betaflixinc.com/fuelcell/activities/{sb.AssignmentUuid}"",
                            ""definition"": {{
                                ""name"": {{
                                    ""en-US"": ""{sb.AssignmentName}""
                                }}
                            }},
                            ""objectType"": ""Activity""
                        }},
                        ""result"": {{
                            ""success"": {sb.Success},
                            ""score"": {{
                                ""raw"": {sb.RawScore}
                            }}
                        }},
                        ""context"": {{
                            ""registration"": ""{sb.ClassUuid}"",
                            ""platform"": ""Unity XR""
                        }}
                    }}";
        }
      default:
        return $@"
                  {{
                    ""actor"": {{
                        ""account"": {{
                            ""name"": ""{username}"",
                            ""homePage"": ""http://138.47.1.17:8081/realms/test""
                        }},
                        ""objectType"": ""Agent""
                    }},
                    ""verb"": {{
                        ""id"": ""http://adlnet.gov/expapi/verbs/attempted"",
                        ""display"": {{
                            ""en-US"": ""attempted""
                        }}
                    }},
                    ""object"": {{
                        ""id"": ""https://www.betaflixinc.com/fuelcell"",
                        ""definition"": {{
                            ""name"": {{
                                ""en-US"": ""{sb.AssignmentName}""
                            }}
                        }},
                        ""objectType"": ""Activity""
                    }},
                    ""result"": {{
                        ""success"": {sb.Success},
                        ""score"": {{
                            ""raw"": {sb.RawScore}
                        }}
                    }},
                    ""context"": {{
                        ""registration"": ""{sb.ClassUuid}"",
                        ""platform"": ""Unity XR""
                    }}
                  }}";
    }

  }
}
