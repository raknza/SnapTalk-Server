using System.Linq.Expressions;
using android_backend.Models;

namespace android_backend.Repositories
{
    public class UserRepository : IRepository<User, int>
    {
        private readonly MyDbContext _context;

        public UserRepository(MyDbContext context)
        {
            _context = context;
        }

        public int Create(User entity)
        {
            _context.User.Add(entity);
            _context.SaveChanges();
            return entity.id;
        }

        public void Update(User entity)
        {
            var oriUser = _context.User.Single(x => x.id == entity.id);
            _context.Entry(oriUser).CurrentValues.SetValues(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            _context.User.Remove(_context.User.Single(x => x.id == id));
            _context.SaveChanges();
        }

        public IEnumerable<User> Find(Expression<Func<User, bool>> expression)
        {
            return _context.User.Where(expression);
        }

        public User FindById(int id)
        {
            return _context.User.SingleOrDefault(x => x.id == id);
        }

        public User FindByUsername(String username)
        {
            return _context.User.SingleOrDefault(x => x.username == username);
        }

        public List<User> FindAll()
        {
            return _context.User.ToList<User>();
        }
    }
}