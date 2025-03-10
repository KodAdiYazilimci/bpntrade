namespace BpnTrade.Domain.Dto
{
    public class ResultDto
    {
        public bool IsSuccess { get; set; }
        public ErrorDto? Error { get; set; }
    }

    public class ResultDto<T> : ResultDto
    {
        public T Data { get; set; }
    }
}
