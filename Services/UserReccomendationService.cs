using System.Diagnostics;

namespace WebAPI.Services
{
    public class UserRecommendationService
    {
        public string FetchPlaces(string args = "")
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python",                 
                Arguments = $"hello.py {args}",      
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                throw new Exception("Failed to start the Python process.");
            }

            string result = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(errors))
            {
                throw new Exception($"Python error: {errors}");
            }

            return result.Trim();
        }
    }
}
