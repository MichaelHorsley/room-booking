using AutoMapper;
using commands;
using host_api.Requests;

namespace host_api.Mapping;

public class RegisterNewHostRequest_RegisterNewHostCommand_Profile : Profile
{
    public RegisterNewHostRequest_RegisterNewHostCommand_Profile()
    {
        CreateMap<RegisterNewHostRequest, RegisterNewHostCommand>();
    }
}