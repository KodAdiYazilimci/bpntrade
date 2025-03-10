namespace BpnTrade.Domain.Dto
{
    public class ErrorDto
    {
        public string Code { get; private set; }
        public string Message { get; private set; }
        public List<string> Details { get; private set; } = new List<string>();
        public ErrorDto InnerError { get; private set; }

        public ErrorDto(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public ErrorDto(string code, string message, ErrorDto innerError)
        {
            Code = code;
            Message = message;
            InnerError = innerError;
        }

        public ErrorDto(string code, string message, params string[] details)
        {
            Code = code;
            Message = message;
            Details.AddRange(details);
        }
    }
}
