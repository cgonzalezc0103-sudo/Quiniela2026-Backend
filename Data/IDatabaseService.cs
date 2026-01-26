namespace Quiniela.Data
{
    public interface IDatabaseService
    {
        Task<T?> ExecuteStoredProcedureSingle<T>(string storedProcedure, object? parameters = null);
        Task<IEnumerable<T>> ExecuteStoredProcedure<T>(string storedProcedure, object? parameters = null);
        Task<int> ExecuteStoredProcedure(string storedProcedure, object? parameters = null);
    }
}