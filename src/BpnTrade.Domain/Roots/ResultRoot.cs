using BpnTrade.Domain.Dto;

namespace BpnTrade.Domain.Roots
{
    public class ResultRoot
    {
        public static ResultDto Success()
        {
            return new ResultDto
            {
                IsSuccess = true,
                Error = null,
            };
        }
        public static ResultDto<T> Success<T>(T data)
        {
            return new ResultDto<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static ResultDto Failure(ErrorDto error)
        {
            return new ResultDto
            {
                IsSuccess = false,
                Error = error,
            };
        }

        public static ResultDto<T> Failure<T>(ErrorDto error)
        {
            return new ResultDto<T>
            {
                IsSuccess = false,
                Error = error,
            };
        }
    }
}
