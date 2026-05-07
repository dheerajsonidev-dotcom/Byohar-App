//using Byohar.Application.Requests.UploadFiles;
using Byohar.Application.Requests.UploadFiles;
using Byohar.Application.Responses;
//using Byohar.Application.Responses.UploadFiles;
using Byohar.Shared.Wrapper;

namespace Byohar.Application.Interfaces.Common
{
    public interface IUploadService
    {
        //Task<Result<UploadChunkResponse>> UploadAsync(UploadChunkRequest request);
        Task<Result<string>> UploadAsync(UploadRequest request);
        Task<Result<int>> DeleteAsync(string url);
        Task<Result<int>> DeleteManyAsync(List<string> urls);
        //FileDetails GetFileDetails(string filePath);
    }
}