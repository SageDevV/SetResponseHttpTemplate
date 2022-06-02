namespace SetResponseHttpTemplate
{

    /// <summary>
    /// ResultadoBase é uma classe que espelha o corpo da requisição Http Rest. 
    /// Para mais detalhes, veja o objeto de retorno na documentação Swagger.
    /// </summary>
    public class ResultadoBase<T>
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();
        public T Data { get; set; }
    }


}


