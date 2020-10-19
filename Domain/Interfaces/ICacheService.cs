namespace Domain.Interfaces
{
    public interface ICacheService
    {
        object Get(string cacheKey);

        bool Set(string cacheKey, object value);

        void Remove(string cacheKey);
    }
}
