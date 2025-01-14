using EFCoreDemo.Data;

namespace EFCoreDemo.Profile
{
    
    public class MappingProfile1 : AutoMapper.Profile
    {
        public MappingProfile1( )
        {
            // TaxPayer Mapping
            CreateMap<TaxPayer, TaxPayerDto>()
                .PreserveReferences()
                .ReverseMap();

            // Enterprise Mapping
            CreateMap<Enterprise, EnterpriseDto>()
                .PreserveReferences()
                .ReverseMap();
            
           CreateMap<collClass, collClassDto>().PreserveReferences().ReverseMap();

            //CreateMap<List<Enterprise>,List<EnterpriseDto>>()
            //    .PreserveReferences()
            //    .ReverseMap();

            // EnterpriseBusinessActivity Mapping
            CreateMap<EnterpriseBusinessActivity, EnterpriseBusinessActivityDto>()
                .PreserveReferences()
                .ReverseMap();

            // Activity Mapping
            CreateMap<Activity, ActivityDto>()
                .PreserveReferences()
                .ReverseMap();
        }
    }

}
