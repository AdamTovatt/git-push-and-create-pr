using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GitPushAndCreatePR
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the current branch name
            string branchName = GetCurrentBranchName();
            if (string.IsNullOrEmpty(branchName))
            {
                Console.WriteLine("Error: Unable to determine current branch.");
                return;
            }

            Console.WriteLine($"Current branch: {branchName}");

            // Push to remote with upstream tracking
            string pushCommand = $"git push --set-upstream origin {branchName}";
            string gitOutput = ExecuteCommand(pushCommand);

            // Extract and open the PR URL
            string urlPattern = @"https:\/\/github\.com\/\S+\/pull\/new\/\S+";
            Match match = Regex.Match(gitOutput, urlPattern);

            if (match.Success)
            {
                string prUrl = match.Value;
                Console.WriteLine($"Opening PR URL: {prUrl}");
                Process.Start(new ProcessStartInfo(prUrl) { UseShellExecute = true });
            }
            else
            {
                Console.WriteLine("Pull request URL not found.");
            }
        }

        static string GetCurrentBranchName()
        {
            string output = ExecuteCommand("git branch --show-current");
            return output.Trim();
        }

        static string ExecuteCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(psi))
            {
                if (process == null)
                {
                    Console.WriteLine("Error: Failed to start process.");
                    return string.Empty;
                }

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Error: {error}");
                }

                return output;
            }
        }
    }
}
