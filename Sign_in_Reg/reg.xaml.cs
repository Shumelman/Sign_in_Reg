using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Media.Effects;
using Sign_in_Reg.Helpers;

namespace Sign_in_Reg
{
    public partial class reg : Window
    {
        private List<Path> nameDiamond = new List<Path>();
        private Random random = new Random();
        private DispatcherTimer animationTimer;
        private int currentAnimatedIndex = 0;
        private static readonly SolidColorBrush DefaultForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4D4D4"));
        private static readonly SolidColorBrush RedForeground = Brushes.Red;
        private static readonly SolidColorBrush GreenForeground = Brushes.Green;

        public reg()
        {
            InitializeComponent();
            InitializePasswordToggle();
            WeakEventManager<Window, EventArgs>.AddHandler(this, "Loaded", OnWindowLoaded);
        }
        private void InitializePasswordToggle()
        {
            EyePasswordHelper.InitializeTiggelePasswordButton(
                b1,
                passwordBox,
                PasswordTextBox
                );
        }

        private void OnWindowLoaded(object sender, EventArgs e)
        {
            FindVisualChildren(this, nameDiamond);
            nameDiamond = nameDiamond
                .Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.StartsWith("diamond"))
                .ToList();

            StartOptimizedAnimation();
        }

        private void FindVisualChildren<T>(DependencyObject parent, List<T> results) where T : DependencyObject
        {
            if (parent == null) return;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    results.Add(result);
                FindVisualChildren(child, results);
            }
        }

        private void StartOptimizedAnimation()
        {
            // Анимируем постепенно с интервалами
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(400);
            animationTimer.Tick += (s, e) => AnimateNextDiamond();
            animationTimer.Start();
        }

        private void AnimateNextDiamond()
        {
            if (currentAnimatedIndex >= nameDiamond.Count)
            {
                animationTimer.Stop();
                return;
            }

            var diamond = nameDiamond[currentAnimatedIndex];
            currentAnimatedIndex++;

            // Простая анимация opacity
            var opacityAnimation = new DoubleAnimation
            {
                From = 0.3,
                To = 0.9,
                Duration = TimeSpan.FromSeconds(1.8),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            diamond.BeginAnimation(OpacityProperty, opacityAnimation);

            // Для некоторых алмазов добавляем легкий blur
            if (random.NextDouble() > 0.7 && diamond.Effect is BlurEffect blurEffect)
            {
                var blurAnimation = new DoubleAnimation
                {
                    From = blurEffect.Radius,
                    To = Math.Max(0.3, blurEffect.Radius * 0.6),
                    Duration = TimeSpan.FromSeconds(2.2),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };

                blurEffect.BeginAnimation(BlurEffect.RadiusProperty, blurAnimation);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            animationTimer?.Stop();
            CleanupAnimations();
            WeakEventManager<Window, EventArgs>.RemoveHandler(this, "Loaded", OnWindowLoaded);
            base.OnClosed(e);
        }

        private void CleanupAnimations()
        {
            foreach (var diamond in nameDiamond)
            {
                diamond.BeginAnimation(OpacityProperty, null);
                if (diamond.Effect is BlurEffect blurEffect)
                    blurEffect.BeginAnimation(BlurEffect.RadiusProperty, null);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(EmailBox.Text))
            {
                Email.Foreground = RedForeground;
                isValid = false;
            }

            if (string.IsNullOrEmpty(UserBox.Text))
            {
                User.Foreground = RedForeground;
                isValid = false;
            }

            if (string.IsNullOrEmpty(passwordBox.Password))
            {
                Password.Foreground = RedForeground;
                isValid = false;
            }

            if (isValid && ValidateEmailInput())
            {
                MainWindow mw = new MainWindow();
                this.Hide();
                mw.Show();
            }
        }

        private void ShowEmailMessage(string message, bool isError = true)
        {
            emailMessage.Text = message;
            emailMessage.Foreground = isError ? RedForeground : GreenForeground;

            // Анимация появления
            DoubleAnimation fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            emailMessage.Visibility = Visibility.Visible;
            emailMessage.BeginAnimation(OpacityProperty, fadeIn);

            // Таймер для скрытия
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            timer.Tick += (s, e) =>
            {
                // Анимация исчезновения
                DoubleAnimation fadeOut = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                };

                fadeOut.Completed += (ss, ee) =>
                {
                    emailMessage.Visibility = Visibility.Collapsed;
                };

                emailMessage.BeginAnimation(OpacityProperty, fadeOut);
                timer.Stop();
            };

            timer.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow mw = new MainWindow();
            mw.Show();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string NoChars = " !#$%^&*()_+={}[]|\\:;\"'<>,?/";
            ValidateInput(sender, e, NoChars, 40);
        }

        private void TextBox_PreviewSpaceInput(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private bool ValidateEmailInput()
        {
            string email = EmailBox.Text;

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowEmailMessage("Email cannot be empty");
                EmailBox.Foreground = RedForeground;
                return false;
            }

            if (!email.Contains("@"))
            {
                ShowEmailMessage("Email must contain @ symbol");
                EmailBox.Foreground = RedForeground;
                return false;
            }

            if (email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                if (!email.EndsWith(".com", StringComparison.OrdinalIgnoreCase))
                {
                    ShowEmailMessage("Gmail must end with .com");
                    EmailBox.Foreground = RedForeground;
                    return false;
                }
            }
            else
            {
                ShowEmailMessage("Please use Gmail address");
                EmailBox.Foreground = RedForeground;
                return false;
            }

            // Все проверки пройдены
            HideEmailMessage();
            EmailBox.Foreground = DefaultForeground;
            return true;
        }

        private void HideEmailMessage()
        {
            emailMessage.Visibility = Visibility.Collapsed;
        }

        private void ValidateInput(object sender, TextCompositionEventArgs e, string forbiddenChars, int maxLength)
        {
            foreach (char c in e.Text)
            {
                if (forbiddenChars.Contains(c))
                {
                    e.Handled = true;
                    return;
                }
            }

            if (sender is TextBox textBox && maxLength > 0)
            {
                if (textBox.Text.Length + e.Text.Length > maxLength)
                {
                    e.Handled = true;
                }
            }
        }

        private void PasswordBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string NoChars = "%^&*()_+={}[]|\\:;\"'<>/";
            ValidateInput(sender, e, NoChars, 0);
        }

        private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Email.Foreground = DefaultForeground;
        }

        private void UserBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            User.Foreground = DefaultForeground;
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password.Foreground = DefaultForeground;
        }

        private void passwordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Password.Foreground = DefaultForeground;
        }
    }
}