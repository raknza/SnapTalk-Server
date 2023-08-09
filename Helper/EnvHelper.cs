namespace android_backend.Helper;


using System;

public class EnvHelper
{
    private readonly IConfiguration configuration;

    public EnvHelper(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public String GetValue(String key){
        DotNetEnv.Env.Load();
        String value = configuration.GetValue<string>(key);
        var arr = value.ToCharArray();
        var envName = "";
        Boolean findEnv = false;
        var realValue = "";
        for(int i=0;i<arr.Length;i++){
            if(arr[i] == '}'){
                findEnv = false;
                realValue = realValue + Environment.GetEnvironmentVariable(envName.ToString());
                envName = "";
            }
            else if(findEnv == true){
                envName = envName + arr[i];
            }
            else if(arr[i] == '{'){
                findEnv = true;
            }
            else if(arr[i] != '$'){
                realValue = realValue + arr[i];
            }
        }
        return realValue.ToString().Trim();
    }

}
