namespace Consultoria.Api.Models.Responses
{
    public sealed class ApiResponse<T>
    {
        public bool Success { get; init; }

        public string Message { get; init; } = string.Empty;

        public T? Data { get; init; }

        public static ApiResponse<T> Ok(
            T data,
            string message = "Operación realizada correctamente.")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }
    }
}
