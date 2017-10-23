using System;
using System.IO;
using MigraDoc.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PdfSharp.ImageSharp
{
    public class NetFrameworkImageSource : ImageSource
    {
        protected override IImageSource FromBinaryImpl(string name, Func<byte[]> imageSource, int? quality = 75)
        {
            return new NetFrameworkImageSourceImpl(name, () =>
            {
                using (var ms = new MemoryStream(imageSource.Invoke()))
                    return Image.FromStream(ms);
            }, (int)quality);
        }

        protected override IImageSource FromFileImpl(string path, int? quality = 75)
        {
            return new NetFrameworkImageSourceImpl(path, () =>
            {
                return Image.FromFile(path);
            }, (int)quality);
        }

        protected override IImageSource FromStreamImpl(string name, Func<Stream> imageStream, int? quality = 75)
        {
            return new NetFrameworkImageSourceImpl(name, () =>
            {
                using (var stream = imageStream.Invoke())
                {
                    return Image.FromStream(imageStream.Invoke());
                }
            }, (int)quality);
        }

        private class NetFrameworkImageSourceImpl : IImageSource
        {
            private Func<Image> _getImage;
            public int Width => Image.Width;
            public int Height => Image.Height;
            public string Name { get; }

            private Image _image;
            private readonly int? _quality;

            private Image Image
            {
                get
                {
                    if (_image == null)
                    {
                        _image = _getImage.Invoke();
                    }
                    return _image;
                }
            }

            public NetFrameworkImageSourceImpl(string name, Func<Image> getImage, int quality)
            {
                Name = name;
                _getImage = getImage;
                _quality = quality;
            }

            public void SaveAsJpeg(MemoryStream ms)
            {
                var paras = new EncoderParameters();
                ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders().Where(x => x.FormatID == ImageFormat.Jpeg.Guid).First();
                var param = new EncoderParameters().Param[0];
                paras.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)_quality);
                Image.Save(ms, jpgEncoder, paras);
            }

            public void Dispose()
            {
                Image.Dispose();
            }
        }
    }
}
