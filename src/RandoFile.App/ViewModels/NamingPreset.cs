namespace RandoFile.App.ViewModels;

/// <summary>Which naming pattern the user selected.</summary>
public enum NamingPreset
{
    /// <summary>Image 1, Image 2, ...</summary>
    Image = 0,

    /// <summary>File 1, File 2, ...</summary>
    File = 1,

    /// <summary>A prefix typed by the user.</summary>
    Custom = 2,
}

/// <summary>Colour of the status message shown under the progress bar.</summary>
public enum StatusLevel
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
}
