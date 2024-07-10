using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace AEWFF_Launcher
{
    public partial class MainWindow : Window
    {
        [DllImport("ModBypass.dll")]
        private static extern int RunLauncher(char[] filePath);

        private static Config config = new Config
        {
            VanillaGamePath = "",
            ModdedGamePath = ""
        };

        public MainWindow()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void SetupButton_Click(object sender, RoutedEventArgs e)
        {

            FolderPathSelector folderPathSelector = new FolderPathSelector(config.VanillaGamePath, config.ModdedGamePath);

            folderPathSelector.ShowDialog();
            LoadConfig();
        }

        private static void LoadConfig()
        {
            if (!File.Exists("Config.json"))
            {
                SaveConfig();
            }

            string configFile = File.ReadAllText("Config.json");
            config = JsonSerializer.Deserialize<Config>(configFile) ?? config;
        }

        private static void SaveConfig()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(config, options);
            File.WriteAllText("Config.json", jsonString);
        }
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StartVanillaGame(object sender, RoutedEventArgs e)
        {
            try
            {
                string pathToExe = config.VanillaGamePath + "\\AEWFightForever.exe";

                Environment.SetEnvironmentVariable("SteamAppId", "1913210");
                Process.Start(pathToExe);

                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting process: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartModdedGame(object sender, RoutedEventArgs e)
        {
            try
            {
                string pathToExe = config.ModdedGamePath + "\\AEWFightForever\\Binaries\\Win64\\AEWFightForever-Win64-Shipping.exe";

                int errorCode = RunLauncher(pathToExe.ToCharArray());
                switch (errorCode)
                {
                    case 0:
                        Close();
                        break;
                    case 1:
                        MessageBox.Show($"Game Not Found (Error Code: {errorCode})", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        MessageBox.Show($"Unknown Error (Error Code: {errorCode})", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting process: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Config
    {
        [JsonPropertyName("VanillaGamePath")]
        public required string VanillaGamePath { get; set; }

        [JsonPropertyName("ModdedGamePath")]
        public required string ModdedGamePath { get; set; }
    }
}