namespace JobCandidate.Domain.Interfaces
{
    public interface ICacheRepository<T> where T : class
    {
        T Get(string key);
        void Set(string key, T item);
        void Remove(string key);
    }

}
