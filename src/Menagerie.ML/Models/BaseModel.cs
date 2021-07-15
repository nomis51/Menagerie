using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NumSharp;
using NumSharp.Generic;
using static Tensorflow.Binding;

namespace Menagerie.ML.Models
{
    public abstract class BaseModel
    {
        #region Props

        protected string ClassesFilePath { get; }
        protected List<string> Classes { get; private set; }

        #endregion

        #region Constructors

        protected BaseModel(string classesFilePath)
        {
            ClassesFilePath = classesFilePath;
            LoadClasses();
        }

        #endregion

        #region Protected methods
        
        protected static NDArray ReadTensorFromImageFile(string fileName, int inputHeight = 299, int inputWidth = 299, int inputMean = 0, int inputStd = 255)
        {
            var graph = tf.Graph().as_default();
            var fileReader = tf.io.read_file(fileName, "file_reader");
            var imageReader = tf.image.decode_jpeg(fileReader, channels: 3, name: "jpeg_reader");
            var caster = tf.cast(imageReader, tf.float32);
            var dimsExpander = tf.expand_dims(caster, 0);
            var resize = tf.constant(new[] {inputHeight, inputWidth});
            var bilinear = tf.image.resize_bilinear(dimsExpander, resize);
            var sub = tf.subtract(bilinear, new float[] {inputMean});
            var normalized = tf.divide(sub, new float[] {inputStd});
            using var sess = tf.Session(graph);
            return sess.run(normalized);
        }

        protected static NDArray ReadTensorFromImage(Bitmap image, int inputHeight = 299, int inputWidth = 299)
        {
            if (!Directory.Exists(".temp"))
            {
                Directory.CreateDirectory(".temp");
            }

            var filePath = $".temp/{Guid.NewGuid().ToString()}.jpeg";
            image.Save(filePath, ImageFormat.Jpeg);
            return ReadTensorFromImageFile(filePath, inputHeight, inputWidth);
        }

        #endregion

        #region Private methods

        private void LoadClasses()
        {
            if (!File.Exists(ClassesFilePath)) throw new FileNotFoundException(ClassesFilePath);

            var data = File.ReadAllText(ClassesFilePath);

            if (string.IsNullOrEmpty(data)) throw new FileLoadException($"Empty data classes for {ClassesFilePath}");

            Classes = data.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        #endregion
    }
}