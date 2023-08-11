namespace android_backend.Service;

using android_backend.Repositories;
using android_backend.Models;
using Microsoft.IdentityModel.Tokens;

public class ContactService
{
    private readonly ContactRepository contactRepository;
    private readonly UserRepository userRepository;


    /// <summary>
    /// Initializes a new instance of the ContactService class.
    /// </summary>
    /// <param name="contactRepository">The contact repository.</param>
    /// <param name="userRepository">The user repository.</param>
    public ContactService(ContactRepository contactRepository, UserRepository userRepository)
    {
        this.contactRepository = contactRepository;
        this.userRepository = userRepository;
    }


    /// <summary>
    /// Creates a contact between two users.
    /// </summary>
    /// <param name="usernameA">The username of user A.</param>
    /// <param name="usernameB">The username of user B.</param>
    /// <returns>Returns `true` if the contact was created successfully; otherwise, `false` if either user does not exist.</returns>
    public Boolean CreateContact(String usernameA, String usernameB)
    {
        User userA = userRepository.FindByUsername(usernameA);
        User userB = userRepository.FindByUsername(usernameB);
        if (userA == null || userB == null)
            return false;
        Contact contact = new Contact
        {
            id = 0,
            userId = userA.id,
            contactUserId = userB.id
        };
        IEnumerable<Contact> contacts = contactRepository.FindById(userA.id,userB.id);
        if(contacts.IsNullOrEmpty()){
            contactRepository.Create(contact);
            return true;
        }
        else
            return false;

    }

    public Boolean DeleteContact(String usernameA, String usernameB){
        User userA = userRepository.FindByUsername(usernameA);
        User userB = userRepository.FindByUsername(usernameB);
        if (userA == null || userB == null)
            return false;
        IEnumerable<Contact> contacts = contactRepository.FindById(userA.id,userB.id);
        if(contacts.IsNullOrEmpty()){
            return false;
        }
        else{
            contactRepository.Delete(contacts.FirstOrDefault<Contact>().id);
            return true;
        }
    }

    /// <summary>
    /// Retrieves a list of contact users for a given username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>Returns a list of contact users associated with the given username, or `null` if the user does not exist.</returns>
    public List<ContactUsers>? GetContactList(String username)
    {

        User user = userRepository.FindByUsername(username);
        if (user != null)
        {
            return userRepository.FindContactUsers(username).ToList();
        }
        else
            return null;


    }
}