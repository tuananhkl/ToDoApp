using System.Diagnostics;
using System.Management;
using Microsoft.AspNetCore.Mvc;

namespace TodoAppApi.Controllers;

[ApiController]
[Route("api/systeminfo")]
public class SystemInfoController : ControllerBase
{
    private readonly ILogger<SystemInfoController> _logger;

    public SystemInfoController(ILogger<SystemInfoController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("machine")]
    public ActionResult<SystemInfo> GetMachineInfo()
    {
        try
        {
            SystemInfo systemInfo = new SystemInfo();

            // Get machine information
            systemInfo.TotalMemory = GetTotalMemory();
            systemInfo.TotalCpuCores = Environment.ProcessorCount;
            return Ok(new {TotalMemory = systemInfo.TotalMemory.ToString("0.000"), TotalCpuCores = systemInfo.TotalCpuCores});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal Server Error");
        }

        
    }

    [HttpGet("app")]
    public ActionResult<AppInfo> GetAppInfo()
    {
        try
        {
            AppInfo appInfo = new AppInfo();

            // Get process information
            Process process = Process.GetCurrentProcess();
            appInfo.ProcessId = process.Id;
            appInfo.WorkingSetMemory = (double)process.WorkingSet64 / (1024 * 1024);
            appInfo.CpuUsage = GetCpuUsage();

            return Ok(appInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Internal Server Error");
        }
    }

    private double GetTotalMemory()
    {
        using (var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem"))
        {
            var query = searcher.Get();
            var item = query.Cast<ManagementObject>().First();
            return (Convert.ToDouble(item["TotalVisibleMemorySize"]) * 1024) / (1024 * 1024 * 1024); // Convert to bytes
        }
    }

    private float GetCpuUsage()
    {
        using (var searcher = new ManagementObjectSearcher("SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name='_Total'"))
        {
            var query = searcher.Get();
            var item = query.Cast<ManagementObject>().First();
            return Convert.ToSingle(item["PercentProcessorTime"]);
        }
    }

    #region Linux

    [HttpGet("cpuusage")]
    public ActionResult<double> GetCpuUsageLinux()
    {
        try
        {
            double cpuUsage = GetCpuUsageValue();
            return Ok(cpuUsage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Failed to get CPU usage. Error: {ex.Message}");
        }
    }

    [HttpGet("memoryusage")]
    public ActionResult<double> GetMemoryUsageLinux()
    {
        try
        {
            double memoryUsage = GetMemoryUsageValue();
            return Ok(memoryUsage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Failed to get memory usage. Error: {ex.Message}");
        }
    }

    private double GetCpuUsageValue()
    {
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "bash",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        })
        {
            process.Start();

            process.StandardInput.WriteLine("top -b -n 1 | grep '%Cpu(s)' | awk '{print $2}'");
            process.StandardInput.Close();

            string result = process.StandardOutput.ReadToEnd().Trim();

            process.WaitForExit();

            if (double.TryParse(result, out double cpuUsage))
            {
                return cpuUsage;
            }

            return -1; // Error in parsing or retrieving CPU usage
        }
    }

    private double GetMemoryUsageValue()
    {
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "bash",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        })
        {
            process.Start();

            process.StandardInput.WriteLine("free -m | awk '/Mem:/ {print $3}'");
            process.StandardInput.Close();

            string result = process.StandardOutput.ReadToEnd().Trim();

            process.WaitForExit();

            if (double.TryParse(result, out double memoryUsage))
            {
                return memoryUsage;
            }

            return -1; // Error in parsing or retrieving memory usage
        }
    }

    #endregion
}

public class SystemInfo
{
    public double TotalMemory { get; set; }
    public int TotalCpuCores { get; set; }
}

public class AppInfo
{
    public int ProcessId { get; set; }
    public double WorkingSetMemory { get; set; }
    public float CpuUsage { get; set; }
}