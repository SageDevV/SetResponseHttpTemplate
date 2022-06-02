using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SetResponseHttpTemplate
{
    public class HttpClientResponse 
    {
        private readonly HttpClient _httpClient;

        public HttpClientResponse()
        {
        }

        public HttpClientResponse(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResultadoBase<T>> GetAsync<T>(string url, string token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

                HttpResponseMessage mensagemResponse = await _httpClient.GetAsync(url);
                ResultadoBase<T> resultadoBase = await ConverteResultadoBaseAsync<T>(mensagemResponse, url);
                return resultadoBase;

            }
            catch (Exception ex)
            {
                return new ResultadoBase<T> { Errors = new List<string> { ex.Message } };
            }
        }

        public async Task<ResultadoBase<T>> PostAsync<T>(string url, object obj, string token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

                HttpResponseMessage mensagemResponse = await _httpClient.PostAsJsonAsync(url, obj);
                ResultadoBase<T> resultadoBase = await ConverteResultadoBaseAsync<T>(mensagemResponse, url);
                return resultadoBase;

            }
            catch (Exception ex)
            {
                return new ResultadoBase<T> { Errors = new List<string> { ex.Message } };
            }
        }

        private async Task<ResultadoBase<T>> ConverteResultadoBaseAsync<T>(HttpResponseMessage respostaRequisicaoTask, string url)
        {
            var resultadoBase = new ResultadoBase<T>();
            switch (respostaRequisicaoTask.StatusCode)
            {
                case HttpStatusCode.InternalServerError:
                    resultadoBase.Errors.Add("StatusCode: 500, Erro: Erro interno no servidor ou não autorizado");
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    resultadoBase.Errors.Add($"StatusCode: 405, Erro: Metodo não encontrada \n {url}");
                    break;
                case HttpStatusCode.NotFound:
                    resultadoBase.Errors.Add($"StatusCode: 404, Erro: Url não encontrada \n {url}");
                    break;
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.OK:
                    string conteudoResponse = await respostaRequisicaoTask.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ResultadoBase<T>>(conteudoResponse);

                default:
                    resultadoBase.Errors.Add($"StatusCode: {respostaRequisicaoTask.StatusCode}");
                    break;
            }
            return resultadoBase;
        }
    }
}
