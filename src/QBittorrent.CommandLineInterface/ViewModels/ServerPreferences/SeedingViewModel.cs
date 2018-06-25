using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct SeedingViewModel
    {
        private readonly Preferences _wrappedObject;

        public SeedingViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Maximal ratio enabled")]
        public bool? MaxRatioEnabled => _wrappedObject.MaxRatioEnabled;

        [Display(Name = "Maximal ratio")]
        [DisplayFormat(DataFormatString = "{0:0.00;None;None}")]
        public double? MaxRatio => _wrappedObject.MaxRatio;

        [Display(Name = "Maximal seeding time enabled")]
        public bool? MaxSeedingTimeEnabled => _wrappedObject.MaxSeedingTimeEnabled;

        [Display(Name = "Maximal seeding time")]
        [DisplayFormat(DataFormatString = "{0:0 minutes;None;None}")]
        public int? MaxSeedingTime => _wrappedObject.MaxSeedingTime;

        [Display(Name = "Action on limit reached")]
        public MaxRatioAction? MaxRatioAction => _wrappedObject.MaxRatioAction;
    }
}
