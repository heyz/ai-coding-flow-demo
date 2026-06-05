using SqlSugar;

namespace SJ.BackEnd.Template.Common.DB;

public class BaseDBConfig
{
    /// <summary>
    /// 所有库配置
    /// </summary>
    public static readonly List<ConnectionConfig> AllConfigs = new();

    /// <summary>
    /// 主库的备用连接配置
    /// </summary>
    public static readonly List<ConnectionConfig> ReuseConfigs = new();

    /// <summary>
    /// 有效的库连接(除去Log库)
    /// </summary>
    public static List<ConnectionConfig> ValidConfig = new();

    public static ConnectionConfig MainConfig;
    public static ConnectionConfig LogConfig; //日志库

    public static bool IsMulti => ValidConfig.Count > 1;

}


public enum DataBaseType
{
    MySql = 0,
    SqlServer = 1,
    Sqlite = 2,
    Oracle = 3,
    PostgreSQL = 4,
    Dm = 5,
    Kdbndp = 6,
}

public class MutiDBOperate
{
    /// <summary>
    /// 连接启用开关
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 连接ID
    /// </summary>
    public string ConnId { get; set; }

    /// <summary>
    /// 从库执行级别，越大越先执行
    /// </summary>
    public int HitRate { get; set; }

    /// <summary>
    /// 连接字符串
    /// </summary>
    public string Connection { get; set; }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public DataBaseType DbType { get; set; }

    /// <summary>
    /// 从库
    /// </summary>
    public List<MutiDBOperate> Slaves { get; set; }
}
