using Galaxy_Swapper_v2.Workspace.Properties;
using Galaxy_Swapper_v2.Workspace.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Galaxy_Swapper_v2.Workspace.Usercontrols
{
    public partial class LoginView : UserControl
    {
        private Storyboard Storyboard { get; set; } = default!;

        public LoginView()
        {
            InitializeComponent();
        }

        private void CharacterAnimation()
        {
            var thicknessAnimation = new ThicknessAnimation
            {
                From = new Thickness(0, 0, 0, 0),
                To = new Thickness(-5, -5, -5, -5),
                Duration = new TimeSpan(0, 0, 0, 4, 0)
            };

            Storyboard = Interface.SetThicknessAnimations(new Interface.ThicknessAnim() { Element = Character, ElementAnim = thicknessAnimation });

            Storyboard.Completed += delegate
            {
                thicknessAnimation.From = new Thickness(-5, -5, -5, -5);
                thicknessAnimation.To = new Thickness(0, 0, 0, 0);

                Storyboard = Interface.SetThicknessAnimations(new Interface.ThicknessAnim() { Element = Character, ElementAnim = thicknessAnimation });
                Storyboard.Completed += delegate
                {
                    CharacterAnimation();
                };
                Storyboard.Begin();
            };
            Storyboard.Begin();
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            CharacterAnimation();

            /*Header.Text = Languages.Read(Languages.Type.View, "LoginView", "Header");
            Activation.Text = Languages.Read(Languages.Type.View, "LoginView", "Activation");
            Activate.Content = Languages.Read(Languages.Type.View, "LoginView", "Activate");
            Tip_1.Text = Languages.Read(Languages.Type.View, "LoginView", "Tip_1");
            Tip_3.Text = Languages.Read(Languages.Type.View, "LoginView", "Tip_3");
            Tip_4.Text = Languages.Read(Languages.Type.View, "LoginView", "Tip_4");*/

            Presence.Update("Login Page");
        }

        private void Password_Focus(object sender, RoutedEventArgs e)
        {
            if (Password.Text == "Bypassed by Alejandro")
            {
                Password.Text = string.Empty;
            }
        }

        private void Password_UnFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Password.Text))
            {
                Password.Text = "Bypassed by Alejandro";
            }
        }

        private void ActivateKey_Click(object sender, RoutedEventArgs e)
        {
            Password.Text = Password.Text.Trim();
            if (Account.Activate(Password.Text))
            {
                Storyboard.Stop();
                Memory.MainView.Main.Visibility = Visibility.Visible;
                Memory.MainView.Tab_Click(Memory.MainView.Dashboard, null!);
                Memory.MainView.RemoveOverlay();
            }
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.Close();
        private void Minimize_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.WindowState = WindowState.Minimized;
        private void Drag_Click(object sender, MouseButtonEventArgs e) => Memory.MainView.DragMove();
        private void Discord_Click(object sender, MouseButtonEventArgs e) => Global.Discord.UrlStart();
        private void Key_Click(object sender, MouseButtonEventArgs e) => Global.Key.UrlStart();
    }
}