using FluentValidation;
using host_api.Requests;

namespace host_api.Validation
{
    public class RegisterNewRoomRequestValidator : AbstractValidator<RegisterNewRoomRequest>
    {
        public RegisterNewRoomRequestValidator()
        {
            RuleFor(request => request.HostId).NotEmpty();
            RuleFor(request => request.RoomId).NotEmpty();
        }
    }
}