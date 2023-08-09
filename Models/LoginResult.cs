namespace android_backend.Models;

public class LoginResult
{
    public Boolean success { get; set; }
    public string token { get; set; }

    public LoginResult(Boolean success, string token){
        this.success = success;
        this.token = token;
    }
}