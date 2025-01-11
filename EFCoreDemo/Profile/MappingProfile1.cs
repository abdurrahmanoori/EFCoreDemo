using EFCoreDemo.Data;

namespace EFCoreDemo.Profile
{
    
    public class MappingProfile1 : AutoMapper.Profile
    {
        public MappingProfile1( )
        {
            // TaxPayer Mapping
            CreateMap<TaxPayer, TaxPayerDto>()
                .ReverseMap();

            // Enterprise Mapping
            CreateMap<Enterprise, EnterpriseDto>()
                .ReverseMap();

            // EnterpriseBusinessActivity Mapping
            CreateMap<EnterpriseBusinessActivity, EnterpriseBusinessActivityDto>()
                .ReverseMap();

            // Activity Mapping
            CreateMap<Activity, ActivityDto>()
                .ReverseMap();
        }
    }

}
