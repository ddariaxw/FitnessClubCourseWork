using FitnessClub.Core.Interfaces;

namespace FitnessClub.Core.Repositories
{
 
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private List<T> _items;

        public InMemoryRepository()
        {
            _items = new List<T>();
        }

        public InMemoryRepository(IEnumerable<T> initialItems)
        {
            _items = initialItems != null ? new List<T>(initialItems) : new List<T>();
        }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Елемент не може бути null.");

            _items.Add(item);
        }

        public bool Remove(T item)
        {
            if (item == null)
                return false;

            return _items.Remove(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_items);
        }

        public List<T> Find(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Критерій пошуку не може бути заданим як null.");

            return _items.FindAll(match);
        }
    }
}