using AutoMapper;

namespace SOSync.Common.MapperProfiles;

public class DatabaseProfile : Profile
{
	public DatabaseProfile()
	{
		CreateMap<DbInfo, DatabaseConfig>()
			.ForMember(dest => dest.Name, map => map.MapFrom(x => x.DbName))
			.ReverseMap();
	}
}
