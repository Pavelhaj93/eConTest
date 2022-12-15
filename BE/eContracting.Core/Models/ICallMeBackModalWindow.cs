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
        #region Dialog

        Image Image { get; set; }

        string MyPhoneNumberLabel { get; set; }

        string PreferredTimeLabel { get; set; }

        string SelectTimePlaceholder { get; set; }

        string AddNoteLabel { get; set; }

        string NoteLabel { get; set; }

        string AddAttachmentLabel { get; set; }

        string AddAnotherFileLabel { get; set; }

        string FilesNote { get; set; }

        string BottomNote { get; set; }

        #endregion

        #region Thank You

        string ThankYouTitle { get; set; }

        string ThankYouText { get; set; }

        string ThankYouButtonLabel { get; set; }

        #endregion

        #region Availability

        IWorkingDay AvailabilityMonday { get; set; }
        IWorkingDay AvailabilityThuesday { get; set; }
        IWorkingDay AvailabilityWednesday { get; set; }
        IWorkingDay AvailabilityThursday { get; set; }
        IWorkingDay AvailabilityFriday { get; set; }

        #endregion

        string OpenDialogButtonLabel { get; set; }

        #region Settings

        string SettingsDescription { get; set; }

        string SettingsProcess { get; set; }

        string SettingsProcessType { get; set; }

        /// <summary>
        /// Gets extensions delimited with comman.
        /// </summary>
        /// <example>.pdf,.png,.jpg,.jpeg,.tif,.tiff</example>
        string SettingsAllowedFileTypes { get; set; }

        string SettingsExternalSystem { get; set; }

        int SettingsMaxFileSize { get; set; }

        #endregion

        #region Erros

        string ErrorInvalidFile { get; set; }

        string ErrorTooBigFile { get; set; }

        string ErrorSentFailed { get; set; }

        string ErrorDuplicateRequest { get; set; }

        #endregion
    }
}
