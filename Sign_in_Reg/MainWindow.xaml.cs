using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sign_in_Reg.Helpers;

namespace Sign_in_Reg
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Random _random = new Random();
        private Storyboard _storyboard;
        public MainWindow()
        {
            InitializeComponent();
            InitializeAnimations();
            InitializePasswordToggle();
        }

        private void InitializePasswordToggle()
        {
            EyePasswordHelper.InitializeTiggelePasswordButton(
                b1,
                passwordBox,
                PasswordTextBox
                );
        }

        private void InitializeAnimations()
        {
            _storyboard = new Storyboard();

            var allDiamonds = new List<(string name, double x, double y)>
            {
                ("Diamond1", 53.5, 100.0),
                ("Diamond2", 113.5, 140.0),
                ("Diamond3", 263.5, 190.0),
                ("Diamond4", 123.5, 230.0),
                ("Diamond5", 153.5, 280.0),
                ("Diamond6", 193.5, 320.0),
                ("Diamond7", 243.5, 250.0),
                ("Diamond8", 143.5, 380.0),
                ("Diamond9", 248.5, 450.0)
            };


            int count = _random.Next(3, 10);
            var randomDiamonds = allDiamonds.OrderBy(x => _random.Next()).Take(count).ToList();

            double currentDelay = 0;

            foreach (var diamond in randomDiamonds)
            {

                double delay = _random.NextDouble() * 2.0 + 0.5;

                CreateBeamAnimation(diamond.name, diamond.x, diamond.y, currentDelay);
                currentDelay += delay;
            }


            _storyboard.Completed += (s, e) =>
            {

                var beams = MainCanvas.Children.OfType<Line>().ToList();
                foreach (var beam in beams)
                    MainCanvas.Children.Remove(beam);

                InitializeAnimations(); 
            };

            _storyboard.Begin(this, true);
        }

        private void CreateBeamAnimation(string diamondName, double x, double targetY, double startDelay)
        {
            // Создаем луч
            var beam = new Line
            {
                X1 = x,
                Y1 = 520,
                X2 = x,
                Y2 = 490, 
                StrokeThickness = 2, 
                Opacity = 0,
                Stroke = new LinearGradientBrush(
                    new GradientStopCollection
                    {
                new GradientStop(Colors.White, 0),
                new GradientStop(Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF), 0.5),
                new GradientStop(Colors.Transparent, 1)
                    },
                    new Point(0, 0),
                    new Point(0, 1))
            };

            MainCanvas.Children.Add(beam);

            var diamond = (Path)FindName(diamondName);
            var diamondScale = (ScaleTransform)FindName($"{diamondName}Scale");

            //Появление луча 
            var appearAnim = new DoubleAnimation
            {
                From = 0,
                To = 0.8,
                Duration = TimeSpan.FromSeconds(0.3),
                BeginTime = TimeSpan.FromSeconds(startDelay)
            };

            //Движение луча 
            var moveAnim = new DoubleAnimation
            {
                From = 490,
                To = targetY,
                Duration = TimeSpan.FromSeconds(1.5),
                BeginTime = TimeSpan.FromSeconds(startDelay)
            };

            var moveBottomAnim = new DoubleAnimation
            {
                From = 520,
                To = targetY + 30,
                Duration = TimeSpan.FromSeconds(1.5),
                BeginTime = TimeSpan.FromSeconds(startDelay)
            };

            // Исчезновение луча 
            var disappearAnim = new DoubleAnimation
            {
                From = 0.8,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                BeginTime = TimeSpan.FromSeconds(startDelay + 1.5 - 0.3)
            };

            // Вспышка ромбика
            var flashAnim = new DoubleAnimation
            {
                From = 0.4,
                To = 1.5,
                Duration = TimeSpan.FromSeconds(0.2),
                AutoReverse = true,
                BeginTime = TimeSpan.FromSeconds(startDelay + 1.3)
            };

            var scaleAnim = new DoubleAnimation
            {
                From = 1,
                To = 1.3,
                Duration = TimeSpan.FromSeconds(0.2),
                AutoReverse = true,
                BeginTime = TimeSpan.FromSeconds(startDelay + 1.3)
            };

    
            Storyboard.SetTarget(appearAnim, beam);
            Storyboard.SetTargetProperty(appearAnim, new PropertyPath(OpacityProperty));

            Storyboard.SetTarget(moveAnim, beam);
            Storyboard.SetTargetProperty(moveAnim, new PropertyPath("(Line.Y2)"));

            Storyboard.SetTarget(moveBottomAnim, beam);
            Storyboard.SetTargetProperty(moveBottomAnim, new PropertyPath("(Line.Y1)"));

            Storyboard.SetTarget(disappearAnim, beam);
            Storyboard.SetTargetProperty(disappearAnim, new PropertyPath(OpacityProperty));

            Storyboard.SetTarget(flashAnim, diamond);
            Storyboard.SetTargetProperty(flashAnim, new PropertyPath(OpacityProperty));

            Storyboard.SetTarget(scaleAnim, diamondScale);
            Storyboard.SetTargetProperty(scaleAnim, new PropertyPath(ScaleTransform.ScaleXProperty));

       
            _storyboard.Children.Add(appearAnim);
            _storyboard.Children.Add(moveAnim);
            _storyboard.Children.Add(moveBottomAnim);
            _storyboard.Children.Add(disappearAnim);
            _storyboard.Children.Add(flashAnim);
            _storyboard.Children.Add(scaleAnim);
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(passwordBox.Password) || string.IsNullOrEmpty(UserOrEmailBox.Text))
            {
                if (string.IsNullOrEmpty(UserOrEmailBox.Text))
                    UserOrEmail.Foreground = new SolidColorBrush(Colors.Red);

                if (string.IsNullOrEmpty(passwordBox.Password))
                    PassordLabel.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Application.Current.Shutdown();
            }

        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgotPassword fop = new ForgotPassword();
            fop.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();

            reg reg = new reg();
            reg.Show();
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

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PassordLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4D4D4"));
        }

        private void TextBox_PreviewSpaceInput(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void PasswordBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string NoChars = " !#$%^&*()_+={}[]|\\:;\"'<>,?/";

            ValidateInput(sender, e, NoChars, 40);
        }

        private void passwordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PassordLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4D4D4"));
        }

        private void UserOrEmailBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string NoChars = " !#$%^&*()_+={}[]|\\:;\"'<>,?/";

            ValidateInput(sender, e, NoChars, 40);
        }

        private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserOrEmail.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4D4D4"));
        }
    }
}
