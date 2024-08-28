internal class PromptBuilderResult
    {
        string _imagePath = "";

        public string? Prompt { get; set; }
        public bool ImageFound { get; set; }
        public string ImagePath
        {
            get
            {

                if (Uri != null && Uri.IsFile)
                {
                    _imagePath = Uri.LocalPath;
                }
                else if (string.IsNullOrEmpty(_imagePath) && ImageBytes != null && ImageBytes.Length > 0)
                {
                    _imagePath = Path.GetTempFileName();
                    if (File.Exists(_imagePath))
                    {
                        File.Delete(_imagePath);
                    }
                    File.WriteAllBytes(_imagePath, ImageBytes ?? new byte[0]);
                }

                return _imagePath;
            }
            set
            {
                _imagePath = value;
            }
        }
        public byte[]? ImageBytes { get; internal set; }
        public Uri? Uri { get; internal set; }
    }