namespace SOSync.Abstractions.DTO
{
    public class ContaSaldoInvalida
    {
        public long Empresa { get; set; }
        public string Conta { get; set; } = "";
        public DateTimeOffset Data { get; set; }
    }
}
