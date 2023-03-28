using Microsoft.Extensions.Logging;
using Prometheus;
using Quartz;
using ServizioPillolaDigitale;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

public class FilesNumber : IJob
{
    // Conta il numero di file .zip in una cartella
    private readonly Gauge _folderZipFilesMetric;
    // Conta il numero di file PNG in una cartella
    private readonly Gauge _folderPNGFilesMetric;

    public FilesNumber()
    {
        _folderZipFilesMetric = Metrics.CreateGauge("zip_files_number",
                                                   "--- Numero di files zip nella cartella 'C:\\tempFolder'",
                                                   new GaugeConfiguration() { LabelNames = new[] { "folder_name", "root_folder", "file_type" } });

        _folderPNGFilesMetric = Metrics.CreateGauge("png_files_number",
                                                   "--- Numero di files png nella cartella 'C:\\tempFolder'",
                                                   new GaugeConfiguration() { LabelNames = new[] { "folder_name", "root_folder", "file_type" } });

    }

    public Task Execute(IJobExecutionContext context)
    {
        var jobData = context.JobDetail.JobDataMap;
        string[] baseFolders = (string[])jobData["baseFolders"];

        foreach (string path in baseFolders)
        {
            var folder = new DirectoryInfo(path);

            var zipLabels = new[] { folder.Name, folder.FullName, "zip" };
            var numberOfZipFiles = folder.EnumerateFiles().Where(f => f.Name.Contains(".zip")).Count();
            _folderZipFilesMetric.WithLabels(zipLabels).Set(numberOfZipFiles);


            var PNGLabels = new[] { folder.Name, folder.FullName, "png" };
            var numberOfPngFiles = folder.EnumerateFiles().Where(f => f.Name.Contains(".png")).Count();
            _folderPNGFilesMetric.WithLabels(PNGLabels).Set(numberOfPngFiles);
            
        }

        return Task.CompletedTask;
    }
}
