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
        /// Relative or Absolute path
        /// </summary>
        private static string BasePath = "/UserFiles/";

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
            return GetFile(Path.Combine(file.OwnerId.ToString(), file.FileId.ToString() + toAppend));
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
            CancellationToken token = new CancellationTokenSource(TimeSpan.FromSeconds(IO_OP_TIMEOUT)).Token;
            
            try {
                await File.WriteAllBytesAsync(Path.Combine(BasePath, relativePath), data, token);
            } catch (Exception) {
                return false;
            }

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
            return await SaveFile(data, Path.Combine(user.Id.ToString(), file.FileId.ToString() + toAppend));
        }
    }
}
