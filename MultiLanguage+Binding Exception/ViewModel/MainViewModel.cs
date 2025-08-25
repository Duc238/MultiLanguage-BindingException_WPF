using MultiLanguage_Binding_Exception.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiLanguage_Binding_Exception.ViewModel
{
    public class MainViewModel:BaseViewModel, INotifyDataErrorInfo
    {
        private string _userName = "";
        private LanguageOption _SelectedLanguage;
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        private ObservableCollection<LanguageOption> _Languages;
        public ObservableCollection<LanguageOption> Languages { get=>_Languages; set { _Languages = value;OnPropertyChanged(); } }

        public LanguageOption SelectedLanguage
        {
            get => _SelectedLanguage;
            set
            {
                _SelectedLanguage = value;
                OnPropertyChanged();
                SetLanguage(_SelectedLanguage.CultureCode);
                RevalidateAll();
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
                ValidateProperty(nameof(UserName));
            }
        }

        public static void SetLanguage(string cultureCode)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (cultureCode)
            {
                case "vi-VN":
                    dict.Source = new Uri("Resources/myResource.vi-VN.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("Resources/myResource.xaml", UriKind.Relative);
                    break;
            }

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        public void RevalidateAll()
        {
            ValidateProperty(nameof(UserName));
        }

        public void ValidateProperty(string propertyName)
        {
            _errors.Remove(propertyName);

            if (propertyName == nameof(UserName) && string.IsNullOrWhiteSpace(UserName))
            {
                string error = Application.Current.Resources["RequiredFieldError"] as string ?? "Required.";
                _errors[propertyName] = new List<string> { error };
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public bool HasErrors => _errors.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public IEnumerable GetErrors(string propertyName) =>
            propertyName != null && _errors.ContainsKey(propertyName)
                ? _errors[propertyName]
                : Enumerable.Empty<string>();
        public MainViewModel()
        {
            Languages = new ObservableCollection<LanguageOption>()
            {
                new LanguageOption { Name = "Tiếng Việt", CultureCode = "vi-VN" },
                new LanguageOption { Name = "English", CultureCode = "en-US" },
            };
            SelectedLanguage = Languages.First();
        }


    }
}
