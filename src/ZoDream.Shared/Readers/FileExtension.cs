using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZoDream.Shared.Readers
{
    public static class FileExtension
    {
        const char Separator = ',';
        public const string VideoFile = "flv,swf,mkv,avi,rm,rmvb,mpeg,mpg,ogg,ogv,mov,wmv,mp4,webm,av1";
        public const string AudioFile = "mp3,wav,mid,flac,ape,m4a";
        public const string ImageFile = "png,jpg,jpeg,gif,bmp,webp,avif";

        public static string FormatFilter(params string[] extensions)
        {
            var sb = new StringBuilder();
            foreach (var extension in extensions)
            {
                foreach (var item in extension.Split(Separator))
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        continue;
                    }
                    if (sb.Length > 0)
                    {
                        sb.Append(";");
                    }
                    sb.Append("*.");
                    sb.Append(item);
                }
            }
            return sb.ToString();
        }

        public static bool IsFile(string path, string extension)
        {
            return IsExtension(Path.GetExtension(path), extension);
        }

        public static bool IsExtension(string ext, string extension)
        {
            if (string.IsNullOrWhiteSpace(ext))
            {
                return false;
            }
            if (ext[0] == '.')
            {
                ext = ext[1..];
            }
            return extension.Split(Separator).Contains(ext);
        }
    }
}
