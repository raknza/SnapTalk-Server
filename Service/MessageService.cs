namespace android_backend.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using android_backend.Models;
using android_backend.Repositories;

public class MessageService
{
    private readonly MessageRepository messageRepository;
    private readonly UserRepository userRepository;

    public MessageService(MessageRepository messageRepository, UserRepository userRepository)
    {
        this.messageRepository = messageRepository;
        this.userRepository = userRepository;
    }

    public List<Message> GetMessage(String username){
        User user = userRepository.FindByUsername(username);
        return messageRepository.FindByUserId(user.id);
    }

    public Boolean ReceivedMessage(int id){
        Message message = messageRepository.FindById(id);
        if(message == null)
            return false;
        message.isReceived = true;
        messageRepository.Update(message);
        return true;
    }

    public void ReceivedAllMessage(String username){
        User user = userRepository.FindByUsername(username);
        List<Message> messages = messageRepository.FindByUserId(user.id);
        for(int i=0;i<messages.Count;i++){
            messages[i].isReceived = true;
            messageRepository.Update(messages[i]);
        }
    }
}