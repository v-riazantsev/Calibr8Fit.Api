namespace Calibr8Fit.Api.Interfaces.DataTransferObjects
{
    public interface IUpdateRequestDto<TKey>
    {
        TKey Id { get; init; }
    }
}