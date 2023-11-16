using System.Diagnostics;
using System.Management;
using Microsoft.AspNetCore.Mvc;

namespace TodoAppApi.Controllers;

[ApiController]
[Route("api/systeminfo")]
public class SystemInfoController : ControllerBase
{
    [HttpGet("machine")]
    public ActionResult<SystemInfo> GetMachineInfo()
    {
        SystemInfo systemInfo = new SystemInfo();

        // Get machine information
        systemInfo.TotalMemory = GetTotalMemory();
        systemInfo.TotalCpuCores = Environment.ProcessorCount;

        return Ok(new {TotalMemory = systemInfo.TotalMemory.ToString("0.000"), TotalCpuCores = systemInfo.TotalCpuCores});
    }

    [HttpGet("app")]
    public ActionResult<AppInfo> GetAppInfo()
    {
        AppInfo appInfo = new AppInfo();

        // Get process information
        Process process = Process.GetCurrentProcess();
        appInfo.ProcessId = process.Id;
        appInfo.WorkingSetMemory = (double)process.WorkingSet64 / (1024 * 1024);
        appInfo.CpuUsage = GetCpuUsage();

        return Ok(appInfo);
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