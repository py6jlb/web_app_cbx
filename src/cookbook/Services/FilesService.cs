using System;
using System.Security.Cryptography;
using cookbook.DTOs.Files;
using cookbook.Infrastructure.db;
using cookbook.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AppDbContext = cookbook.Infrastructure.db.AppDbContext;

namespace cookbook.Features.Files;

public sealed class FilesService
{
    private readonly AppDbContext _db;
    private readonly Filestorage _cfg;

    public FilesService(AppDbContext db, IOptions<Filestorage> cfg)
    {
        _cfg = cfg.Value;
        _db = db;
    }

    public async Task Upload(
        DTOs.Files.UploadFile request,
        CancellationToken cancellationToken = default
    )
    {
        var recipe = await _db.Recipes.FirstOrDefaultAsync(
            x => x.Id == request.RecipeId,
            cancellationToken
        );

        if (recipe is null)
        {
            throw new Exception($"Не найден рецепт");
        }

        string sha1Hash = await ComputeSha1HashAsync(request.File, cancellationToken);

        var path = Path.Combine(_cfg.Path, recipe.Id);
        var dirName = Path.GetDirectoryName(path);
        var dirInfo = Directory.CreateDirectory(path);
        var filePath = Path.Combine(path, sha1Hash);

        if (File.Exists(filePath))
        {
            throw new Exception($"A file with the name '{request.File.FileName}' already exists.");
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        // Сохраняем превью, если оно есть
        string? previewFileId = null;
        if (request.Preview != null && request.Preview.Length > 0)
        {
            var previewHash = await ComputeSha1HashAsync(request.Preview, cancellationToken);
            var previewFilePath = Path.Combine(path, previewHash);

            if (!File.Exists(previewFilePath))
            {
                using var previewStream = new FileStream(previewFilePath, FileMode.Create);
                await request.Preview.CopyToAsync(previewStream, cancellationToken);
            }

            var previewFileDto = new FileDto()
            {
                FileName = request.Preview.FileName,
                Source = previewFilePath,
                MimeType = MimeTypes.GetMimeType(request.Preview.FileName),
                IsTitle = false,
                RecipeId = recipe.Id,
                Size = request.Preview.Length,
            };
            var previewEntity = previewFileDto.ToEntity();
            await _db.Files.AddAsync(previewEntity, cancellationToken);
            previewFileId = previewEntity.Id;
        }

        var newFile = new FileDto()
        {
            FileName = request.File.FileName,
            Source = filePath,
            MimeType = MimeTypes.GetMimeType(request.File.FileName),
            IsTitle = request.IsTitle,
            RecipeId = recipe.Id,
            Size = request.File.Length,
            PreviewFileId = previewFileId,
        };
        await _db.Files.AddAsync(newFile.ToEntity(), cancellationToken);

        //сохраняем все добавленное
        await _db.SaveChangesAsync(cancellationToken);

        return;
    }

    private static async Task<string> ComputeSha1HashAsync(
        IFormFile file,
        CancellationToken cancellationToken
    )
    {
        using var stream = file.OpenReadStream();
        using var sha1 = SHA1.Create();
        byte[] hashBytes = await sha1.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexStringLower(hashBytes);
    }

    public async Task<FileDto> GetFileDescriptor(string fileId, CancellationToken cancellationToken)
    {
        var fileDescr = await _db
            .Files.Include(f => f.PreviewFile)
            .FirstOrDefaultAsync(x => x.Id == fileId, cancellationToken);
        if (fileDescr == null)
        {
            throw new Exception("Файл не найден");
        }

        var result = fileDescr.ToDto();
        return result;
    }

    public FileStream GetFileStream(string path)
    {
        if (!File.Exists(path))
        {
            throw new Exception("Файл не найден");
        }
        var stream = File.OpenRead(path);
        return stream;
    }

    public async Task DeleteFile(string fileId, CancellationToken cancellationToken)
    {
        // Загружаем файл с превью
        var fileDescr = await _db
            .Files.Include(f => f.PreviewFile)
            .FirstOrDefaultAsync(x => x.Id == fileId, cancellationToken);

        if (fileDescr == null)
        {
            throw new Exception("Файл не найден");
        }

        // Собираем все файлы для удаления (основной + превью)
        var filesToDelete = new List<Domain.Entities.File>();

        // Добавляем превью первым (оно будет удалено первым)
        if (fileDescr.PreviewFile != null)
        {
            filesToDelete.Add(fileDescr.PreviewFile);
        }

        // Добавляем основной файл
        filesToDelete.Add(fileDescr);

        // Удаляем файлы с диска
        foreach (var file in filesToDelete)
        {
            try
            {
                var filePath = Path.Combine(_cfg.Path, file.Source);
                if (File.Exists(filePath))
                {
                    var attr = File.GetAttributes(filePath);
                    if (!attr.HasFlag(FileAttributes.Directory))
                    {
                        File.Delete(filePath);
                    }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        // Удаляем из БД (все записи)
        foreach (var file in filesToDelete)
        {
            _db.Files.Remove(file);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return;
    }
}
