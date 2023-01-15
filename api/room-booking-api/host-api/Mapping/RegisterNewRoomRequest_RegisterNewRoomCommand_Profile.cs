using AutoMapper;
using commands;
using host_api.Requests;

namespace host_api.Mapping
{
    public class RegisterNewRoomRequest_RegisterNewRoomCommand_Profile : Profile
    {
        public RegisterNewRoomRequest_RegisterNewRoomCommand_Profile()
        {
            CreateMap<RegisterNewRoomRequest, RegisterNewRoomCommand>();
        }
    }
}