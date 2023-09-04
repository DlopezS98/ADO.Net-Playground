namespace core_data_provider;

public class SingletonSqlConnection
{
    private static SingletonSqlConnection _instance;

    //public readonly 

    private SingletonSqlConnection() { }

    public static SingletonSqlConnection Instance
    {
        get
        {
            _instance ??= new SingletonSqlConnection();
            return _instance;
        }
    }

    public void CloseConnection()
    {
        _instance.CloseConnection();
    }
}
