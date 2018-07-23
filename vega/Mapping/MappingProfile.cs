using System.Linq;
using AutoMapper;
using vega.Controllers.Resources;
using vega.Models;

namespace vega.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to API Resource
            CreateMap<Make, MakeResource>();
            CreateMap<Model, ModelResource>();
            CreateMap<Feature, FeatureResource>();
            CreateMap<Vehicle, VehicleResource>()
              .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(vf => vf.FeatureId)));


             // API Resource to Domain
             CreateMap<VehicleResource,Vehicle>()
             .ForMember(v => v.Contact, opt => opt.MapFrom(vr => new Contact
             {
                 ContactName = vr.Contact.Name,
                 ContactEmail = vr.Contact.Email,
                 ContactPhone = vr.Contact.Phone
             })).AfterMap((vr, v) => {
                // Remove unselected features
                var removedFeatures = v.Features.Where(f => !vr.Features.Contains(f.FeatureId));
                foreach (var f in removedFeatures)
                  v.Features.Remove(f);

                // Add new features
                var addedFeatures = vr.Features.Where(id => !v.Features.Any(f => f.FeatureId == id)).Select(id => new VehicleFeature { FeatureId = id });   
                foreach (var f in addedFeatures)
                    v.Features.Add(f);
            });
             
        }
    }
}