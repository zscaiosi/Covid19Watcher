using Covid19Watcher.Application.Errors;
using Covid19Watcher.Application.Helpers;
using System;
using System.Linq;

namespace Covid19Watcher.Application.Services
{
    public class BaseService
    {
        public BaseService()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected SuccessData<T> SuccessData<T>(T data)
        {
            return new SuccessData<T>(data, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        protected ErrorData ErrorData(ServiceErrors error)
        {
            return new ErrorData(error);
        }
    }
    public class ResultData
    {
        public bool Success {get;set;}
    }
    public class ErrorData : ResultData
    {
        public ErrorData(ServiceErrors error)
        {
            Success = false;
            Code = (int)error;
            ErrorMessage = error.GetEnumDescription();
        }
        public int Code {get;set;}
        public string ErrorMessage {get;set;}
    }
    public class SuccessData<T> : ResultData
    {
        public SuccessData(T data, bool isSuccess = true)
        {
            Success = isSuccess;
            Data = data;
        }
        public T Data {get;set;}
    }
}