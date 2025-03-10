namespace BpnTrade.Domain.Dto
{
    public class ErrorDto
    {
        public string Code { get; private set; }
        public string Message { get; private set; }

        public ErrorDto(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
