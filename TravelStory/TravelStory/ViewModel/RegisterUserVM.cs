using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TravelStory.Messages;
using TravelStory.Model;
using TravelStory.Resources;
using TravelStory.View;
using Xamarin.Forms;
using static TravelStory.Model.Interfaces;

namespace TravelStory.ViewModel
{
    public class RegisterUserVM : INotifyPropertyChanged
    {
        public string PageTitle { get; private set; }

        //stringhe 
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
        public string MandatoryEntries { get; set; }
        public string LeaveEntriesBlank { get; set; }
        public string RegisterUser { get; set; }

        public bool EnableEmailEntry { get; set; }
        public bool VisibleOldPswEntry { get; set; }
        public bool EnableRegisterBtn { get; set; }

        //stringhe inserite dall'utente
        public string NameText { get; set; }
        public string SurnameText { get; set; }
        public string EmailText { get; set; }
        public string OldPasswordText { get; set; }
        public string PasswordText { get; set; }
        public string PasswordRepeatText { get; set; }

        public ICommand RegisterCmd { get; set; }

        private AzureDataService<ProfileM> AzureProfile;

        ProfileM profilePassed = null;

        public RegisterUserVM()
        {
            profilePassed = null;

            if (App.SelectedObj != null)
            {
                if (App.SelectedObj.GetType() == typeof(ProfileM))
                {
                    #region UPDATE PROFILE
                    profilePassed = (ProfileM)App.SelectedObj;
                    App.SelectedObj = null;

                    PageTitle = Dictionary.ResourceManager.GetString("EditProfile", Dictionary.Culture);
                    NameText = profilePassed.Name;
                    SurnameText = profilePassed.Surname;
                    EmailText = profilePassed.Email;
                    EnableEmailEntry = false;
                    OldPassword = Dictionary.ResourceManager.GetString("OldPassword", Dictionary.Culture) + " *";
                    VisibleOldPswEntry = true;
                    Password = Dictionary.ResourceManager.GetString("NewPassword", Dictionary.Culture) + " **";
                    PasswordRepeat = Dictionary.ResourceManager.GetString("NewPasswordRepeat", Dictionary.Culture) + " **";
                    LeaveEntriesBlank = " ** " + Dictionary.ResourceManager.GetString("LeaveEntriesBlank", Dictionary.Culture);
                    RegisterUser = Dictionary.ResourceManager.GetString("UpdateUser", Dictionary.Culture);
                    #endregion
                }
            }
            else
            {
                #region ADD USER
                PageTitle = Dictionary.ResourceManager.GetString("RegisterUser", Dictionary.Culture);
                Name = Dictionary.ResourceManager.GetString("Name", Dictionary.Culture) + " *";
                Surname = Dictionary.ResourceManager.GetString("Surname", Dictionary.Culture) + " *";
                Email = Dictionary.ResourceManager.GetString("Email", Dictionary.Culture) + " *";
                EnableEmailEntry = true;
                VisibleOldPswEntry = false;
                Password = Dictionary.ResourceManager.GetString("Password", Dictionary.Culture) + " *";
                PasswordRepeat = Dictionary.ResourceManager.GetString("PasswordRepeat", Dictionary.Culture) + " *";
                RegisterUser = Dictionary.ResourceManager.GetString("RegisterUser", Dictionary.Culture);
                #endregion
            }


            MandatoryEntries = " * " + Dictionary.ResourceManager.GetString("MandatoryEntries", Dictionary.Culture);

            EnableRegisterBtn = true;
            RegisterCmd = new Command(RegisterFunc);

            AzureProfile = new AzureDataService<ProfileM>();
        }

        private void RegisterFunc()
        {
            if (profilePassed != null)
            {
                UpdateAccount();
            }
            else
            {
                AddAccount();
            }
        }

        private async void AddAccount()
        {
            EnableRegisterBtn = false;
            RegisterUser = Dictionary.ResourceManager.GetString("CreatingAccount", Dictionary.Culture);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableRegisterBtn"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RegisterUser"));

            TrimStrings();

            if (EmailText != null && EmailText != "" && PasswordText != null && PasswordText != "" && PasswordRepeatText != null && PasswordRepeatText != "" &&
                NameText != null && NameText != "" && SurnameText != null && SurnameText != "")
            {
                bool isEmail = await CheckEmailFormat(EmailText);
                if (isEmail)
                {
                    if (PasswordText == PasswordRepeatText)
                    {
                        if (GeneralFunc.CheckInternet())
                        {
                            

                            var listEmails = await AzureProfile.CheckEmail(EmailText);
                            if (listEmails.Count() < 1)
                            {
                                var profile = new ProfileM(EmailText, PasswordText, NameText, SurnameText, "");
                                await AzureProfile.InsertRow(profile);

                                var title = Dictionary.ResourceManager.GetString("Success", Dictionary.Culture);
                                var mex = Dictionary.ResourceManager.GetString("AccountCreated", Dictionary.Culture);
                                var notificator = DependencyService.Get<IToastNotificator>();
                                notificator.Notify(ToastNotificationType.Success, title, mex, TimeSpan.FromSeconds(1));

                                MessagingCenter.Send(new AccountCreated(true), "");
                                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AccountCreated"));
                            }
                            else
                            {
                                //notifico che l'account è già presente                               
                                var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                                var mex = Dictionary.ResourceManager.GetString("SameAccount", Dictionary.Culture);
                                var notificator = DependencyService.Get<IToastNotificator>();
                                bool tapped = await notificator.Notify(ToastNotificationType.Error,
                                    title, mex, TimeSpan.FromSeconds(3));
                            }
                        }
                        else
                        {
                            //notifico che non c'è connessione internet
                            ErrorInternetConnection();
                        }
                    }
                    else
                    {
                        //notifico password non corrispondenti
                        ErrorPswMatch();
                    }
                }
                else
                {
                    //notifico che l'email non è di un formato valido
                    ErrorEmailFormat();
                }
            }
            else
            {
                //Notifico che i campi sono vuoti
                ErrorMandatoryFields();
            }

