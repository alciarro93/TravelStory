using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Messages;
using Xamarin.Forms;

namespace TravelStory.View
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
            BindingContext = new ViewModel.LoginVM();

            RegisterUserBtn.Clicked += RegisterUserBtn_Clicked;

            EmailEntry.Completed += EmailEntry_Completed;
            PasswordEntry.Completed += PasswordEntry_Completed;
        }

        private void PasswordEntry_Completed(object sender, EventArgs e)
        {
            LoginButton.Command.Execute(null);
        }

        private void EmailEntry_Completed(object sender, EventArgs e)
        {
            PasswordEntry.Focus();
        }

        private async void RegisterUserBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterUser(true));
        }
    }
}
