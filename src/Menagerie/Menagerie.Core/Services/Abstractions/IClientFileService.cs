﻿namespace Menagerie.Core.Services.Abstractions;

public interface IClientFileService : IService
{
    void SetClientFilePath(string filePath);
}