namespace android_backend.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class ContactUsers
{
    public ContactUsers() {}

    public ContactUsers(User user){
        id = user.id;
        username = user.username;
        name = user.name;
        email = user.email;
        avatar = user.avatar;
        profile = user.profile;
    }
    public int id { get; set; }
    public string username { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string avatar { get; set; }
    public string profile { get; set; }
}
