﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Tests
{
    public class MemoryPageLoginModel : IPageLoginModel
    {
        public string BirthDateLabel { get; set; }
        public string BirthDatePlaceholder { get; set; }
        public string BirthDateValidationMessage { get; set; }
        public string BirthDateHelpMessage { get; set; }
        public string VerificationMethodLabel { get; set; }
        public string CalendarOpen { get; set; }
        public string CalendarNextMonth { get; set; }
        public string CalendarPreviousMonth { get; set; }
        public string CalendarSelectDay { get; set; }
        public string ButtonText { get; set; }
        public string ValidationMessage { get; set; }
        public string RequiredFields { get; set; }
        public int MaxFailedAttempts { get; set; }
        public string DelayAfterFailedAttempts { get; set; }
        public bool AutoGenerateTestableCombinationPlaceholders { get; set; }
        public IStepModel Step_Default { get; set; }
        public string PageTitle { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Path { get; set; }
        public IInfoBoxModel InfoLoginBox { get; set; }
        public string WarningDontHaveAccess { get; set; }
        public string CampaignLabel { get; set; }
        public string IndividualLabel { get; set; }
        public string ElectricityLabel { get; set; }
        public string GasLabel { get; set; }
        public string LoginView_eCat { get; set; }
        public string LoginView_eAct { get; set; }
        public string LoginView_eLab { get; set; }
        public string LoginClick_eCat { get; set; }
        public string LoginClick_eAct { get; set; }
        public string LoginClick_eLab { get; set; }
        public IStepModel Step_NoLogin { get; set; }
        public IStepModel Step_ProductChange { get; set; }
        public Guid TemplateId { get; set; }
    }
}
