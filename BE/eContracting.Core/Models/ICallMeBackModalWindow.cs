using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using Sitecore.Data;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{97B25FDB-9468-47B3-8053-D4F758EC5E5F}", AutoMap = true)]
    public interface ICallMeBackModalWindow : IModalDialogBaseModel
    {
        Image Image { get; set; }

        string MyPhoneNumberLabel { get; set; }

        string WhenWeCanCallYouBackLabel { get; set; }

        string AddNoteLabel { get; set; }

        string NoteLabel { get; set; }

        string AddAttachmentLabel { get; set; }

        string AddAnotherFileLabel { get; set; }

        string FilesNote { get; set; }

        string BottomNote { get; set; }

        string ErrorInvalidFile { get; set; }

        string ErrorTooBigFile { get; set; }

        string ThankYouTitle { get; set; }

        string ThankYouText { get; set; }

        string ThankYouButtonLabel { get; set; }

        IWorkingDay AvailabilityMonday { get; set; }
        IWorkingDay AvailabilityThuesday { get; set; }
        IWorkingDay AvailabilityWednesday { get; set; }
        IWorkingDay AvailabilityThursday { get; set; }
        IWorkingDay AvailabilityFriday { get; set; }

        string OpenDialogButtonLabel { get; set; }
    }
}
