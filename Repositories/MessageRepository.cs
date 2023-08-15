namespace android_backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using android_backend.Models;

public class MessageRepository: IRepository<Message, int>
{
    private readonly MyDbContext _context;

    public MessageRepository(MyDbContext context)
    {
        _context = context;
    }
    
    public int Create(Message entity)
    {
        _context.Message.Add(entity);
         _context.SaveChanges();
        return entity.id;
    }

    public void Update(Message entity)
    {
        var oriUser = _context.Message.Single(x => x.id == entity.id);
        _context.Entry(oriUser).CurrentValues.SetValues(entity);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        _context.Message.Remove(_context.Message.Single(x => x.id == id));
        _context.SaveChanges();
    }

    public IEnumerable<Message> Find(Expression<Func<Message, bool>> expression)
    {
        return _context.Message.Where(expression);
    }

    public Message FindById(int id)
    {
        return _context.Message.SingleOrDefault(x => x.id == id);
    }
    public List<Message> FindAll()
    {
        return _context.Message.ToList<Message>();
    }

    public List<Message> FindByUserId(int userId){
        return _context.Message
        .Where(message => message.recipientId == userId && message.isReceived == false)
        .ToList();
    }

}
