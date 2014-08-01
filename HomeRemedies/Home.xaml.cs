using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace HomeRemedies
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Home : HomeRemedies.Common.LayoutAwarePage
    {
        public Home()
        {
            this.InitializeComponent();
            Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView().CommandsRequested += GroupedItemsPage_CommandsRequested;

        }

        void GroupedItemsPage_CommandsRequested(Windows.UI.ApplicationSettings.SettingsPane sender, Windows.UI.ApplicationSettings.SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Clear();

            Windows.UI.ApplicationSettings.SettingsCommand privacyInfo = new Windows.UI.ApplicationSettings.SettingsCommand(
                        "PrivacyInfo",
                        "Privacy Policy",
                        (UICommandInvokedHandler) =>
                        {
                            var md1 = new MessageDialog("Home Remedies and/or its affiliates may use personal information and monitor website activity generally, to collect information on how this Site is used and the volume and frequecy of access so that we may improve this Site , as well as Home Remedies's related services. Home Remedies will not provide your information to unrelated third parties without your prior express consent, except as permitted under this Privacy Policy. \n\nHome Remedies will co-operate with the governmental authorities and comply with all court orders involving requests for personal information. \n\nThe personal information Home Remedies collects on this Site is an asset of Home Remedies ‘s business and in the event that Home Remedies, or substantially all of its assets, are acquitted or become the subject of a merger, such information may be transferred. \n\nPlease contact Home Remedies if you have any questions concerning Home Remedies’s privacy policy, the way Home Remedies uses your personal information or if you would like to review your personal information that Smith’s has collected in order to update or delete it from our system. \n\nThis privacy policy applies to information collected over this Site. Home Remedies reserves the right to apply different policies to information collected off-line.");
                            md1.Commands.Add(new UICommand("OK", (UICommand) =>
                            {

                            }));
                            md1.ShowAsync();
                        });
            args.Request.ApplicationCommands.Add(privacyInfo);
            Windows.UI.ApplicationSettings.SettingsCommand privacyInfo2 = new Windows.UI.ApplicationSettings.SettingsCommand(
                       "PrivacyInfo XYZ",
                       "Our Team",
                       (UICommandInvokedHandler) =>
                       {
                           var md1 = new MessageDialog("Home Remedies created by :\n\nJay Shah\nPooja Shah\nTapan Desai\nPratik Mehta");
                           md1.Commands.Add(new UICommand("OK", (UICommand) =>
                           {

                           }));
                           md1.ShowAsync();
                       });
            args.Request.ApplicationCommands.Add(privacyInfo2);
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ItemsPage), "AllGroups");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Zest));
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri("mailto:?to=" + "zestsolutions.india@gmail.com" + "&subject=Home Medications Feedback&body=");
            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }
    }
}
