using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FarmDssProfile : MainProfile
    {
        public FarmDssProfile()
        {
            // Entities to Dtos
            CreateMap<Farm, FarmDssDto>()
            .AfterMap((src, dest, context) =>
                {
                    // Only select first Weather Station or Weather Data Source, because current requirements only accepts  
                    // one per farm. This might change in the future
                    if (src.FarmWeatherDataSources.Any())
                        dest.WeatherDataSourceDto = context
                            .Mapper
                            .Map<WeatherDataSourceDto>(
                                src.FarmWeatherDataSources.FirstOrDefault().WeatherDataSource);

                    if (src.FarmWeatherStations.Any())
                        dest.WeatherStationDto = context
                            .Mapper
                            .Map<WeatherStationDto>(
                                src.FarmWeatherStations.FirstOrDefault().WeatherStation);

                    // Get last field        
                    if (src.Fields.Any())
                    {
                        var lastField = src.Fields.LastOrDefault();
                        dest.FieldId = lastField.Id;
                        dest.FieldName = lastField.Name;

                        // if (lastField.FieldCropPests.Any())
                        // {
                        //     var fieldCropPestDss = lastField.FieldCropPests.LastOrDefault().FieldCropPestDsses.FirstOrDefault();
                        //     dest.FieldCropPestDssDto = context
                        //         .Mapper
                        //         .Map<FieldCropPestDssDto>(fieldCropPestDss);

                        //     dest.DssId = fieldCropPestDss.CropPestDss.DssId;
                        // }
                    }
                }); ;
        }
    }
}