            EnableRegisterBtn = true;
            RegisterUser = Dictionary.ResourceManager.GetString("RegisterUser", Dictionary.Culture);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableRegisterBtn"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RegisterUser"));
        }

        private async void UpdateAccount()
        {
            TrimStrings();

            if (NameText != null && NameText != "" && SurnameText != null && SurnameText != "" &&
                OldPasswordText != null && OldPasswordText != "")
            {
                if (OldPasswordText == profilePassed.Password)
                {
                    if (GeneralFunc.CheckInternet())
                    {                       
                        var profile = ManageDB.mainConnection.GetProfile(profilePassed.Email);
                        profile.Name = NameText;
                        profile.Surname = SurnameText;
                        profile.Synced = false;
                        if ((PasswordText != null && PasswordText != "") || (PasswordRepeatText != null && PasswordRepeatText != ""))
                        {
                            if (PasswordText == PasswordRepeatText)
                            {
                                profile.Password = PasswordText;
                                UpdateProfile(profile);
                            }
                            else
                            {
                                //notifico password non corrispondenti
                                ErrorPswMatch();
                            }
                        }
                        else
                        {
                            profile.Password = profilePassed.Password;
                            UpdateProfile(profile);
                        }                       
                    }
                    else
                    {
                        //notifico che non c'è connessione internet
                        ErrorInternetConnection();
                    }
                }
                else
                {
                    //Notifico che la vecchia password non corrisponde
                    var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                    var mex = Dictionary.ResourceManager.GetString("ErrorOldPsWMatch", Dictionary.Culture);
                    var notificator = DependencyService.Get<IToastNotificator>();
                    bool tapped = await notificator.Notify(ToastNotificationType.Error,
                        title, mex, TimeSpan.FromSeconds(3));
                }
            }
            else
            {
                //Notifico che i campi sono vuoti
                ErrorMandatoryFields();
            }
        }

        private async void UpdateProfile(ProfileM profile)
        {
            EnableRegisterBtn = false;
            RegisterUser = Dictionary.ResourceManager.GetString("UpdatingAccount", Dictionary.Culture);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableRegisterBtn"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RegisterUser"));
           
            await AzureProfile.UpdateRow(profile);

            var connProfile = new MaintenanceDB<ProfileM>();
            profile.Synced = true;
            connProfile.SaveEdit(profile);

            GeneralFunc.Sync_Current_User(profile.Email);

            var title = Dictionary.ResourceManager.GetString("Success", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("AccountUpdated", Dictionary.Culture);
            var notificator = DependencyService.Get<IToastNotificator>();
            notificator.Notify(ToastNotificationType.Success, title, mex, TimeSpan.FromSeconds(1));

            MessagingCenter.Send(new RefreshMaster(true), "");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RefreshMaster"));

            MessagingCenter.Send(new AccountCreated(true), "");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AccountCreated"));
        }

        private void TrimStrings()
        {
            if (NameText != null)
            {
                NameText = NameText.Trim();
            }
            if (SurnameText != null)
            {
                SurnameText = SurnameText.Trim();
            }
            if (EmailText != null)
            {
                EmailText = EmailText.Trim();
            }
            if (OldPasswordText != null)
            {
                OldPasswordText = OldPasswordText.Trim();
            }
            if (PasswordText != null)
            {
                PasswordText = PasswordText.Trim();
            }
            if (PasswordRepeatText != null)
            {
                PasswordRepeatText = PasswordRepeatText.Trim();
            }
        }

        private async Task<bool> CheckEmailFormat(string email)
        {

            var response = await DependencyService.Get<IRestSharp>().VerifyEmail(email);
            return response;
            //return Regex.IsMatch(email,
            //            @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            //          + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            //          + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$",
            //            RegexOptions.IgnoreCase);
        }

        #region Error notifications

        private async void ErrorMandatoryFields()
        {
            //Notifico che i campi sono vuoti
            var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("IncMandatory", Dictionary.Culture);
            var notificator = DependencyService.Get<IToastNotificator>();
            bool tapped = await notificator.Notify(ToastNotificationType.Error,
                title, mex, TimeSpan.FromSeconds(3));
        }

        private async void ErrorEmailFormat()
        {
            //notifico che l'email non è di un formato valido
            var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("IncEmail", Dictionary.Culture);
            var notificator = DependencyService.Get<IToastNotificator>();
            bool tapped = await notificator.Notify(ToastNotificationType.Error,
                title, mex, TimeSpan.FromSeconds(3));
        }

        private async void ErrorPswMatch()
        {
            //notifico password non corrispondenti
            var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("IncPsw", Dictionary.Culture);
            var notificator = DependencyService.Get<IToastNotificator>();
            bool tapped = await notificator.Notify(ToastNotificationType.Error,
                title, mex, TimeSpan.FromSeconds(3));
        }

        private async void ErrorInternetConnection()
        {
            //notifico che non c'è connessione internet
            var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("NoInternet", Dictionary.Culture);
            var notificator = DependencyService.Get<IToastNotificator>();
            bool tapped = await notificator.Notify(ToastNotificationType.Warning,
                title, mex, TimeSpan.FromSeconds(3));
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
