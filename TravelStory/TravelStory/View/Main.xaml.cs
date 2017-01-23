using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Messages;
using TravelStory.Model;
using TravelStory.ViewModel;
using Xamarin.Forms;
using static TravelStory.Model.Interfaces;

namespace TravelStory.View
{
    public partial class Main : MasterDetailPage
    {
        public Main()
        {
            InitializeComponent();

            Master = new Menu();
            MasterBehavior = MasterBehavior.Popover;
            HomeChoise();

            IsPresentedChanged += Main_IsPresentedChanged;


            MessagingCenter.Subscribe<PageSelected>(this, "", async (sender) =>
            {
                var pageType = sender.PageType;
                IsPresented = false;

                if (pageType == typeof(Home) || pageType == typeof(HomeTravel))
                {
                    if (sender.NavigationPush)
                    {
                        var page = (Page)Activator.CreateInstance(typeof(HomeTravel));
                        await Detail.Navigation.PushAsync(page);
                    }
                    else
                    {
                        HomeChoise();
                    }
                }
                else
                {
                    if (sender.NavigationPop)
                    {
                        await Detail.Navigation.PopAsync();
                    }
                    if (sender.NavigationPush)
                    {
                        Page page = new Page();
                        if (pageType == typeof(RegisterUser))
                        {
                            if (App.RegisterNewUser)
                            {
                                page = new RegisterUser(true);
                            }
                            else
                            {
                                page = new RegisterUser(false);
                            }
                            App.RegisterNewUser = false;
                        }
                        else
                        {
                            page = (Page)Activator.CreateInstance(pageType);
                        }
                        
                        await Detail.Navigation.PushAsync(page);
                    }
                    if (!sender.NavigationPop && !sender.NavigationPush)
                    {
                        Detail = new NavigationPage((Page)Activator.CreateInstance(pageType));
                    }
                }
                App.CurrentPage = Detail;
            });

            MessagingCenter.Subscribe<RefreshMaster>(this, "", (sender) =>
            {
                if (sender.RefreshMenu)
                {
                    Master = new Menu();
                }
            });

            MessagingCenter.Subscribe<CloseMaster>(this, "", (sender) =>
            {
                if (sender.CloseMasterMenu)
                {
                    IsPresented = false;
                }
            });

            MessagingCenter.Subscribe<UserLoggedOut>(this, "", (sender) =>
            {
                if (sender.LoggedOut)
                {
                    MessagingCenter.Unsubscribe<PageSelected>(this, "");
                    MessagingCenter.Unsubscribe<UserLoggedOut>(this, "");
                }
            });

        }

        private void Main_IsPresentedChanged(object sender, EventArgs e)
        {
            if (IsPresented)
            {
                DependencyService.Get<ILocalSettings>().HideKeyboard();
            }
            
        }

        void HomeChoise()
        {                      
            if (App.Current_User.ActiveTravel == "")
            {
                Detail = new NavigationPage(new Home());
            }
            else
            {
                Detail = new NavigationPage(new HomeTravel());
            }
            App.CurrentPage = Detail;
        }

    }
}
