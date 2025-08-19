using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Results;

namespace Proyecto_Planilla_Utils
{
    public interface IToReturn<T>
    {
        T Data { get; }
        int Status { get; }
        string TransactionId { get; }
        string Message { get; }
    }
    public class ToReturn<T> : IToReturn<T>
    {

        public ToReturn(T value) : this() => Data = value;

        public ToReturn()
        {
        }
        public T Data { get; }
        public int Status => (Data == null ? 200 : 200);

        public string TransactionId => DateTime.Now.ToString("yyyyMMddHHmmssFFF");
        public string Message => "";
    }

    public class ToReturnError<T> : IToReturn<T>
    {
        public ToReturnError(string value = "", int codestatus = 500) : this() => (Message, Status) = (value, codestatus);
        public ToReturnError()
        {
        }
        public T Data { get; }
        public int Status { get; }
        public string TransactionId => DateTime.Now.ToString("yyyyMMddHHmmssFFF");
        public string Message { get; }
    }
    public class ToReturnNoEncontrado<T> : IToReturn<T>
    {
        public ToReturnNoEncontrado(string value = "Datos No encontrados", int codestatus = 404) : this() => (Message, Status) = (value, codestatus);
        public ToReturnNoEncontrado()
        {
        }
        public T Data { get; }
        public int Status { get; }
        public string TransactionId => DateTime.Now.ToString("yyyyMMddHHmmssFFF");
        public string Message { get; }
    }
    public class ToReturnValidation<T> : IToReturn<T>
    {

        public ToReturnValidation(FluentValidation.Results.ValidationResult value, int codestatus = 412) : this() => (Message, Status) = (
            // value.Errors[0].ErrorMessage
            string.Join(Environment.NewLine, value.Errors.Select(x => $"- {x.ErrorMessage}.").ToList())
            , codestatus);
        public ToReturnValidation(string value, int codestatus = 412) : this() => (Message, Status) = (value.ToString(), codestatus);

        public ToReturnValidation()
        {
        }
        public T Data { get; }
        public int Status { get; }
        public string TransactionId => DateTime.Now.ToString("yyyyMMddHHmmssFFF");
        public string Message { get; }
    }

    public interface IToReturnList<T>
    {
        IEnumerable<T> Data { get; }
        int Status { get; }
        string Message { get; }
        string TransactionId { get; }
    }

    public class ToReturnList<T> : IToReturnList<T>
    {

        public ToReturnList(IEnumerable<T> value) : this() => Data = value;

        public ToReturnList()
        {
        }


        public IEnumerable<T> Data { get; }
        public int Status => (Data.Any() ? 200 : 200);
        public string TransactionId => DateTime.Now.ToString("yyyyMMddHHmmssFFF");
        public string Message => "";
    }
    public class ToReturnNoEncontradoList<T> : IToReturnList<T>
    {

        public ToReturnNoEncontradoList(string value = "", int codestatus = 404) : this() => (Message, Status) = (value.ToString(), codestatus);
        public ToReturnNoEncontradoList()
        {
        }
        public IEnumerable<T> Data { get; }
        public int Status { get; }
        public string TransactionId => DateTime.Now.ToString("yyyyMMddHHmmssFFF");
        public string Message { get; }
    }
    public class ToReturnErrorList<T> : IToReturnList<T>
    {

        public ToReturnErrorList(string value = "", int codestatus = 500) : this() => (Message, Status) = (value.ToString(), codestatus);
        public ToReturnErrorList()
        {
        }
        public IEnumerable<T> Data { get; }
        public int Status { get; }
        public string TransactionId => DateTime.Now.ToString("yyyyMMddHHmmssFFF");
        public string Message { get; }
    }

}
