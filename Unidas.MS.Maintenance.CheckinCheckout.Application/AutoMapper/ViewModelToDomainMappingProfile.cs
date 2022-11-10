using AutoMapper;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ItemCheckinCheckoutRequestViewModel, CaseManagementServices.WorkshopCheck>()
                .ForMember(x => x.DriverId, y => y.MapFrom(z => FormatDocument(z.Document)))
                .ForMember(x => x.WorkshopCustomerConfirmation, y => y.MapFrom(z => FormatBooleanYesNo(z.WorkshopCustomerConfirmation)))
                .ForMember(x => x.WorkshopServiceCompleted, y => y.MapFrom(z => FormatBooleanYesNo(z.WorkshopServiceCompleted)));
        }

        private string FormatDocument(string document)
        {
            if (!string.IsNullOrEmpty(document))
            {
                document = document.Replace(".", "");
                document.Insert(document.Length - 2, "-");
            }

            return document;
        }

        private int FormatBooleanYesNo(bool yesNo)
        {
            return yesNo ? 1 : 0;
        }
    }
}
