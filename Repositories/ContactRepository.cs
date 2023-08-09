namespace android_backend.Repositories;

using System.Linq.Expressions;
using android_backend.Models;

public class ContactRepository : IRepository<Contact, int>
{
    private readonly MyDbContext _context;

    public ContactRepository(MyDbContext context)
    {
        _context = context;
    }

    public int Create(Contact entity)
    {
        _context.Contact.Add(entity);
        _context.SaveChanges();
        return entity.id;
    }

    public void Update(Contact entity)
    {
        var oriUser = _context.Contact.Single(x => x.id == entity.id);
        _context.Entry(oriUser).CurrentValues.SetValues(entity);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        _context.Contact.Remove(_context.Contact.Single(x => x.id == id));
        _context.SaveChanges();
    }

    public IEnumerable<Contact> Find(Expression<Func<Contact, bool>> expression)
    {
        return _context.Contact.Where(expression);
    }

    public Contact FindById(int id)
    {
        return _context.Contact.SingleOrDefault(x => x.id == id);
    }

    public List<Contact> FindAll()
    {
        return _context.Contact.ToList<Contact>();
    }
}
