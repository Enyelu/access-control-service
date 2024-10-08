﻿using access_control.core.Commands.Lock;
using access_control.core.Commands.Permission;
using access_control.core.DataTransferObjects;
using access_control.core.Queries.Event;
using access_control.core.Queries.Lock;
using access_control.core.Queries.Permission;
using access_control.domain.Entities;
using AutoMapper;

namespace access_control.core.Profiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<HandleCreatePermission.CommandExtention, Permission>()
                .ForMember(x => x.TenantId, options => options.MapFrom((m, _1, _2, ctx) => (string)ctx.Items["TenantId"]))
                .ForMember(x => x.CreatedBy, options => options.MapFrom((m, _1, _2, ctx) => (string)ctx.Items["CreatedBy"]))
                .ReverseMap();

            CreateMap<HandleDeletePermission.Command, DeletePermissionDto>().ReverseMap();
            CreateMap<HandlePermissionByTenantId.Result, Permission>().ReverseMap();
            CreateMap<HandleFetchEvents.Result, EventLog>().ReverseMap();
            CreateMap<HandleCreateLock.Command, Lock>()
                .ForMember(x => x.CreatedBy, options => options.MapFrom((m, _1, _2, ctx) => (string)ctx.Items["CreatedBy"]))
                .ReverseMap();

            CreateMap<HandleFetchAllocatedLocks.Result, Lock>().ReverseMap();
            CreateMap<HandleRaiseComplaint.Command, Complaint>()
                .ForMember(x => x.CreatedBy, options => options.MapFrom((m, _1, _2, ctx) => (string)ctx.Items["CreatedBy"]))
                .ReverseMap();
            CreateMap<HandleRaiseComplaint.Command, RaiseComplaintDto>().ReverseMap();


            CreateMap<HandleViewComplaint.Result, Complaint>().ReverseMap();
        }
    }
}