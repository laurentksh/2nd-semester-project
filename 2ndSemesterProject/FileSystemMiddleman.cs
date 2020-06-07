using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using _2ndSemesterProject.Models;
using _2ndSemesterProject.Models.Database;
using System.Threading;

namespace _2ndSemesterProject
{
    /// <summary>
    /// Allows us to standardize accesses to user files and folders.
    /// Useful for cases like a server migration or simply moving user datas from one folder to another.
    /// </summary>
    public static class FileSystemMiddleman
    {
        /// <summary>
        /// All IO operations will timeout after this delay (seconds).
        /// </summary>
        public const int IO_OP_TIMEOUT = 60;

        /// <summary>
        /// IO Operation Buffer size.
        /// </summary>
        public const int BUFFER_SIZE = 16384;

        /// <summary>
        /// Relative or Absolute path
        /// </summary>
        private static string BasePath = @"UserFiles\";

        public static Exception LastException;

        /// <summary>
        /// Get the FileStream for the specified relative path. (Direct access)
        /// Prefer using the second GetFile method with a AppUser and a CloudFile.
        /// 
        /// This method does not throws any exception.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns>A FileStream, or null if none found.</returns>
        public static FileStream GetFile(string relativePath)
        {
            try {
                return File.OpenRead(Path.Combine(BasePath, relativePath));
            } catch (Exception) {
                return null;
            }
        }

        /// <summary>
        /// Get the FileStream for the specified AppUser and CloudFile.
        /// 
        /// This method does not throws any exception.
        /// </summary>
        /// <param name="file">CloudFile</param>
        /// <returns>A FileStream, or null if none found.</returns>
        public static FileStream GetFile(CloudFile file, string toAppend = null)
        {
            return GetFile(GetAbsolutePath(file) + toAppend);
        }

        public static string GetAbsolutePath(CloudFile file)
        {
            return Path.Combine(file.OwnerId.ToString(), file.FileId.ToString());
        }

        /// <summary>
        /// Save binary data to the specified path asynchronously.
        /// 
        /// The default timeout delay is available at <see cref="IO_OP_TIMEOUT"/>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="relativePath"></param>
        /// <returns>true: the IO op. was successfull. false: the IO op. failed or took too much time.</returns>
        public static async Task<bool> SaveFile(byte[] data, string relativePath)
        {
            return await SaveFile(new MemoryStream(data), relativePath);
        }

        public static async Task<bool> SaveFile(Stream data, string relativePath)
        {
            CancellationToken token = new CancellationTokenSource(TimeSpan.FromSeconds(IO_OP_TIMEOUT)).Token;
            string fp = Path.Combine(BasePath, relativePath);
            string folder = Path.GetDirectoryName(fp);

            if (!Directory.Exists(BasePath))
                Directory.CreateDirectory(BasePath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            FileStream fs;
            try {
                fs = File.OpenWrite(fp);
            } catch (Exception ex) {
                LastException = ex;
                return false;
            }

            //https://stackoverflow.com/a/58577829/7313891
            try {
                using var ms = new MemoryStream();
                await data.CopyToAsync(ms, token);
                var memBuffer = ms.GetBuffer();

                for (int i = 0; i < memBuffer.Length; i++) {
                    token.ThrowIfCancellationRequested();

                    var nBuffer = new byte[BUFFER_SIZE];

                    for (int j = 0; j < nBuffer.Length && i < memBuffer.Length; j++) {
                        token.ThrowIfCancellationRequested();

                        nBuffer[j] = memBuffer[i];
                        i++;
                    }

                    await fs.WriteAsync(nBuffer, 0, nBuffer.Length);
                }
            } catch (Exception ex) {
                LastException = ex;
                return false;
            }

            await data.DisposeAsync();
            await fs.DisposeAsync();

            return true;
        }

        /// <summary>
        /// Save binary data to the specified path asynchronously.
        /// 
        /// The default timeout delay is available at <see cref="IO_OP_TIMEOUT"/>
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="data"></param>
        /// <returns>true: the IO op. was successfull. false: the IO op. failed or took too much time.</returns>
        public static async Task<bool> SaveFile(byte[] data, AppUser user, CloudFile file, string toAppend = null)
        {
            return await SaveFile(new MemoryStream(data), user, file, toAppend);
        }

        /// <summary>
        /// Save a Stream to the specified path asynchronously.
        /// 
        /// The default timeout delay is available at <see cref="IO_OP_TIMEOUT"/>
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="data"></param>
        /// <returns>true: the IO op. was successfull. false: the IO op. failed or took too much time.</returns>
        public static async Task<bool> SaveFile(Stream data, AppUser user, CloudFile file, string toAppend = null)
        {
            return await SaveFile(data, Path.Combine(user.Id.ToString(), file.FileId.ToString() + toAppend));
        }
    }
}
