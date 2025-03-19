using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TerminalGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ExecuteCommand(object sender, RoutedEventArgs e)
        {
            string command = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            OutputTextBox.Document.Blocks.Clear();
            AppendTextToOutput($"> {command}\n", Brushes.Blue);

            await RunCommand(command);
        }

        private async Task RunCommand(string command)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe", // If we were developing the tool for Linux/macOS, we would use "/bin/bash"
                    Arguments = $"/c {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process process = new Process { StartInfo = psi };
                process.OutputDataReceived += (s, e) => AppendTextToOutput(e.Data, Brushes.Green);
                process.ErrorDataReceived += (s, e) => AppendTextToOutput(e.Data, Brushes.Red);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                AppendTextToOutput($"Error: {ex.Message}\n", Brushes.Red);
            }
        }

        private void AppendTextToOutput(string text, Brush color)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Paragraph paragraph = new Paragraph(new Run(text)) { Foreground = color };
                OutputTextBox.Document.Blocks.Add(paragraph);
                OutputTextBox.ScrollToEnd();
            });
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteCommand(sender, e);
            }
        }
    }
}