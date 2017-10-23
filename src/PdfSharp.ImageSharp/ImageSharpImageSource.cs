using System;
using System.IO;
using SixLabors.ImageSharp;
using MigraDoc.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace PdfSharp.ImageSharp
{
    public class ImageSharpImageSource : ImageSource
    {
        protected override IImageSource FromBinaryImpl(string name, Func<byte[]> imageSource, int? quality = 75)
        {
            return new ImageSharpImageSourceImpl<Rgba32>(name, () =>
            {
                return Image.Load(imageSource.Invoke());
            }, (int)quality);
        }

        protected override IImageSource FromFileImpl(string path, int? quality = 75)
        {
            return new ImageSharpImageSourceImpl<Rgba32>(path, () =>
            {
                return Image.Load(path);
            }, (int)quality);
        }

        protected override IImageSource FromStreamImpl(string name, Func<Stream> imageStream, int? quality = 75)
        {
            return new ImageSharpImageSourceImpl<Rgba32>(name, () =>
            {
                using (var stream = imageStream.Invoke())
                {
                    return Image.Load(stream);
                }
            }, (int)quality);
        }

        private class ImageSharpImageSourceImpl<TPixel> : IImageSource where TPixel : struct, IPixel<TPixel>
    {

            private Image<TPixel> _image;
            private Image<TPixel> Image
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
            private Func<Image<TPixel>> _getImage;
            private readonly int _quality;

            public int Width => Image.Width;
            public int Height => Image.Height;
            public string Name { get; }

            public ImageSharpImageSourceImpl(string name, Func<Image<TPixel>> getImage, int quality)
            {
                Name = name;
                _getImage = getImage;
                _quality = quality;
            }

            public void SaveAsJpeg(MemoryStream ms)
            {
                Image.Clone(m => m.AutoOrient()).SaveAsJpeg(ms, new JpegEncoder() { Quality = _quality });
            }

            public void Dispose()
            {
                Image.Dispose();
            }
        }
    }
}
