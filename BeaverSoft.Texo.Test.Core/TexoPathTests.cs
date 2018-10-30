using System;
using System.IO;
using System.Linq;
using BeaverSoft.Texo.Core.Path;
using Xunit;

namespace BeaverSoft.Texo.Test.Core
{
    public class TexoPathTests
    {
        [Fact]
        public void TexoPath_AbsoluteSimpleFilePath_OneFile()
        {
            TexoPath path = new TexoPath("C:/Windows/System32/Win32_DeviceGuard.dll");
            var files = path.GetFiles();
            var directories = path.GetDirectories();

            Assert.Single(files);
            Assert.Empty(directories);
            Assert.Equal("Win32_DeviceGuard.dll", new FileInfo(files.First()).Name);
        }

        [Fact]
        public void TexoPath_AbsoluteComplexFilePath_OneFile()
        {
            TexoPath path = new TexoPath("C:/Win**32/Win32_DeviceGuard.dll");
            var files = path.GetFiles();
            var directories = path.GetDirectories();

            Assert.Single(files);
            Assert.Empty(directories);
            Assert.Equal("Win32_DeviceGuard.dll", new FileInfo(files.First()).Name);
        }

        [Fact]
        public void TexoPath_RelativeFilePath_MultipleFiles()
        {
            Environment.CurrentDirectory = "d:/working";
            TexoPath path = new TexoPath("texu.ui.test.dir/**/?b?.txt");
            var files = path.GetFiles();
            var directories = path.GetDirectories();

            Assert.NotEmpty(files);
            Assert.Empty(directories);
        }

        [Fact]
        public void TexoPath_RelativePath_MultipleFilesAndDirectories()
        {
            Environment.CurrentDirectory = "d:/working";
            TexoPath path = new TexoPath("texu.ui.test.dir/**b*");
            var files = path.GetFiles();
            var directories = path.GetDirectories();

            Assert.NotEmpty(files);
            Assert.NotEmpty(directories);
        }

        [Fact]
        public void TexoPath_RelativePathTwo_MultipleFilesAndDirectories()
        {
            Environment.CurrentDirectory = "d:/working";
            TexoPath path = new TexoPath("texu.ui.test.dir/**b**");
            var files = path.GetFiles();
            var directories = path.GetDirectories();

            Assert.NotEmpty(files);
            Assert.NotEmpty(directories);
        }
    }
}
