using SOSyncAbstractions.DTO;

using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace SOSyncCommon;

public static class DatabaseConfigExtensions
{
    public static string GetConnectionString(this DatabaseConfig databaseConfig)
    {
        var connBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseConfig.Host,
            Database = databaseConfig.Name,
            Port = Convert.ToInt32(databaseConfig.Port),
            Username = databaseConfig.User,
            Password = databaseConfig.Password,
            Pooling = false,
            Encoding = "LATIN1",
            ClientEncoding = "LATIN1"
        };

        return connBuilder.ConnectionString;
    }

    public static NpgsqlConnection GetDbConnection(this DatabaseConfig databaseConfig)
    {
        var conn =  new NpgsqlConnection(GetConnectionString(databaseConfig));
        if (conn.State != ConnectionState.Open)
            conn.Open();
        return conn;
    }
    public static async Task<NpgsqlConnection> GetDbConnectionAsync(this DatabaseConfig databaseConfig)
    {
        var conn = new NpgsqlConnection(GetConnectionString(databaseConfig));
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();
        return conn;
    }

    public static bool HasConnection(this DatabaseConfig databaseConfig, ILogger<object>? logger = null)
    {
        try
        {
            // Checking first if service is running.
            var client = new TcpClient();
            if (!client
                .ConnectAsync(databaseConfig.Host, int.Parse(databaseConfig.Port))
                .Wait(2000))
                throw new SocketException();
            using var connection = databaseConfig.GetDbConnection();
            var cmd = new NpgsqlCommand("select version();", connection);
            cmd.Parameters.Add(
                new NpgsqlParameter("version", DbType.String)
                {
                    Direction = ParameterDirection.Output
                });
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            return true;
        }
        catch (SocketException)
        {
            logger?.LogError("Não foi possível conectar ao endereço fornecido!");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
        }
        return false;
    }
    public static void KillSync(this DatabaseConfig databaseConfig)
    {
        try
        {
            string query = @"SELECT pid,estacao, ts as time_started from usuario_pid where modulo='sync';";
            using var conn = databaseConfig.GetDbConnection();
            var usuarioPIDs = conn.Query<UsuarioPID>(query);
            foreach (var usuarioPID in usuarioPIDs)
                conn.Execute($"select pg_terminate_backend({usuarioPID.PID});");

            conn.Execute("DELETE FROM usuario_pid WHERE modulo='sync';");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Erro : ", ex.Message);
        }
    }
    public static async Task KillSyncAsync(this DatabaseConfig databaseConfig)
    {
        try
        {
            string query = @"SELECT pid,estacao, ts as time_started from usuario_pid where modulo='sync';";
            using var conn = await databaseConfig.GetDbConnectionAsync();
            var usuarioPIDs = await conn.QueryAsync<UsuarioPID>(query);

            foreach (var usuarioPID in usuarioPIDs)
                await conn.ExecuteAsync($"select pg_terminate_backend({usuarioPID.PID});");

            await conn.ExecuteAsync("DELETE FROM usuario_pid WHERE modulo='sync';");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Erro : ", ex.Message);
        }
    }
    public static void KillUpdates(this DatabaseConfig databaseConfig)
    {
        try
        {
            using var conn = databaseConfig.GetDbConnection();

            string query = @"SELECT pid,estacao, ts as time_started from usuario_pid where modulo='manutencao';";
            var usuarioPIDs = conn.Query<UsuarioPID>(query);

            foreach (var usuarioPID in usuarioPIDs)
                conn.Execute($"select pg_terminate_backend({usuarioPID.PID});");

            conn.Execute("DELETE FROM usuario_pid WHERE modulo='sync';");
            conn.Execute("DELETE FROM mutex_db WHERE processo='manutencao';");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Erro : " + ex.Message);
        }
    }
    public static async Task KillUpdatesAsync(this DatabaseConfig databaseConfig)
    {
        try
        {
            using var conn = await databaseConfig.GetDbConnectionAsync();

            string query = @"SELECT pid,estacao, ts as time_started from usuario_pid where modulo='manutencao';";
            var usuarioPIDs = await conn.QueryAsync<UsuarioPID>(query);

            foreach (var usuarioPID in usuarioPIDs)
                await conn.ExecuteAsync($"select pg_terminate_backend({usuarioPID.PID});");

            await conn.ExecuteAsync("DELETE FROM usuario_pid WHERE modulo='sync';");
            await conn.ExecuteAsync("DELETE FROM mutex_db WHERE processo='manutencao';");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Erro : " + ex.Message);
        }
    }
    public static async Task ClearTablesAsync(this DatabaseConfig databaseConfig, bool replaceSyncFiles)
    {
        var queries = new string[]
        {
            "TRUNCATE TABLE usuario_log_flow;",
            "DELETE FROM usuario_log WHERE usuario=-1;",
            "TRUNCATE TABLE empresa_distribuidora_flow;",
            $"UPDATE pgd_hosts SET syncftpoverwrite = {replaceSyncFiles}; "
        };

        try
        {
            using var conn = await databaseConfig.GetDbConnectionAsync();

            foreach (var query in queries)
                await conn.ExecuteAsync(query);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Erro : " + ex.Message);
        }
    }
    public static void ClearTables(this DatabaseConfig databaseConfig, bool replaceSyncFiles)
    {
        var queries = new string[]
        {
            "TRUNCATE TABLE usuario_log_flow;",
            "DELETE FROM usuario_log WHERE usuario=-1;",
            "TRUNCATE TABLE empresa_distribuidora_flow;",
            $"UPDATE pgd_hosts SET syncftpoverwrite = {replaceSyncFiles}; "
        };

        try
        {
            using var conn = databaseConfig.GetDbConnection();

            foreach (var query in queries)
                conn.Execute(query);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Erro : " + ex.Message);
        }
    }

    public static async Task ExecuteContaSaldoCheckAsync(this DatabaseConfig databaseConfig)
    {
        string queryLogTable = "LOCK TABLE conta_saldo_invalida IN SHARE MODE;";
         DateTime data_ativ;
        DateTime started_time = DateTime.Now;

        using var conn = databaseConfig.GetDbConnection();
        using var transaction = conn.BeginTransaction();
        await conn.ExecuteAsync(queryLogTable);

        var contasSaldo = await conn.QueryAsync<ContaSaldoInvalida>("SELECT empresa, conta, data from conta_saldo_invalida;");

        var saldo_invalida = await conn.QueryFirstAsync<int>("SELECT count(*) FROM conta_saldo_invalida");

        if (saldo_invalida > 0)
        {
            var processedAccount = 1;
            try
            {
                foreach (var contaSaldo in contasSaldo)
                {
                    Debug.WriteLine($"Processando registro {processedAccount} de {saldo_invalida} => Empresa: {contaSaldo.Empresa} - Conta: {contaSaldo.Conta} - Data: {contaSaldo.Data}");

                    await conn.ExecuteAsync($"PERFORM conta_saldo_empresa_calcula_f({contaSaldo.Empresa}, {contaSaldo.Conta}, {contaSaldo.Data});");

                    processedAccount++;
                }
                await conn.ExecuteAsync("DELETE FROM conta_saldo_invalida;");
                var querySaldoControle = new StringBuilder()
                    .Append("SELECT  data_ativacao as DataAtivacao, quantidade_exec as QuantidadeExec, data_ult_exec as DataUltimaExec ")
                    .Append("FROM conta_saldo_controle ")
                    .Append("ORDER BY data_ult_exec DESC ")
                    .Append("limit 1");
                var contaSaldoControle = await conn.QueryFirstOrDefaultAsync<ContaSaldoControle>(querySaldoControle.ToString());

                if (contaSaldoControle is not null)
                {
                    if (contaSaldoControle.DataUltimaExec == DateTime.Today)
                    {
                        var queryUpdateSaldoControle = $"update conta_saldo_controle " +
                            $"set data_ult_exec = timeofday()::timestamp, duracao_ult_exec = (timeofday()::timestamp - current_timestamp), " +
                            $"quantidade_exec = {processedAccount}, quantidade_reg = {saldo_invalida};";
                        await conn.ExecuteAsync(queryUpdateSaldoControle);
                    }
                }
                else
                {
                    data_ativ = DateTime.Now;
                    var sqlInsertContaControle = $@"insert into conta_saldo_controle(ativo, data_ativacao, data_ult_exec, duracao_ult_exec, quantidade_exec, quantidade_reg) 
                                values(true, {data_ativ}, timeofday()::timestamp, (timeofday()::timestamp - current_timestamp), {processedAccount}, {saldo_invalida});";
                    await conn.ExecuteAsync(sqlInsertContaControle);
                }
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }

            Debug.WriteLine($"Tempo de execucao: {(DateTime.Now - started_time).TotalSeconds} segundos .");
        }
        else
        {
            Debug.WriteLine("Nenhuma conta a processar !");
        }
    }
    public static async Task<List<Sync>> ListSyncStatus(this DatabaseConfig databaseConfig, string order_by = "atraso DESC")
    {
        string query = $@"WITH sync_status AS
                                  (SELECT h.hostname as Conexao, p.codigo || ' - ' || p.nome AS empresa,
                                          fs.ts AS LastUpdate,
                                     (SELECT last_value
                                      FROM pgd_fid_seq)-fs.gfid AS atraso
                                   FROM pgd_flow_sync fs
                                   JOIN pgd_hosts h ON(fs.sid = h.sid)
                                   JOIN pessoa p ON (p.grid = h.pessoa)
                                   WHERE fs.sid >= 0
                                   ORDER BY {order_by}) 
                                   SELECT 
                                   CASE 
	                                WHEN atraso >= 1000000 THEN 'danger'
	                                WHEN atraso >= 500000 THEN 'minus'
	                                WHEN atraso >= 200000 THEN 'warning'
	                                WHEN atraso >= 20000 THEN 'circle_blue'
	                                ELSE 'circle_green'
	                                END as status, conexao, empresa, atraso, LastUpdate FROM sync_status";
        try
        {
            using var client = await databaseConfig.GetDbConnectionAsync();
            return (List<Sync>)await client.QueryAsync<Sync>(query);            
        }
        catch (Exception) { }

        return new List<Sync>();
    }
}
