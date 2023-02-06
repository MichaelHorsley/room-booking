using AutoMapper;
using commands;
using host_api.Requests;

namespace host_api.Mapping;

public class SignUpNewHostRequest_SignUpNewHostCommand_Profile : Profile
{
    public SignUpNewHostRequest_SignUpNewHostCommand_Profile()
    {
        CreateMap<SignUpNewHostRequest, SignUpNewHostCommand>();
    }
}