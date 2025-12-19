namespace Telefin.Common.Models
{
    internal class TelegramResponse
    {
        public TelegramResponse(bool ok, int? errorCode, string? description)
        {
            Ok = ok;
            ErrorCode = errorCode;
            Description = description;
        }

        public bool Ok { get; }

        public int? ErrorCode { get; }

        public string? Description { get; }
    }
}
