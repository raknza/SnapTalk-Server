namespace android_backend.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class User
{
    public int id { get; set; }
    public String username { get; set; }
    public String password { get; set; }
    public String? email { get; set; }
    public String? avatar { get; set; }

}
