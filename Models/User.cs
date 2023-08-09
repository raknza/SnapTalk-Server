namespace android_backend.Models;

using System;



public class User
{
    public int id { get; set; }
    public String username { get; set; }
    public String password { get; set; }
    public String name { get; set; }
    public String? email { get; set; }
    public String? avatar { get; set; }
    public String? profile { get; set; }
}
