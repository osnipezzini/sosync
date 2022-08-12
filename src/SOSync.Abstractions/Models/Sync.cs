namespace SOSync.Abstractions.Models
{
    public class Sync
    {
        public string Conexao { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Empresa { get; set; } = string.Empty;
        public int Atraso {get; set;}
        public DateTime LastUpdate { get; set; }

        public override string ToString() => Conexao;
    }
}
