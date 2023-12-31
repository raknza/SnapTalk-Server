namespace android_backend.Service;

using android_backend.Models;
using Microsoft.AspNetCore.Mvc;
using android_backend.Repositories;
using android_backend.Helper;


/// <summary>
/// Service responsible for managing user-related operations.
/// </summary>
public class UserService
{
    private readonly UserRepository userRepository;
    private readonly JwtHelper jwtHelper;
    private readonly RedisService redisService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="jwtHelper">The JWT helper.</param>
    public UserService(UserRepository userRepository, JwtHelper jwtHelper, RedisService redisService)
    {
        this.userRepository = userRepository;
        this.jwtHelper = jwtHelper;
        this.redisService = redisService;
    }



    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="parameter">The user details.</param>
    /// <returns>Returns `true` if the user was created successfully; otherwise, `false` if the username already exists.</returns>
    public Boolean Create([FromForm] User parameter)
    {
        if (userRepository.FindByUsername(parameter.username) != null)
            return false;
        User user = new User
        {
            id = 0,
            username = parameter.username,
            name = parameter.username,
            password = MD5Helper.Hash(parameter.password),
            email = parameter.email,
            avatar = parameter.avatar

        };
        userRepository.Create(user);
        return true;

    }

    /// <summary>
    /// Retrieves a user by username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>Returns the user information.</returns>
    public User GetUser(string username)
    {
        User user = userRepository.FindByUsername(username);
        if(user != null)
            return user;
        return null;
    }

    /// <summary>
    /// Retrieves a list of all users.
    /// </summary>
    /// <returns>Returns a list of users.</returns>
    public List<User> GetList()
    {
        return userRepository.FindAll();
    }

    /// <summary>
    /// Updates user information.
    /// </summary>
    /// <param name="username">The username of the user to update.</param>
    /// <param name="password">The new password (optional).</param>
    /// <param name="name">The new name (optional).</param>
    /// <param name="email">The new email (optional).</param>
    /// <param name="avatar">The new avatar URL (optional).</param>
    /// <param name="profile">The new profile information (optional).</param>
    /// <returns>Returns `true` if the user information was updated successfully.</returns>
    public Boolean UpdateUser(string username, [FromForm] string? password, [FromForm] string? name, [FromForm] string? email, [FromForm] string? avatar, [FromForm] string? profile)
    {
        User user = userRepository.FindByUsername(username);
        user.password = password == null ? user.password : MD5Helper.Hash(password);
        user.name = name ?? user.name;
        user.email = email ?? user.email;
        user.avatar = avatar ?? user.avatar;
        user.profile = profile ?? user.profile;
        userRepository.Update(user);
        return true;
    }

    /// <summary>
    /// Performs user login by verifying provided username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>
    /// A <see cref="LoginResult"/> object indicating whether the login was successful,
    /// along with an authentication token if successful; otherwise, returns a failed result.
    /// </returns>
    public LoginResult Login(string username, string password)
    {
        User user = userRepository.FindByUsername(username);
        if (user != null && user.password.Equals(MD5Helper.Hash(password))){
            LoginResult result = new LoginResult(true,jwtHelper.GenerateToken(username));
            redisService.SetString(username,result.token);
            return result;
        }
        else{
            return new LoginResult(false,null);
        }
    }


    /// <summary>
    /// Check if the provided JWT token is valid in the Redis cache for the given user.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="jwt">The JWT token to check.</param>
    /// <returns>Returns true if the token is valid; otherwise, false.</returns>
    public Boolean CheckAuth(string username, string jwt)
    {
        return jwtHelper.IsInRedis(username,jwt);
    }

}
