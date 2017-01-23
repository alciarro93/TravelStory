using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Messages;
using Xamarin.Forms;

namespace TravelStory.View
{
    public partial class RegisterUser : ContentPage
    {
        protected bool FromLogin;
        public RegisterUser(bool fromLoginPage)
        {
            InitializeComponent();
            this.BindingContext = new ViewModel.RegisterUserVM();

            FromLogin = fromLoginPage;

            RegName.Completed += RegName_Completed;
            RegSurname.Completed += RegSurname_Completed;
            RegEmail.Completed += RegEmail_Completed;
            RegOldPsw.Completed += RegOldPsw_Completed;
            RegPsw.Completed += RegPsw_Completed;
            RegPswRep.Completed += RegPswRep_Completed;

            MessagingCenter.Subscribe<AccountCreated>(this, "", async (sender) =>
            {
                if (sender.NewAccountCreated)
                {
                    if (FromLogin)
                    {
                        await Navigation.PopToRootAsync();
                        MessagingCenter.Unsubscribe<AccountCreated>(this, "");
                    }
                    else
                    {
                        MessagingCenter.Send(new PageSelected(typeof(RegisterUser),false,true), "");
                        MessagingCenter.Unsubscribe<AccountCreated>(this, "");
                    }
                }
            });
        }

        private void RegPswRep_Completed(object sender, EventArgs e)
        {
            RegCmd.Command.Execute(null);
        }

        private void RegPsw_Completed(object sender, EventArgs e)
        {
            RegPswRep.Focus();
        }

        private void RegOldPsw_Completed(object sender, EventArgs e)
        {
            RegPsw.Focus();
        }

        private void RegEmail_Completed(object sender, EventArgs e)
        {
            if (FromLogin)
            {
                RegPsw.Focus();
            }
            else
            {
                RegOldPsw.Focus();
            }
            
        }

        private void RegSurname_Completed(object sender, EventArgs e)
        {
            if (FromLogin)
            {
                RegEmail.Focus();
            }
            else
            {
                RegOldPsw.Focus();
            }
        }

        private void RegName_Completed(object sender, EventArgs e)
        {
            RegSurname.Focus();
        }
    }
}
