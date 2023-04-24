using AquestalkProj;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace YukkuriCharacterSettingsProject
{
 

    public class PartsImageInfo
    {
        public PartsImageInfo(PartsType partsType, string materialImagePath)
        {
            PartsType = partsType;
            MaterialImagePath = materialImagePath;
            FileName = Path.GetFileNameWithoutExtension(materialImagePath);
        }

        public PartsType PartsType { get; }
        public string MaterialImagePath { get; }
        public Uri MaterialImageUri => new Uri(MaterialImagePath);
        public string FileName { get; }
        
        public BitmapImage GetImageSource()
        {
            return new BitmapImage(MaterialImageUri);
        }
    }

    public class PartsStreamImageInfo : PartsImageInfo
    {
        public PartsStreamImageInfo(PartsType partsType, List<string> materialStreamImagePath) : base(partsType,materialStreamImagePath[0])
        {
            MaterialStreamImagePath = materialStreamImagePath.ConvertAll(p => new Uri(p));
        }

        public List<Uri> MaterialStreamImagePath { get; }

        public List<BitmapImage> GetStreamImageSources()
        {
            return MaterialStreamImagePath.ConvertAll(p=>new BitmapImage(p));
        }
    }



}
