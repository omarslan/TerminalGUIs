using System.Diagnostics;
using Xunit;

namespace TerminalGUI
{
    public class Tests
    {
        [Fact]
        public async Task RunCommand_ShouldCaptureStdout()
        {
            // Arrange
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe", // Use "/bin/bash" for Linux/macOS
                Arguments = "/c echo HelloTest",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };

            // Act
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            // Assert
            Assert.Contains("HelloTest", output);
        }

        [Fact]
        public async Task RunCommand_ShouldCaptureStderr()
        {
            // Arrange
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe", // Use "/bin/bash" for Linux/macOS
                Arguments = "/c invalidCommand123",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };

            // Act
            process.Start();
            string errorOutput = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            // Assert
            Assert.NotEmpty(errorOutput);
        }
    }
}