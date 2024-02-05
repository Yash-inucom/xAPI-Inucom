using UnityEngine;

public class LoginInvoker : MonoBehaviour
{
    
    void Start()
    {
        XapiHelper.Login("student@email.com", "password");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
