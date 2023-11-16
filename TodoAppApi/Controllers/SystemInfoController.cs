using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Management;

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

        return Ok(systemInfo);
    }

    [HttpGet("app")]
    public ActionResult<AppInfo> GetAppInfo()
    {
        AppInfo appInfo = new AppInfo();

        // Get process information
        Process process = Process.GetCurrentProcess();
        appInfo.ProcessId = process.Id;
        appInfo.WorkingSetMemory = process.WorkingSet64;
        appInfo.CpuUsage = GetCpuUsage();

        return Ok(appInfo);
    }

    private long GetTotalMemory()
    {
        using (var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem"))
        {
            var query = searcher.Get();
            var item = query.Cast<ManagementObject>().First();
            return Convert.ToInt64(item["TotalVisibleMemorySize"]) * 1024; // Convert to bytes
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
    public long TotalMemory { get; set; }
    public int TotalCpuCores { get; set; }
}

public class AppInfo
{
    public int ProcessId { get; set; }
    public long WorkingSetMemory { get; set; }
    public float CpuUsage { get; set; }
}